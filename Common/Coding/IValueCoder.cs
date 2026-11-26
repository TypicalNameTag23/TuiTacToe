using Common.Packets;

namespace Common.Coding;

public interface IValueCoder
{
    public Type Type { get; }
    
    public byte[] Encode(ValueEncoder encoder, CodableValue value);

    public int DecodeInto(ValueDecoder decoder, CodableValue value, byte[] data);
}

public abstract class ValueCoder<T> : IValueCoder
{
    public Type Type => typeof(T);
    
    public byte[] Encode(ValueEncoder encoder, CodableValue value)
    {
        return Encode(encoder, (CodableValue<T>) value);
    }

    public int DecodeInto(ValueDecoder decoder, CodableValue value, byte[] data)
    {
        return DecodeInto(decoder, (CodableValue<T>) value, data);
    }

    public abstract byte[] Encode(ValueEncoder encoder, CodableValue<T> value);
    public abstract int DecodeInto(ValueDecoder decoder, CodableValue<T> value, byte[] data);
}