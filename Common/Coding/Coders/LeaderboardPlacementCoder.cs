using Common.Coding;
using Common.Utilities;

namespace Common.Decoding.ValueDecoders;

public class LeaderboardPlacementCoder : ValueCoder<LeaderboardPlacement?>
{
    public override byte[] Encode(ValueEncoder encoder, CodableValue<LeaderboardPlacement?> value)
    {
        var encodedValue = value.Value is null ? [] : encoder.Encode(value.Value);
        var sizeBytes = encodedValue.Length;
        
        var encoded = new byte[sizeof(int) + sizeBytes];
        encoded.CopyInto(BitConverters.GetBytesEndianIndependent(sizeBytes), encodedValue);
        return encoded;
    }

    public override int DecodeInto(ValueDecoder decoder, CodableValue<LeaderboardPlacement?> value, byte[] data)
    {
        var sizeBytes = data.GetInt(0);
        if (sizeBytes == 0)
        {
            value.Value = null;
            return sizeof(int);
        }

        var decoded = new LeaderboardPlacement();
        var bytesRead = decoder.TryDecodeInto(data[sizeof(int)..], decoded);

        Console.WriteLine($"{bytesRead}, {sizeBytes}");
        value.Value = decoded;
        
        return sizeof(int) + sizeBytes;
    }
}