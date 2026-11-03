using System.Buffers;
using System.Net.Sockets;
using Common.Packets;
using Common.Packets.Generic;
using Common.Packets.Lobby;
using Common.Utilities;
using Server.Data;

namespace Server;

public class Session : IDisposable
{
     private readonly Func<Session, GameSession> _getGameSession;
     private readonly ILeaderboardSource _leaderboardSource;
     private CancellationTokenSource _cancellationTokenSource;
     private NetworkStreamProcessor _networkStreamProcessor;
     private GameSession? _gameSession;
     private TcpClient? _client;

     public EventHandler<EventArgs>? OnDisconnect;

     public Session(
          TcpClient client, 
          ILeaderboardSource leaderboardSource,
          Func<Session, GameSession> getGameSession)
     {
          _client = client;
          _leaderboardSource = leaderboardSource;
          _getGameSession = getGameSession;
          _client.GetStream().ReadTimeout = 50;
          _ = StartAsync();
     }
     
     public async Task StartAsync()
     {
          _networkStreamProcessor = new NetworkStreamProcessor(_client!.GetStream(), ProcessIncomingData);
          _cancellationTokenSource = new CancellationTokenSource();
          
          try
          {
               await _networkStreamProcessor.StartAsync(_cancellationTokenSource.Token);
          }
          catch (Exception e)
          {
               Console.WriteLine("An exception occured while processing incoming data, disconnecting from client.");
               Console.WriteLine(e);
               Disconnect();
          }
     }

     public void Disconnect()
     {
          _cancellationTokenSource.Cancel();
          _client?.Close();
          _client = null;
          
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
               case PacketType.LobbyCreate:
               {
                    CreateLobby(packet.Id);
                    break;
               }
               case PacketType.LobbyDisconnect:
               {
                    DisconnectFromLobby(packet.Id);
                    break;
               }
               case PacketType.LobbyJoin:
               {
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
                    await _client!.SendAsync(new DeniedPacket(packet.Id));
                    break;
               }
          }
     }

     private void DisconnectFromLobby(ulong id)
     {
          if (_gameSession == null)
          {
               _client!.SendAsync(new DeniedPacket(id)).Wait();
               return;
          }

          _gameSession.Disconnect(this);
     }

     private void CreateLobby(ulong id)
     {
          if (_gameSession != null)
          {
               _client!.SendAsync(new DeniedPacket(id)).Wait();
               return;
          }
                    
          _gameSession = _getGameSession(this);
          _client!.SendAsync(new LobbyInfoPacket(_gameSession.LobbyId, id)).Wait();
     }
}