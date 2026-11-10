using System.Buffers;
using System.Text;
using Common.Utilities;

namespace Common.Packets.Game;

public class BoardPlayedPacket : AbstractPacket
{
    private const int XSizeBytes = sizeof(short);
    private const int YSizeBytes = sizeof(short);
    
    public short X { get; private set; }
    public short Y { get; private set; }
    public override int PacketSizeBytes => PacketBaseSizeBytes + XSizeBytes + YSizeBytes;
    public override PacketType PacketType => PacketType.BoardPlayed;
    
    internal BoardPlayedPacket() { }
    
    public BoardPlayedPacket(short x, short y)
    {
        X = x;
        Y = y;
    }

    public override byte[] Encode()
    {
        var basePacketBytes = base.Encode();
        var xBytes = BitConverters.GetBytesEndianIndependent(X);
        var yBytes = BitConverters.GetBytesEndianIndependent(Y);
        
        var encoded = new byte[PacketSizeBytes];
        encoded.CopyInto(basePacketBytes, xBytes, yBytes);
        return encoded;
    }

    internal override bool Load(ReadOnlySequence<byte> buffer)
    {
        if (!base.Load(buffer) || buffer.Length < PacketSizeBytes)
        {
            return false;
        }
        
        X = buffer.GetShort(PacketBaseSizeBytes, XSizeBytes);
        Y = buffer.GetShort(PacketBaseSizeBytes, YSizeBytes);
        return true;
    }
}