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
        base.RegisterValues();

        RegisterDynamicSizeValue(() => WinnerUsername, value => WinnerUsername = value);
    }
}