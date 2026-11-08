using System.Buffers;
using System.Text;
using Common.Utilities;

namespace Common.Packets.Lobby;

public class LobbyJoinInformationPacket : AbstractPacket
{    
    private const int UsernameLengthSizeBytes = sizeof(short);
    
    public override int PacketSizeBytes => PacketBaseSizeBytes + UsernameLengthSizeBytes + UsernameLength;
    public override PacketType PacketType => PacketType.LobbyJoinInformation;
    
    public string Username { get; private set; }
    public short UsernameLength => (short) Username.Length;
    
    internal LobbyJoinInformationPacket() { }
    
    public LobbyJoinInformationPacket(ulong id, string username) : base(id)
    {
        if (username.Length > short.MaxValue)
        {
            throw new ArgumentException("Username is too long.", nameof(username));
        }
        
        Username = username;
    }

    public override byte[] Encode()
    {
        var basePacketBytes = base.Encode();
        var usernameLengthBytes = BitConverters.GetBytesEndianIndependent(UsernameLength);
        var usernameBytes = Encoding.ASCII.GetBytes(Username);
        
        var encoded = new byte[PacketSizeBytes];
        encoded.CopyInto(basePacketBytes, usernameLengthBytes, usernameBytes);
        return encoded;
    }

    internal override bool Load(ReadOnlySequence<byte> buffer)
    {
        if (!base.Load(buffer) || buffer.Length < PacketBaseSizeBytes + UsernameLengthSizeBytes)
        {
            return false;
        }
        
        var usernameLength = buffer.GetShort(PacketBaseSizeBytes, UsernameLengthSizeBytes);
        if (buffer.Length < PacketBaseSizeBytes + UsernameLengthSizeBytes + usernameLength)
        {
            return false;
        }
        
        Username = buffer.GetString(PacketBaseSizeBytes + UsernameLengthSizeBytes, usernameLength);

        return true;
    }
}