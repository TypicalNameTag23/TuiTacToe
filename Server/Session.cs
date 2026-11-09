using System.Buffers;
using System.Net.Sockets;
using Common.Packets;
using Common.Packets.Game;
using Common.Packets.Generic;
using Common.Packets.Handshake;
using Common.Packets.Lobby;
using Common.Utilities;
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
     private NetworkStreamProcessor _networkStreamProcessor;
     private DateTime _lastHeartbeat;
     private TcpClient? _client;
     
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
          _client.GetStream().ReadTimeout = 50;
     }

     public async Task StartAsync()
     {
          _cancellationTokenSource = new CancellationTokenSource();
          _networkStreamProcessor = new NetworkStreamProcessor(_client!.GetStream(), ProcessIncomingData);

          try
          {
               var processorTask = _networkStreamProcessor.StartAsync(_cancellationTokenSource.Token);
               var heartbeatTask = StartHeartbeatTask();
               _lastHeartbeat = DateTime.UtcNow;

               await Task.WhenAll(heartbeatTask, processorTask);
          }
          catch (Exception e)
          {
               Console.WriteLine("An exception occured while processing incoming data, disconnecting from client.");
               Console.WriteLine(e);
               Disconnect();
          }
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
                         Disconnect();
                    }
               }
          });
     }

     public void Disconnect()
     {
          GameSession?.OnGameCompleted -= OnGameCompletedAsync;
          GameSession?.OnGuestConnected -= OnOpponentConnectedAsync;
          GameSession?.OnOpponentDisconnected -= OnOpponentDisconnectedAsync;
          GameSession?.Disconnect(this);
          GameSession = null;
          
          _client?.SendAsync(new CloseConnectionPacket()).Wait();
          _client?.Close();
          _client = null;

          _cancellationTokenSource.Cancel();
          OnDisconnect?.Invoke(this, EventArgs.Empty);
     }

     public void Dispose()
     {
          _cancellationTokenSource.Dispose();
          _client?.Dispose();
     }

     private int ProcessIncomingData(ReadOnlySequence<byte> buffer)
     {
          if (!PacketDecoder.TryReadPacket(ref buffer, out var packet))
          {
               return -1;
          }
          
          _ = HandlePacketAsync(packet!);
          return packet!.PacketSizeBytes;
     }

     private async Task HandlePacketAsync(AbstractPacket packet)
     {
          Console.WriteLine($"Received packet: {packet}");
          switch (packet.PacketType)
          {
               case PacketType.OpenConnection:
               {
                    await _client.SendAsync(new AcceptedPacket(packet.Id));
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
                    await CreateLobbyAsync(packet.Id);
                    break;
               }
               case PacketType.LeaveLobby:
               {
                    await HandleLeaveLobbyPacketAsync(packet.Id);
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
                    await _client.SendAsync(new AcceptedPacket(packet.Id));
                    break;
               }
               case PacketType.CloseConnection:
               {
                    Console.WriteLine($"Client {_client!.Client.RemoteEndPoint} disconnected.");
                    Disconnect();
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
                    await _client.SendAsync(new DeniedPacket(packet.Id));
                    break;
               }
          }
     }

     private async Task JoinLobbyAsync(JoinLobbyPacket packet)
     {
          if (!Authenticated || GameSession != null)
          {
               await _client.SendAsync(new DeniedPacket(packet.Id));
               return;
          }
          
          GameSession = _getGameSession(packet.LobbyId);
          if (GameSession == null)
          {
               await _client.SendAsync(new DeniedPacket(packet.Id));
               return;
          }

          GameSession.Connect(this);
          GameSession.OnGameCompleted += OnGameCompletedAsync; 
          GameSession.OnOpponentDisconnected += OnOpponentDisconnectedAsync; 
          
          await _client.SendAsync(new LobbyJoinInformationPacket(packet.Id, GameSession.Host!.User!.Username));
          
          throw new NotImplementedException();
     }

     private async Task RegisterAsync(RegisterPacket packet)
     {
          User = await _tryCreateUserAsync(packet.Username, packet.PasswordHashString);
          if (!Authenticated)
          {
               await _client.SendAsync(new DeniedPacket(packet.Id));
               return;
          }
          
          await _client.SendAsync(new AcceptedPacket(packet.Id));
     }

     private async Task LoginAsync(LoginPacket packet)
     {
          User = await _validatePasswordAsync(packet.Username, packet.PasswordHashString);
          if (!Authenticated)
          {
               await _client.SendAsync(new DeniedPacket(packet.Id));
               return;
          }
          
          await _client.SendAsync(new AcceptedPacket(packet.Id));
     }

     private async Task HandleLeaveLobbyPacketAsync(ulong id)
     {
          if (!Authenticated || GameSession == null)
          {
               await _client.SendAsync(new DeniedPacket(id));
               return;
          }
          
          GameSession.OnGameCompleted -= OnGameCompletedAsync; 
          GameSession.OnGuestConnected -= OnOpponentConnectedAsync; 
          GameSession.OnOpponentDisconnected -= OnOpponentDisconnectedAsync; 
          GameSession.Disconnect(this);
          GameSession = null;
          await _client.SendAsync(new AcceptedPacket(id));
     }

     private async Task CreateLobbyAsync(ulong id)
     {
          if (!Authenticated || GameSession != null)
          {
               await _client.SendAsync(new DeniedPacket(id));
               return;
          }
                    
          GameSession = _createGameSession(this);
          GameSession.OnGameCompleted += OnGameCompletedAsync; 
          GameSession.OnGuestConnected += OnOpponentConnectedAsync; 
          GameSession.OnOpponentDisconnected += OnOpponentDisconnectedAsync; 
          
          await _client!.SendAsync(new LobbyCreationInformationPacket(id, GameSession.LobbyId));
     }

     private async void OnOpponentConnectedAsync(object? sender, Session session)
     {
          await _client.SendAsync(new OpponentConnectedPacket());
     }

     private async void OnOpponentDisconnectedAsync(object? sender, EventArgs e)
     {
          await _client.SendAsync(new OpponentDisconnectedPacket());
     }

     private async void OnGameCompletedAsync(object? sender, Session? winner)
     {
          if (this == winner)
          {
               Console.WriteLine("yay im the winner");
          }
          
          await _client.SendAsync(new GameCompletedPacket());
     }
}