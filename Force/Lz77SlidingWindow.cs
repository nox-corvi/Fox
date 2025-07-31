using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Force;

public sealed class Lz77SlidingWindow
{
    private readonly byte[] _buffer;
    private readonly int _windowSize;

    private readonly int[] _head;   // hash → letzte Position
    private readonly int[] _prev;   // pos → vorherige Position mit gleichem Hash

    private int _pos;               // aktuelle Position im „Ring“

    public Lz77SlidingWindow(int windowSize = 32768, int hashSize = 65536)
    {
        _windowSize = windowSize;
        _buffer = new byte[windowSize];

        _head = Enumerable.Repeat(-1, hashSize).ToArray();
        _prev = Enumerable.Repeat(-1, windowSize).ToArray();

        _pos = 0;
    }

    public void AddByte(byte b)
    {
        _buffer[_pos] = b;

        // Hash berechnen und Verkettung anlegen
        int h = ComputeHash(b, PrevByte(), PrevPrevByte());
        _prev[_pos] = _head[h];
        _head[h] = _pos;

        _pos = (_pos + 1) % _windowSize;
    }

    // TODO: Lookup-Methode: Finde Kandidatenpositionen für aktuelle Bytes

    private int ComputeHash(byte b1, byte b2, byte b3)
    {
        // sehr simpler Hash, später „tunen“
        return ((b1 << 8) ^ (b2 << 4) ^ b3) & (_head.Length - 1);
    }
}