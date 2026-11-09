namespace Common.Packets.Leaderboard;

public class LeaderboardInformationPacket : AbstractPacket
{
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.LeaderboardInformation;
}