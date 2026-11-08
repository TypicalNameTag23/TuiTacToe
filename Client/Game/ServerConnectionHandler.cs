using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using Common;
using Common.Packets;
using Common.Packets.Generic;
using Common.Packets.Handshake;
using Common.Utilities;

namespace Client.Game;

public class ServerConnectionHandler(string address, int port)
{
     private static readonly TimeSpan TimeOut = TimeSpan.FromSeconds(5);

     private Dictionary<ulong, AbstractPacket> _pendingPackets = [];
     private CancellationTokenSource? _cancellationTokenSource;
     private NetworkStreamProcessor? _networkStreamProcessor;
     private TcpClient? _server;

     public EventHandler<EventArgs>? OnConnectionClosed;
     public EventHandler<AbstractPacket>? OnPacketReceived;
     public bool Connected => _server != null;
     
     public async Task<bool> TryConnectAsync(CancellationTokenSource cancellationTokenSource)
     {
          _cancellationTokenSource = cancellationTokenSource;
          if (Connected)
          {
               return true;
          }

          try
          {
               _server = new TcpClient();
               await _server.ConnectAsync(address, port);
               _networkStreamProcessor = new NetworkStreamProcessor(_server.GetStream(), ProcessIncomingData);
               _ = _networkStreamProcessor.StartAsync(cancellationTokenSource.Token);

               var openConnectionPacket = new OpenConnectionPacket();
               await TrySendAsync(openConnectionPacket);
               
               var response = await GetResponseAsync(openConnectionPacket.Id);
               if (response is not { PacketType: PacketType.Accepted })
               {
                    Console.WriteLine("Failed to connect to the backend server, server responded with non Accepted packet!");
                    
                    await CloseConnectionAsync();
                    return false;
               }

          }
          catch (SocketException e)
          {
               Console.WriteLine("Failed to connect to the backend server, is it up?");
               Console.WriteLine(e);

               await CloseConnectionAsync();
               return false;
          }
          
          return true;
     }

     public async Task DisconnectAsync()
     {
          if (!Connected)
          {
               return;
          }

          if (await TrySendAsync(new CloseConnectionPacket()))
          {
               await CloseConnectionAsync();
          }
     }

     public async Task<bool> TrySendAsync(AbstractPacket packet)
     {
          if (!Connected)
          {
               return false;
          }
          
          try
          {
               await _server!.SendAsync(packet);
          }
          catch (Exception e)
          {
               Console.WriteLine("Failed to send packet");
               Console.WriteLine(e);
               
               await CloseConnectionAsync();
               return false;
          }
          
          return true;
     }

     public async Task<AbstractPacket?> GetResponseAsync(AbstractPacket packet)
     {
          return await GetResponseAsync(packet.Id);
     }
     
     public async Task<AbstractPacket?> GetResponseAsync(ulong id, CancellationToken cancellationToken = default)
     {
          if (!Connected)
          {
               return null;
          }

          using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource!.Token);
          var token = linkedTokenSource.Token;
          var stopwatch = new Stopwatch();
          stopwatch.Start();
          
          return await Task.Run(async () =>
          {
               while (!token.IsCancellationRequested)
               {
                    if (_pendingPackets.Remove(id, out var packet))
                    {
                         stopwatch.Stop();
                         return packet;
                    }

                    if (stopwatch.Elapsed > TimeOut)
                    {
                         stopwatch.Stop();
                         await DisconnectAsync();
                    }
               }
               
               return null;
          }, token);
     }

     private async Task CloseConnectionAsync()
     {
          if (!Connected)
          {
               return;
          }
          
          await _cancellationTokenSource!.CancelAsync();
          _server!.Close();
          _server.Dispose();
          _server = null;
          
          OnConnectionClosed?.Invoke(this, EventArgs.Empty);
     }

     private int ProcessIncomingData(ReadOnlySequence<byte> buffer)
     {
          if (!PacketDecoder.TryReadPacket(ref buffer, out var packet))
          {
               return 0;
          }
          
          OnPacketReceived?.Invoke(this, packet!);
          HandlePacket(packet!);
          return packet!.PacketSizeBytes;
     }

     private void HandlePacket(AbstractPacket packet)
     {
          Console.WriteLine($"Received packet: {packet}");
          switch (packet.PacketType)
          {
               case PacketType.LoginInformation:
               case PacketType.LobbyJoinInformation:
               case PacketType.LobbyCreationInformation:
               case PacketType.Denied:
               case PacketType.Accepted:
               {
                    _pendingPackets.Add(packet.Id, packet);
                    break;
               }
               case PacketType.OpponentConnected:
               case PacketType.OpponentDisconnected:
               case PacketType.GameCompleted:
               {
                    OnPacketReceived?.Invoke(this, packet);
                    break;
               }
               case PacketType.CloseConnection:
               {
                    CloseConnectionAsync().Wait();
                    break;
               }
               default:
               {
                    Console.WriteLine("Received client side packet from server, dropping.");
                    break;
               }
          }
     }
}