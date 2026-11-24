namespace Common.Packets.Game;

public class UpdateBoardPacket : AbstractPacket
{
    public short X { get; set; }
    public short Y { get; set; }
    
    internal UpdateBoardPacket() : base(PacketType.UpdateBoard) { }
    
    public UpdateBoardPacket(short x, short y) : base(PacketType.UpdateBoard)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(x, 2);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(y, 2);
        ArgumentOutOfRangeException.ThrowIfLessThan(x, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(y, 0);
        
        X = x;
        Y = y;
    }
    
    protected override void RegisterValues()
    {
        base.RegisterValues();

        RegisterFixedSizeValue(nameof(X), () => X, value => X = value, sizeof(short));
        RegisterFixedSizeValue(nameof(Y), () => Y, value => Y = value, sizeof(short));
    }
}