using Common.Coding.Exceptions;
using Common.Utilities;

namespace Common.Coding.Coders;

public class ULongCoder : ValueCoder<ulong>
{
    public override byte[] Encode(ValueEncoder encoder, CodableValue<ulong> value)
    {
        return BitConverters.GetBytesEndianIndependent(value.Value);
    }

    public override int DecodeInto(ValueDecoder decoder, CodableValue<ulong> value, byte[] data)
    {
        if (data.Length < sizeof(ulong))
        {
            throw new CodingException($"Expected at least {sizeof(ulong)} bytes, but got {data.Length} instead.");
        }

        value.Value = data.GetULong(0);
        return sizeof(ulong);
    }
}