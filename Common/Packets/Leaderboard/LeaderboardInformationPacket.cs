namespace Common.Packets.Leaderboard;

public class LeaderboardInformationPacket : AbstractResponsePacket
{
    public LeaderboardPlacement? FirstPlace { get; private set; }
    public LeaderboardPlacement? SecondPlace { get; private set; }
    public LeaderboardPlacement? ThirdPlace { get; private set; }

    internal LeaderboardInformationPacket() : base(PacketType.LeaderboardInformation)
    {
    }

    public LeaderboardInformationPacket(uint id, LeaderboardPlacement? firstPlace, LeaderboardPlacement? secondPlace, LeaderboardPlacement? thirdPlace) : base(id,
        PacketType.LeaderboardInformation)
    {
        FirstPlace = firstPlace;
        SecondPlace = secondPlace;
        ThirdPlace = thirdPlace;
    }

    protected override void RegisterValues()
    {
        // frankly I dont like this but I really have either the motivation or time to actually
        // implement a clean system for sending variably sized lists over the network
        // I might work on this later but as of now this will have to do
        base.RegisterValues();
        RegisterDynamicSizeValue(nameof(FirstPlace), () => FirstPlace, t => FirstPlace = t);
        RegisterDynamicSizeValue(nameof(SecondPlace), () => SecondPlace, t => SecondPlace = t);
        RegisterDynamicSizeValue(nameof(ThirdPlace), () => ThirdPlace, t => ThirdPlace = t);
    }
}