namespace Common.Encoding.ValueEncoders;

public interface IValueEncoder
{
    public Type Type { get; }
    
    public byte[] Encode(object data);
}