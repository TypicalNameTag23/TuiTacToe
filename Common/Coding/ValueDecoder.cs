using Common.Coding.Exceptions;

namespace Common.Coding;

public class ValueDecoder(Dictionary<Type, IValueCoder> coders)
{
    /// <summary>
    /// Tries to read a packet from the provided buffer of bytes
    /// </summary>
    /// <param name="buffer">The buffer to try to read a packet in from</param>
    /// <param name="packet">The packet that was read, null if not able to read packet</param>
    /// <returns>True if a packet was read, otherwise false</returns>
    public int TryDecodeInto(byte[] buffer, Codable codable)
    {
        var bytesRead = 0;
        foreach (var codableValue in codable.CodableValues)
        {
            if (!coders.TryGetValue(codableValue.Type, out var valueDecoder))
            {
                throw new CoderNotFoundException(codableValue.Type);
            }

            bytesRead += valueDecoder.DecodeInto(this, codableValue, buffer[bytesRead..]);
        }
        
        return bytesRead;
    }
}