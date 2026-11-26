using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using Common;
using Common.Decoding;
using Common.Encoding;
using Common.Packets;
using Common.Packets.Generic;
using Common.Packets.Handshake;

namespace Client;

public class ServerConnectionHandler : IDisposable
{
     private readonly IPAddress _address;
     private readonly ushort _port;
     private static readonly TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(5);
     
     private CancellationTokenSource? _cancellationTokenSource;
     private PacketHandler? _packetProcessor;
     private TcpClient? _server;

     public EventHandler<EventArgs>? OnConnectionClosed;
     public EventHandler<AbstractPacket>? OnPacketReceived;
     private readonly PacketEncoder _encoder;
     private readonly PacketDecoder _decoder;
     private SslStream _ssl;
     public bool Connected => _server != null;

     public ServerConnectionHandler(IPAddress address, ushort port)
     {
          _address = address;
          _port = port;
          _encoder = PacketEncoderFactory
               .New()
               .WithDefaultEncoders()
               .Create();

          _decoder = PacketDecoderFactory
               .New()
               .WithDefaultDecoders()
               .WithDefaultPackets()
               .Create();
     }
     
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
               await _server.ConnectAsync(_address, _port);
               _ssl = new SslStream(_server.GetStream());
               await _ssl.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
               {
                    RemoteCertificateValidationCallback = delegate { return true; }
               });
               
               _packetProcessor = new PacketHandler(_ssl, _encoder, _decoder, TimeSpan.FromSeconds(5));
               _packetProcessor.OnPacketReceived += (s, p) => OnPacketReceived?.Invoke(s, p);
               _ = _packetProcessor.StartAsync(cancellationTokenSource.Token);

               var openConnectionPacket = new OpenConnectionPacket();
               var response = await TrySendAndGetResponseAsync(openConnectionPacket);
               if (response is not { PacketType: PacketType.Accepted })
               {
                    Console.WriteLine("Failed to connect to the backend server, server responded with non Accepted packet!");
                    
                    await CloseConnectionAsync();
                    return false;
               }

               // TrySendAsync(new AcceptedPacket(1));
               _ = StartHeartbeatTask();
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
               await _packetProcessor!.TrySendAsync(packet);
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

     public async Task<AbstractResponsePacket?> TrySendAndGetResponseAsync(AbstractRespondablePacket packet)
     {
          if (!Connected)
          {
               return null;
          }
          
          try
          {
               return await _packetProcessor!.TrySendAndGetResponseAsync(packet);
          }
          catch (Exception e)
          {
               Console.WriteLine("Failed to send packet");
               Console.WriteLine(e);
               
               await CloseConnectionAsync();
               return null;
          }
     }

     private Task StartHeartbeatTask()
     {
          return Task.Run(async () =>
          {
               while (_cancellationTokenSource is { IsCancellationRequested: false })
               {
                    var packet = new HeartbeatPacket();
                    var response = await TrySendAndGetResponseAsync(packet);
                    if (response is not { PacketType: PacketType.Accepted })
                    {
                         Console.WriteLine("Didn't receive heartbeat response from server, disconnecting.");
                         await CloseConnectionAsync();
                         break;
                    }
                    
                    await Task.Delay(HeartbeatInterval);
               }
          });
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
          Console.WriteLine("Connection closed");
          _server = null;
          
          OnConnectionClosed?.Invoke(this, EventArgs.Empty);
     }

     public void Dispose()
     {
          _cancellationTokenSource?.Dispose();
          _server?.Dispose();
          _ssl.Dispose();
     }
}