namespace Common.Packets.Generic;

public class DeniedPacket : AbstractResponsePacket
{
    public string? Reason { get; private set; }
    
    internal DeniedPacket() : base(PacketType.Denied) { }

    public DeniedPacket(uint id, string? reason = null) : base(id, PacketType.Denied)
    {
        Reason = reason;
    }

    protected override void RegisterValues()
    {
        base.RegisterValues();
        RegisterDynamicSizeValue(nameof(Reason), () => Reason, v => Reason = v);
    }
}