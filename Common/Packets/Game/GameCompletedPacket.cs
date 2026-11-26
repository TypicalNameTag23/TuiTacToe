namespace Common.Packets.Game;

public class GameCompletedPacket : AbstractPacket
{
    public string? WinnerUsername { get; set; }

    internal GameCompletedPacket() : base(PacketType.GameCompleted) { }
    
    public GameCompletedPacket(string? winnerUsername) : base(PacketType.GameCompleted)
    {
        WinnerUsername = winnerUsername;
    }
    
    protected override void RegisterValues()
    {
        RegisterDynamicSizeValue(nameof(WinnerUsername), () => WinnerUsername, value => WinnerUsername = value);
    }
}