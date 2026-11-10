using Common.Encoding.ValueEncoders;

namespace Common.Decoding.ValueDecoders;

public abstract class AbstractValueDecoder<T> : IValueDecoder
{
    public Type Type { get; } = typeof(T);
    public abstract object Decode(ReadOnlySpan<byte> data);
}