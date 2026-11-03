using System.Buffers;
using System.Net.Sockets;
using Common.Packets;
using Common.Packets.Generic;
using Common.Utilities;
using Server;

namespace Client.Game;

public class ServerConnectionHandler(string address, int port)
{
     private static readonly TimeSpan TimeOut = TimeSpan.FromSeconds(5);

     private Dictionary<ulong, AbstractPacket> _pendingPackets = [];
     private CancellationTokenSource? _cancellationTokenSource;
     private NetworkStreamProcessor? _networkStreamProcessor;
     private TcpClient? _server;

     public EventHandler<EventArgs>? OnConnectionClosed;
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
          }
          catch (SocketException e)
          {
               Console.WriteLine("Failed to connect to the backend server, is it up?");
               Console.WriteLine(e);

               await CloseConnectionAsync();
               return false;
          }
          // catch (Exception e)
          // {
          //      Console.WriteLine("An exception occured while processing incoming data, disconnecting from server.");
          //      Console.WriteLine(e);
          //      
          //      await DisconnectAsync();
          // }
          
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

     public async Task<AbstractPacket?> GetResponseAsync(ulong id, CancellationToken cancellationToken = default)
     {
          if (!Connected)
          {
               return null;
          }
          
          var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource!.Token);
          var token = linkedTokenSource.Token;
          var start = DateTime.Now;
          while (!token.IsCancellationRequested)
          {
               if (_pendingPackets.Remove(id, out var packet))
               {
                    return packet;
               }

               if (DateTime.Now - start > TimeOut)
               {
                    await DisconnectAsync();
               }
          }
          
          return null;
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
          
          HandlePacket(packet!);
          return packet!.PacketSizeBytes;
     }

     private void HandlePacket(AbstractPacket packet)
     {
          Console.WriteLine($"Received packet: {packet}");
          switch (packet.PacketType)
          {
               case PacketType.LobbyInfo:
               case PacketType.Denied:
               case PacketType.Accepted:
               {
                    _pendingPackets.Add(packet.Id, packet);
                    break;
               }
               case PacketType.CloseConnection:
               {
                    OnConnectionClosed?.Invoke(this, EventArgs.Empty);
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