using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Force;

public sealed class BitWriter 
    : IBitWriter, IDisposable
{
    private readonly Stream _stream;
    private byte _buffer;
    private int _bitCount;

    public BitWriter(Stream stream) => _stream = stream;

    public void WriteBit(int bit)
    {
        _buffer = (byte)((_buffer << 1) | (bit & 1));
        _bitCount++;

        if (_bitCount == 8)
            FlushByte();
    }

    public void WriteBits(uint value, int count)
    {
        for (int i = count - 1; i >= 0; i--)
        {
            WriteBit((int)((value >> i) & 1));
        }
    }

    public void Flush()
    {
        if (_bitCount > 0)
        {
            _buffer <<= (8 - _bitCount);  // Auffüllen der verbleibenden Bits
            _stream.WriteByte(_buffer);
            _buffer = 0;
            _bitCount = 0;
        }
    }

    private void FlushByte()
    {
        _stream.WriteByte(_buffer);
        _buffer = 0;
        _bitCount = 0;
    }

    public void Dispose() => Flush();
}

public class BitReader
    : IBitReader
{
    private readonly Stream _stream;
    private byte _buffer;
    private int _bitCount;

    public BitReader(Stream stream)
    {
        _stream = stream;
        _bitCount = 0;
    }

    public int ReadBit()
    {
        if (_bitCount == 0)
        {
            int b = _stream.ReadByte();
            if (b == -1) return -1;
            _buffer = (byte)b;
            _bitCount = 8;
        }

        int bit = (_buffer & 0x80) >> 7;
        _buffer <<= 1;
        _bitCount--;

        return bit;
    }

    public uint ReadBits(int count)
    {
        uint value = 0;
        for (int i = 0; i < count; i++)
        {
            int bit = ReadBit();
            if (bit < 0)
                throw new EndOfStreamException();  // oder „return value“ je nach Design

            value = (value << 1) | (uint)bit;
        }
        return value;
    }
}
