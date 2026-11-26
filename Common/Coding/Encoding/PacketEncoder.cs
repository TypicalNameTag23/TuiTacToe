using Common.Coding;
using Common.Packets;
using Common.Utilities;

namespace Common.Encoding;

public class PacketEncoder
{
    private readonly ValueEncoder _encoder;

    public PacketEncoder(Dictionary<Type, IValueCoder> coders)
    {
        _encoder = new ValueEncoder(coders);
    }

    public PacketEncoder(ValueEncoder encoder)
    {
        _encoder = encoder;
    }
    
    public byte[] Encode(AbstractPacket packet)
    {
        var encodedPacketType = BitConverters.GetBytesEndianIndependent((short) packet.PacketType);
        var encodedPacket = _encoder.Encode(packet);
        var encoded = new byte[encodedPacketType.Length + encodedPacket.Length];
        encoded.CopyInto(encodedPacketType, encodedPacket);
        
        return encoded;
    }
}