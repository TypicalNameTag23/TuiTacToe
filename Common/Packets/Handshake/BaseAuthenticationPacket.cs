using System.Buffers;
using System.Text;
using Common.Utilities;

namespace Common.Packets.Handshake;

public abstract class BaseAuthenticationPacket : AbstractPacket
{
    private const int UsernameLengthSizeBytes = sizeof(short);
    private const int PasswordHashSizeBytes = 64;
    
    public override int PacketSizeBytes => PacketBaseSizeBytes + UsernameLengthSizeBytes + UsernameLength + PasswordHashSizeBytes;
    
    public string Username { get; private set; }
    public short UsernameLength => (short) Username.Length;
    public byte[] PasswordHash { get; private set; }
    public string PasswordHashString => Convert.ToBase64String(PasswordHash);

    internal BaseAuthenticationPacket() { }
    
    public BaseAuthenticationPacket(string username, byte[] passwordHash)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("Username and password must neither be null nor empty.");
        }
        
        if (username.Length > short.MaxValue)
        {
            throw new ArgumentException(nameof(username), "Username is too long.");
        }
        
        if (passwordHash.Length != PasswordHashSizeBytes)
        {
            throw new ArgumentException(nameof(passwordHash), $"Password hash should be {PasswordHashSizeBytes} bytes long.");
        }
        
        Username = username;
        PasswordHash = passwordHash;
    }

    public override byte[] Encode()
    {
        var basePacketBytes = base.Encode();
        var usernameLengthBytes = BitConverters.GetBytesEndianIndependent(UsernameLength);
        var usernameBytes = Encoding.ASCII.GetBytes(Username);
        
        var encoded = new byte[PacketSizeBytes];
        encoded.CopyInto(basePacketBytes, usernameLengthBytes, usernameBytes, PasswordHash);
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
        
        var passwordHashBytes = buffer
            .Slice(PacketBaseSizeBytes + UsernameLengthSizeBytes + usernameLength, PasswordHashSizeBytes)
            .FirstSpan;
        PasswordHash = [..passwordHashBytes];

        return true;
    }
}