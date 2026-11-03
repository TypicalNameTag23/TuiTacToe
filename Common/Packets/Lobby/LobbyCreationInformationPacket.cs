using System.Buffers;
using Common.Utilities;

namespace Common.Packets.Lobby;

public class LobbyCreationInformationPacket : AbstractPacket
{
    private const int LobbyIdSizeBytes = sizeof(int);

    public override int PacketSizeBytes => base.PacketSizeBytes + LobbyIdSizeBytes;
    public override PacketType PacketType => PacketType.LobbyCreationInformation;
    public int LobbyId { get; private set; }
    
    public LobbyCreationInformationPacket() { }

    public LobbyCreationInformationPacket(int lobbyId, ulong id) : base(id)
    {
        LobbyId = lobbyId;
    }
    
    public override byte[] Encode()
    {
        var basePacketBytes = base.Encode();
        var lobbyIdBytes = BitConverter.GetBytes(LobbyId);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(lobbyIdBytes);
        }

        var encoded = new byte[PacketBaseSizeBytes + LobbyIdSizeBytes];
        Buffer.BlockCopy(
            basePacketBytes, 0, 
            encoded, 0, PacketBaseSizeBytes);
        Buffer.BlockCopy(
            lobbyIdBytes, 0, 
            encoded, PacketBaseSizeBytes, LobbyIdSizeBytes);

        return encoded;
    }

    // todo little endian validate data length
    internal override void Load(ReadOnlySequence<byte> buffer)
    {
        base.Load(buffer);
        var slice = buffer
            .Slice(PacketBaseSizeBytes, LobbyIdSizeBytes)
            .FirstSpan;
        
        if (BitConverter.IsLittleEndian)
        {
            slice = slice.Reversed();
        }
        
        var data = BitConverter.ToInt32(slice);
        LobbyId = data;
    }

    public override string ToString() => $"{{ PacketType: {PacketType}, Id: {Id}, LobbyId: {LobbyId} }}";
}