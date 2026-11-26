using Common.Coding.Exceptions;
using Common.Utilities;

namespace Common.Coding.Coders;

public class ByteArrayCoder : ValueCoder<byte[]>
{
    public override byte[] Encode(ValueEncoder encoder, CodableValue<byte[]> value)
    {
        // if the value is not fixed size we need to also encode the value length
        if (!value.IsFixedSize)
        {
            // nulls are also allowed
            var sizeBytes = value.Value?.Length ?? 0;
            var encoded = new byte[sizeof(int) + sizeBytes];
            encoded.CopyInto(BitConverters.GetBytesEndianIndependent(sizeBytes), value.Value ?? []);
            return encoded;
        }
        
        // we know the value is fixed size, null should not be allowed
        if (value.Value == null)
        {
            throw new CodingException("A fixed size value can not be null.");
        }

        // the value is not allowed to be longer or shorter than the expected size
        return value.Value.Length != value.SizeBytes ? 
            throw new CodingException($"Expected a value of size {value.SizeBytes} bytes but got {value.Value.Length} bytes instead.") 
            : value.Value;
    }

    public override int DecodeInto(ValueDecoder decoder, CodableValue<byte[]> value, byte[] data)
    {
        if (!value.IsFixedSize)                     // if the value is not a fixed size, we also need to decode the length
        {
            var sizeBytes = data.GetInt(0);
            if (sizeBytes == 0)                     // if its zero, the value is null
            {
                value.Value = null;
                return 0;
            }

            value.Value = data[sizeof(int)..sizeBytes];
            return sizeof(int) + sizeBytes;
        }

        if (data.Length != value.SizeBytes)
        {
            throw new CodingException($"The size of the provided data, {data.Length} bytes, does not match the expected size, {value.SizeBytes} bytes.");
        }
        
        value.Value = data[..(int)value.SizeBytes];
        return (int) value.SizeBytes; 
    }
}