namespace Common.Packets.Game;

public class BoardPlayedPacket : AbstractRespondablePacket
{
    public short X { get; set; }
    public short Y { get; set; }

    internal BoardPlayedPacket() : base(PacketType.BoardPlayed) { }
    
    public BoardPlayedPacket(short x, short y) : base(PacketType.BoardPlayed)
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

        RegisterFixedSizeValue(() => X, value => X = value, sizeof(short));
        RegisterFixedSizeValue(() => Y, value => Y = value, sizeof(short));
    }
}