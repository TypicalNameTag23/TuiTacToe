using Common.Coding;
using Common.Coding.Coders;

namespace Common.Encoding;

public class PacketEncoderFactory
{
    private Dictionary<Type, IValueCoder> _coders = [];

    public static PacketEncoderFactory New()
    {
        return new PacketEncoderFactory();
    }

    public PacketEncoderFactory WithCoder(IValueCoder encoder)
    {
        _coders.Add(encoder.Type, encoder);

        return this;
    }

    public PacketEncoderFactory WithDefaultEncoders()
    {
        WithCoder(new ShortCoder());
        WithCoder(new IntegerCoder());
        WithCoder(new LongCoder());
        WithCoder(new UIntCoder());
        WithCoder(new ULongCoder());
        WithCoder(new StringCoder());
        WithCoder(new ByteArrayCoder());
        WithCoder(new LeaderboardPlacementCoder());

        return this;
    }

    public PacketEncoder Create()
    {
        return new PacketEncoder(_coders);
    }
}