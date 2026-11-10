using Common.Encoding.ValueEncoders;

namespace Common.Encoding;

public class PacketEncoderFactory
{
    private Dictionary<Type, IValueEncoder> _encoders = [];

    public static PacketEncoderFactory New()
    {
        return new PacketEncoderFactory();
    }

    public PacketEncoderFactory WithEncoder(IValueEncoder encoder)
    {
        _encoders.Add(encoder.Type, encoder);

        return this;
    }

    public PacketEncoderFactory WithDefaultEncoders()
    {
        WithEncoder(new ShortEncoder());
        WithEncoder(new IntegerEncoder());
        WithEncoder(new LongEncoder());
        WithEncoder(new UIntEncoder());
        WithEncoder(new ULongEncoder());
        WithEncoder(new PacketTypeEncoder());

        return this;
    }

    public PacketEncoder Create()
    {
        return new PacketEncoder(_encoders);
    }
}