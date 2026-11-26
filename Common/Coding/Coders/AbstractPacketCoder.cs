using Common.Coding.Exceptions;
using Common.Decoding;
using Common.Encoding;
using Common.Packets;
using Common.Utilities;

namespace Common.Coding.Coders;

public class AbstractPacketCoder : ValueCoder<AbstractPacket?>
{
    public override byte[] Encode(ValueEncoder encoder, CodableValue<AbstractPacket?> value)
    {
        var packetEncoder = new PacketEncoder(encoder);
        
        // if the value is not fixed size we need to also encode the value length
        byte[] encodedValue;
        if (!value.IsFixedSize)
        {
            // nulls are also allowed
            encodedValue = value.Value == null ? [] : packetEncoder.Encode(value.Value);
            var sizeBytes = encodedValue.Length;
            
            var encoded = new byte[sizeof(int) + sizeBytes];
            encoded.CopyInto(BitConverters.GetBytesEndianIndependent(sizeBytes), encodedValue);
            return encoded;
        }
        
        // we know the value is fixed size, null should not be allowed
        if (value.Value == null)
        {
            throw new CodingException("A fixed size value can not be null.");
        }

        // the value is not allowed to be longer or shorter than the expected size
        encodedValue = packetEncoder.Encode(value.Value);
        return encodedValue.Length != value.SizeBytes ? 
            throw new CodingException($"Expected a value of size {value.SizeBytes} bytes but got {encodedValue.Length} bytes instead.") 
            : encodedValue;
    }

    public override int DecodeInto(ValueDecoder decoder, CodableValue<AbstractPacket?> value, byte[] data)
    {
        // not super clean but it works
        var packetDecoder = PacketDecoderFactory
            .New()
            .WithDefaultPackets()
            .WithDefaultDecoders()
            .Create();

        AbstractPacket? packet;
        if (!value.IsFixedSize)                     // if the value is not a fixed size, we also need to decode the length
        {
            var sizeBytes = data.GetInt(0);
            if (sizeBytes == 0)                     // if its zero, the value is null
            {
                value.Value = null;
                return 0;
            }

            packetDecoder.TryReadPacket(data[sizeof(int)..(sizeof(int) + sizeBytes)], out packet);
            value.Value = packet;
            return sizeof(int) + sizeBytes;
        }

        if (data.Length != value.SizeBytes)
        {
            throw new CodingException($"The size of the provided data, {data.Length} bytes, does not match the expected size, {value.SizeBytes} bytes.");
        }
        
        packetDecoder.TryReadPacket(data[sizeof(int)..(sizeof(int) + (int)value.SizeBytes)], out packet);
        value.Value = packet;
        return (int) value.SizeBytes; 
    }
}