namespace Common.Decoding.ValueDecoders;

public interface IValueDecoder
{
    public Type Type { get; }
    
    public object Decode(ReadOnlySpan<byte> data);
}