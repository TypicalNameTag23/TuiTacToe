using System.Buffers;

namespace Common;

// todo: later

/// <summary>
/// 
/// </summary>
/// Todo: Allow reading more than 8 bytes (less bit offset) at once; do testing
public class BitReader
{
    // private readonly Stream _stream;
    private readonly ReadOnlySequence<byte> _sequence;
    private ulong _buffer;
    
    // The number of bits that have been offset in the buffer, should be removed at first opportunity
    private ushort _offsetBits;
    // Number of bits read but not removed from the buffer, the difference between _bitOffset and _offsetBits gives how many bits need to be offset at the end of the buffer
    private ushort _bitOffset;
    // The number of bytes in the buffer that are free to be filled
    private ushort _freeBytes = 8;
    
    // Debug value
    private ulong _readBits;
    
    public BitReader(ReadOnlySequence<byte> sequence)
    {
        _sequence = sequence ;
        FillBuffer();
    }

    public ulong ReadBits(int numberOfBits)
    {
        if (numberOfBits > 64 - _offsetBits)
        {
            throw new Exception();
        }

        var bits = _buffer >> (64 - numberOfBits);
        Advance(numberOfBits);
        return bits;
    }

    public ulong ReadBytes(int numberOfBytes)
    {
        return ReadBits(numberOfBytes * 8);
    }
    
    public byte ReadByte()
    {
        return (byte) ReadBits(8);
    }

    public bool ReadBoolean()
    {
        var bit = _buffer >> 63 == 0;
        Advance(1);
        return bit;
    }

    public void Skip(int numberOfBits)
    {
        if (numberOfBits > 64 - _offsetBits)
        {
            throw new Exception();
        }
        
        Advance(numberOfBits);
    }

    private void FillBuffer()
    {
        while (_freeBytes != 0)
        {
            if (_freeBytes != 64 && _offsetBits != 0)
            {
                _buffer <<= 8 - _offsetBits;
            }
            else
            {
                _buffer <<= 8;
            }
            
            var b = (ulong) _sequence.FirstSpan[0];
            _buffer |= b;
            _freeBytes -= 1;
        }
        
        if (_bitOffset == 0)
        {
            _offsetBits = 0;
        }
        else if (_bitOffset - _offsetBits != 0)
        {
            if (_bitOffset - _offsetBits > 0)
            {
                _buffer <<= _bitOffset - _offsetBits;
            }
            else
            {
                _buffer <<= _bitOffset;
            }
            _offsetBits += (ushort) (_bitOffset - _offsetBits);
        }
    }

    private void Advance(int numberOfBits)
    {
        _readBits += (ulong) numberOfBits;
        _freeBytes = (ushort) Math.Floor(numberOfBits / 8f);
        _bitOffset += (ushort) (numberOfBits % 8);
        if (_bitOffset >= 8)
        {
            _freeBytes += 1;
            _bitOffset -= 8;
        }
        
        Debug();
        FillBuffer();
    }

    public void Debug()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Read Bits: {_readBits} | Read Bytes: {(int) Math.Floor(_readBits / 8f)} | Buffer Size: {64 - _offsetBits} | Free Bytes: {_freeBytes} | Bit Offset: {_bitOffset} | Offset Bits: {_offsetBits}");
        var binary = Convert.ToString((long)_buffer, 2).PadLeft(64, '0');
        for (var i = 0; i < binary.Length; i++)
        {
            Console.BackgroundColor = ConsoleColor.Black;

            if ((i + _offsetBits) % 8 == 0 && i != 0)
            {
                Console.Write(" ");
            }

            if (i < _freeBytes * 8 - _offsetBits)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
            }
            
            if (i >= _freeBytes * 8 && i < _freeBytes * 8 + (_bitOffset - _offsetBits))
            {
                Console.BackgroundColor = ConsoleColor.Red;
            }

            if (i >= 64 - _offsetBits)
            {
                Console.BackgroundColor = ConsoleColor.Red;
            }
            
            Console.Write(binary[i]);
        }
        
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine();
        Console.WriteLine();
    }
}