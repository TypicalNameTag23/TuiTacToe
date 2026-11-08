namespace Common.Packets.Leaderboard;

public class ViewLeaderboardPacket : AbstractPacket
{
    public ViewLeaderboardPacket() { }

    public ViewLeaderboardPacket(ulong id) : base(id) { }
    
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.ViewLeaderboard;
}