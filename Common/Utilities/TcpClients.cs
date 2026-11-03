using System.Net.Sockets;
using Common.Packets;

namespace Common.Utilities;

public static class TcpClients
{
    public static async Task SendAsync(this TcpClient? tcpClient, byte[] message)
    {
        if (tcpClient == null)
        {
            return;
        }
        
        var stream = tcpClient.GetStream();
        await stream.WriteAsync(message);
    }

    public static async Task SendAsync(this TcpClient? tcpClient, AbstractPacket packet)
    {
        if (tcpClient == null)
        {
            return;
        }
        
        await tcpClient.SendAsync(packet.Encode());
        Console.WriteLine($"Sent packet: {packet}");
    }
}