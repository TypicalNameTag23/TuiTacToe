namespace Common.Encoding.ValueEncoders;

public abstract class AbstractValueEncoder<T> : IValueEncoder
{
    public Type Type { get; } = typeof(T);
    
    public abstract byte[] Encode(T data);

    public byte[] Encode(object data)
    {
        return Encode((T)data);
    }
}