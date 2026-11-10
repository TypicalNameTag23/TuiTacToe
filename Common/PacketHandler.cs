using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using Common.Decoding;
using Common.Encoding;
using Common.Packets;
using Common.Utilities;

namespace Common;

public class PacketHandler
{
     private uint packetId = 0;
     private readonly TimeSpan _timeOut;
     private CancellationToken _cancellationToken;

     private readonly Dictionary<ulong, AbstractResponsePacket?> _pendingResponsePackets = [];
     private readonly TcpClient _connection;
     private readonly PacketEncoder _encoder;
     private readonly PacketDecoder _decoder;

     public EventHandler<AbstractPacket>? OnPacketReceived;

     public PacketHandler(TcpClient connection, PacketEncoder encoder, PacketDecoder decoder, TimeSpan timeOut)
     {
          _connection = connection;
          _encoder = encoder;
          _decoder = decoder;
          _timeOut = timeOut;
     }

     public async Task StartAsync(CancellationToken cancellationToken)
     {
          _cancellationToken = cancellationToken;

          var networkProcessor = new NetworkStreamProcessor(_connection.GetStream(), ProcessIncomingData);
          await networkProcessor.StartAsync(cancellationToken);
     }
     
     public async Task<bool> TrySendAsync(AbstractPacket packet)
     {
          if (packet is AbstractRespondablePacket responsePacket)
          {
               responsePacket.Id = packetId++;
          }
          
          try
          {
               await _connection.SendAsync(_encoder.Encode(packet), _cancellationToken);
               Console.WriteLine($"Sent packet: {packet}");
          }
          catch (Exception e)
          {
               Console.WriteLine("Failed to send packet");
               Console.WriteLine(e);
               
               return false;
          }
          
          return true;
     }
     
     public async Task<AbstractResponsePacket?> TrySendAndGetResponseAsync(AbstractRespondablePacket packet)
     {
          try
          {
               await TrySendAsync(packet);
               _pendingResponsePackets[packet.Id] = null;
               
               return await GetResponseAsync(packet);
          }
          catch (Exception e)
          {
               Console.WriteLine("Failed to send packet");
               Console.WriteLine(e);
               
               return null;
          }
     }

     private Task<AbstractResponsePacket?> GetResponseAsync(AbstractRespondablePacket packet)
     {
          var stopwatch = new Stopwatch();
          stopwatch.Start();
          return Task.Run(() =>
          {
               while (!_cancellationToken.IsCancellationRequested)
               {
                    if (_pendingResponsePackets[packet.Id] != null)
                    {
                         _pendingResponsePackets.Remove(packet.Id, out var response);
                         stopwatch.Stop();

                         return response!;
                    }

                    if (stopwatch.Elapsed > _timeOut)
                    {
                         stopwatch.Stop();
                         return null;
                    }
               }
               
               return null;
          }, _cancellationToken);
     }

     private int ProcessIncomingData(ReadOnlySequence<byte> buffer)
     {
          var bytesRead = _decoder.TryReadPacket(buffer, out var packet);
          if (bytesRead == -1)
          {
               return -1;
          }
          
          Console.WriteLine($"Received packet: {packet}");
          if (packet is AbstractResponsePacket responsePacket 
              && _pendingResponsePackets.ContainsKey(responsePacket.Id))
          {
               _pendingResponsePackets[responsePacket.Id] = responsePacket;
               return bytesRead;
          }
          
          OnPacketReceived?.Invoke(this, packet!);
          return bytesRead;
     }
}