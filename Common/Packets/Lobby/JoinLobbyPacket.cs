using System.Buffers;
using Common.Utilities;

namespace Common.Packets.Lobby;

public class JoinLobbyPacket : AbstractPacket
{
    private const int LobbyIdSizeBytes = sizeof(int);
    
    public override int PacketSizeBytes => PacketBaseSizeBytes + LobbyIdSizeBytes;
    public override PacketType PacketType => PacketType.JoinLobby;
    public int LobbyId { get; private set; }
    
    internal JoinLobbyPacket() { }

    public JoinLobbyPacket(int lobbyId)
    {
        LobbyId = lobbyId;
    }

    public override byte[] Encode()
    {
        var basePacketBytes = base.Encode();
        var lobbyIdBytes = BitConverters.GetBytesEndianIndependent(LobbyId);

        var encoded = new byte[PacketBaseSizeBytes + LobbyIdSizeBytes];
        encoded.CopyInto(basePacketBytes, lobbyIdBytes);
        return encoded;
    }

    internal override bool Load(ReadOnlySequence<byte> buffer)
    {
        if (!base.Load(buffer) || buffer.Length < PacketBaseSizeBytes)
        {
            return false;
        }
        
        LobbyId = buffer.GetInt(PacketBaseSizeBytes, LobbyIdSizeBytes);
        return true;
    }
}