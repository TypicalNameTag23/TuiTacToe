using System.Net.Sockets;
using Common;
using Common.Decoding;
using Common.Encoding;
using Common.Packets;
using Common.Packets.Game;
using Common.Packets.Generic;
using Common.Packets.Handshake;
using Common.Packets.Lobby;
using Server.Data;
using Server.Data.Models;

namespace Server;

public class Session : IDisposable
{
     private static readonly TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(10);
     private static readonly TimeSpan HeartbeatCheckInterval = TimeSpan.FromSeconds(5);
     
     private readonly Func<int, GameSession?> _getGameSession;
     private readonly Func<Session, GameSession> _createGameSession;
     private readonly Func<string, string, Task<User?>> _tryCreateUserAsync;
     private readonly Func<string, string, Task<User?>> _validatePasswordAsync;
     private readonly ILeaderboardSource _leaderboardSource;
     
     private CancellationTokenSource _cancellationTokenSource;
     private PacketHandler _packetHandler;
     private DateTime _lastHeartbeat;
     private TcpClient? _client;
     private readonly PacketEncoder _encoder;
     private readonly PacketDecoder _decoder;

     public User? User { get; private set; }
     public GameSession? GameSession { get; private set; }
     public bool Authenticated => User != null;

     public EventHandler<EventArgs>? OnDisconnect { get; set; }

     public Session(
          TcpClient client, 
          ILeaderboardSource leaderboardSource,
          Func<Session, GameSession> createGameSession,
          Func<int, GameSession?> getGameSession,
          Func<string, string, Task<User?>> tryCreateUserAsync,
          Func<string, string, Task<User?>> validatePasswordAsync)
     {
          _client = client;
          _leaderboardSource = leaderboardSource;
          _createGameSession = createGameSession;
          _getGameSession = getGameSession;
          _tryCreateUserAsync = tryCreateUserAsync;
          _validatePasswordAsync = validatePasswordAsync;
          _encoder = PacketEncoderFactory
               .New()
               .WithDefaultEncoders()
               .Create();

          _decoder = PacketDecoderFactory
               .New()
               .WithDefaultDecoders()
               .WithDefaultPackets()
               .Create(); 
          
          // _client.GetStream().ReadTimeout = 50;
     }

     public async Task StartAsync()
     {
          _cancellationTokenSource = new CancellationTokenSource();
          _packetHandler = new PacketHandler(_client!, _encoder, _decoder, TimeSpan.FromSeconds(5));
          _packetHandler.OnPacketReceived += (_, packet) => _ = OnPacketReceivedAsync(packet);
          _lastHeartbeat = DateTime.UtcNow;
          
          var packetHandlerTask = _packetHandler.StartAsync(_cancellationTokenSource.Token);
          var heartbeatTask = StartHeartbeatTask();
          await Task.WhenAll(packetHandlerTask, heartbeatTask);
     }

     private Task StartHeartbeatTask()
     {
          return Task.Run(async () =>
          {
               while (!_cancellationTokenSource.IsCancellationRequested)
               {
                    await Task.Delay(HeartbeatCheckInterval, _cancellationTokenSource.Token);
                    if (DateTime.UtcNow - _lastHeartbeat >= HeartbeatInterval)
                    {
                         Console.WriteLine($"{(DateTime.UtcNow - _lastHeartbeat).Seconds} seconds since last heartbeat, disconnecting.");
                         await DisconnectAsync();
                    }
               }
          });
     }

     public async Task DisconnectAsync()
     {
          GameSession?.OnGameCompleted -= OnGameCompletedAsync;
          GameSession?.OnGuestConnected -= OnOpponentConnectedAsync;
          GameSession?.OnOpponentDisconnected -= OnOpponentDisconnectedAsync;
          GameSession?.Disconnect(this);
          GameSession = null;
          
          await _packetHandler.TrySendAsync(new CloseConnectionPacket());
          _client?.Close();
          _client = null;

          await _cancellationTokenSource.CancelAsync();
          OnDisconnect?.Invoke(this, EventArgs.Empty);
     }

     public void Dispose()
     {
          _cancellationTokenSource.Dispose();
          _client?.Dispose();
     }

     private async Task OnPacketReceivedAsync(AbstractPacket packet)
     {
          switch (packet.PacketType)
          {
               case PacketType.OpenConnection:
               {
                    var p = (AbstractRespondablePacket)packet;
                    await _packetHandler.TrySendAsync(new AcceptedPacket(p.Id));
                    break;
               }
               case PacketType.Register:
               {
                    await RegisterAsync((RegisterPacket) packet);
                    break;
               }
               case PacketType.Login:
               {
                    await LoginAsync((LoginPacket) packet);
                    break;
               }
               case PacketType.CreateLobby:
               {
                    var p = (AbstractRespondablePacket)packet;
                    await CreateLobbyAsync(p.Id);
                    break;
               }
               case PacketType.LeaveLobby:
               {
                    var p = (AbstractRespondablePacket)packet;
                    await HandleLeaveLobbyPacketAsync(p.Id);
                    break;
               }
               case PacketType.JoinLobby:
               {
                    await JoinLobbyAsync((JoinLobbyPacket) packet);
                    break;
               }
               case PacketType.Heartbeat:
               {
                    _lastHeartbeat = DateTime.UtcNow;
                    var p = (AbstractRespondablePacket)packet;
                    await _packetHandler.TrySendAsync(new AcceptedPacket(p.Id));
                    break;
               }
               case PacketType.CloseConnection:
               {
                    Console.WriteLine($"Client {_client!.Client.RemoteEndPoint} disconnected.");
                    await DisconnectAsync();
                    break;
               }
               case PacketType.BoardPlayed:
               {
                    Console.WriteLine("Cool man");
                    
                    var p = (AbstractRespondablePacket)packet;
                    await _packetHandler.TrySendAsync(new AcceptedPacket(p.Id));
                    break;
               }
               case PacketType.ViewLeaderboard:
               {
                    var leaderboard = await _leaderboardSource.GetLeaderboardAsync(5);
                    // ...
                    
                    break;
               }
               default:
               {
                    Console.WriteLine($"Received server side packet from client {_client!.Client.RemoteEndPoint}");
                    var p = (AbstractRespondablePacket)packet;
                    await _packetHandler.TrySendAsync(new DeniedPacket(p.Id));
                    break;
               }
          }
     }

     private async Task JoinLobbyAsync(JoinLobbyPacket packet)
     {
          if (!Authenticated || GameSession != null)
          {
               await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id));
               return;
          }
          
          GameSession = _getGameSession(packet.LobbyId);
          if (GameSession == null)
          {
               await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id));
               return;
          }

          GameSession.Connect(this);
          GameSession.OnGameCompleted += OnGameCompletedAsync; 
          GameSession.OnOpponentDisconnected += OnOpponentDisconnectedAsync; 
          
          await _packetHandler.TrySendAsync(new LobbyJoinInformationPacket(GameSession.Host!.User!.Username, packet.Id));
          
          throw new NotImplementedException();
     }

     private async Task RegisterAsync(RegisterPacket packet)
     {
          User = await _tryCreateUserAsync(packet.Username, Convert.ToBase64String(packet.PasswordHash));
          if (!Authenticated)
          {
               await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id));
               return;
          }
          
          await _packetHandler.TrySendAsync(new AcceptedPacket(packet.Id));
     }

     private async Task LoginAsync(LoginPacket packet)
     {
          User = await _validatePasswordAsync(packet.Username, Convert.ToBase64String(packet.PasswordHash));
          if (!Authenticated)
          {
               await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id));
               return;
          }
          
          await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id));
     }

     private async Task HandleLeaveLobbyPacketAsync(uint id)
     {
          if (!Authenticated || GameSession == null)
          {
               await _packetHandler.TrySendAsync(new DeniedPacket(id));
               return;
          }
          
          GameSession.OnGameCompleted -= OnGameCompletedAsync; 
          GameSession.OnGuestConnected -= OnOpponentConnectedAsync; 
          GameSession.OnOpponentDisconnected -= OnOpponentDisconnectedAsync; 
          GameSession.Disconnect(this);
          GameSession = null;
          await _packetHandler.TrySendAsync(new AcceptedPacket(id));
     }

     private async Task CreateLobbyAsync(uint id)
     {
          if (!Authenticated || GameSession != null)
          {
               await _packetHandler.TrySendAsync(new DeniedPacket(id));
               return;
          }
                    
          GameSession = _createGameSession(this);
          GameSession.OnGameCompleted += OnGameCompletedAsync; 
          GameSession.OnGuestConnected += OnOpponentConnectedAsync; 
          GameSession.OnOpponentDisconnected += OnOpponentDisconnectedAsync; 
          
          await _packetHandler.TrySendAsync(new LobbyCreationInformationPacket(GameSession.LobbyId, id));
     }

     private async void OnOpponentConnectedAsync(object? sender, Session session)
     {
          await _packetHandler.TrySendAsync(new OpponentConnectedPacket());
     }

     private async void OnOpponentDisconnectedAsync(object? sender, EventArgs e)
     {
          await _packetHandler.TrySendAsync(new OpponentDisconnectedPacket());
     }

     private async void OnGameCompletedAsync(object? sender, Session? winner)
     {
          if (this == winner)
          {
               Console.WriteLine("yay im the winner");
          }
          
          await _packetHandler.TrySendAsync(new GameCompletedPacket());
     }
}