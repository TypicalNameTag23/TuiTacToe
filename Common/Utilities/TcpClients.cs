using System.Net.Sockets;

namespace Common.Utilities;

public static class TcpClients
{
    public static async Task SendAsync(this TcpClient? tcpClient, byte[] message, CancellationToken cancellationToken = default)
    {
         if (tcpClient == null)
        {
            return;
        }
        
        var stream = tcpClient.GetStream();
        await stream.WriteAsync(message, cancellationToken);
    }
}