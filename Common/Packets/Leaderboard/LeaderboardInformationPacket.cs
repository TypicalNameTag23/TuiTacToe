namespace Common.Packets.Leaderboard;

public class LeaderboardInformationPacket : AbstractResponsePacket
{
    internal LeaderboardInformationPacket() : base(PacketType.LeaderboardInformation)
    {
    }

    public LeaderboardInformationPacket(uint id, List<LeaderboardPlacement> placements) : base(id,
        PacketType.LeaderboardInformation)
    {
        Placements = placements;
    }

    public List<LeaderboardPlacement> Placements { get; private set; }

    protected override void RegisterValues()
    {
        base.RegisterValues();
        RegisterListValue(nameof(Placements), () => Placements, t => Placements = t);
    }
}