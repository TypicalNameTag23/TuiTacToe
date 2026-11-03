namespace Common.Packets;

public class ViewLeaderboardPacket : AbstractPacket
{
    public ViewLeaderboardPacket() { }

    public ViewLeaderboardPacket(ulong id) : base(id) { }
    
    public override PacketType PacketType => PacketType.ViewLeaderboard;
}