namespace Common.Packets.Leaderboard;

public class LeaderboardInformationPacket : AbstractResponsePacket
{
    internal LeaderboardInformationPacket() : base(PacketType.LeaderboardInformation) { }
    
    public LeaderboardInformationPacket(uint id) 
        : base(id, PacketType.LeaderboardInformation)
    {
        // todo
    }
}