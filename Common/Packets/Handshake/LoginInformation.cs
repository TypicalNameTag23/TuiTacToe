using System.Buffers;
using System.Text;
using Common.Utilities;

namespace Common.Packets.Handshake;

public class LoginInformation : AbstractPacket
{
    private const int SessionIdSizeBytes = 64;
    
    public override PacketType PacketType => PacketType.LoginInformation;

    public override int PacketSizeBytes => PacketBaseSizeBytes + SessionIdSizeBytes;
    public string SessionId { get; private set; }
    
    internal LoginInformation() { }
    
    public LoginInformation(ulong id, string sessionId) : base(id)
    {
        SessionId = sessionId;
    }

    public override byte[] Encode()
    {
        var basePacketBytes = base.Encode();
        var sessionIdBytes = Encoding.ASCII.GetBytes(SessionId);
        
        var encoded = new byte[PacketSizeBytes];
        encoded.CopyInto(basePacketBytes, sessionIdBytes);
        return encoded;
    }

    internal override bool Load(ReadOnlySequence<byte> buffer)
    {
        if (!base.Load(buffer) || buffer.Length < PacketSizeBytes)
        {
            return false;
        }
        
        SessionId = buffer.GetString(PacketBaseSizeBytes, SessionIdSizeBytes);
        
        return true;
    }
}