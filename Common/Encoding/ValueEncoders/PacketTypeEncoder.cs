using Common.Packets;
using Common.Utilities;

namespace Common.Encoding.ValueEncoders;

public class PacketTypeEncoder : AbstractValueEncoder<PacketType>
{
    public override byte[] Encode(PacketType data)
    {
        return BitConverters.GetBytesEndianIndependent((short) data);
    }
}