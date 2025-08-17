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

    #region Properties
    public int MaxMatchLength { get; set; } = 258;
    public int MinMatchLength { get; set; } = 3;

    #endregion

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

        // Hash für die letzten 3 Bytes berechnen
        byte b1 = PrevPrevByte();
        byte b2 = PrevByte();
        int h = ComputeHash(b1, b2, b);

        // Verkettete Liste aktualisieren
        _prev[_pos] = _head[h];
        _head[h] = _pos;

        // Ringposition weiterschieben
        _pos = (_pos + 1) % _windowSize;
    }

    /// <summary>
    /// Gibt das zuletzt eingefügte Byte zurück.
    /// </summary>
    public byte PrevByte()
    {
        // -1 im Ring, wrap-around beachten
        int index = (_pos - 1 + _windowSize) % _windowSize;
        return _buffer[index];
    }

    /// <summary>
    /// Gibt das vorletzte eingefügte Byte zurück.
    /// </summary>
    public byte PrevPrevByte()
    {
        int index = (_pos - 2 + _windowSize) % _windowSize;
        return _buffer[index];
    }

    /// <summary>
    /// Sucht das längste Match ab der aktuellen Position.
    /// Gibt Distance und Length zurück. (0,0) = kein Match.
    /// </summary>
    public (int Distance, int Length) FindMatch(byte next1, byte next2, byte next3, int maxChain = 256)
    {
        // Hashwert berechnen aus den drei kommenden Bytes
        int h = ComputeHash(next1, next2, next3);

        int candidate = _head[h];
        int bestLength = 0;
        int bestDistance = 0;

        // Anzahl der zu prüfenden Kandidaten limitieren
        int chainCount = 0;

        while (candidate != -1 && chainCount < maxChain)
        {
            chainCount++;

            int matchLength = CountMatchLength(candidate, next1, next2, next3);
            if (matchLength > bestLength)
            {
                bestLength = matchLength;
                bestDistance = DistanceFrom(candidate);

                if (bestLength >= MaxMatchLength)
                    break; // besser geht nicht
            }

            candidate = _prev[candidate];
        }

        if (bestLength >= MinMatchLength)
            return (bestDistance, bestLength);

        return (0, 0); // kein Match
    }

    /// <summary>
    /// Vergleicht die Bytes ab candidate-Position und zählt,
    /// wie viele übereinstimmen.
    /// </summary>
    private int CountMatchLength(int candidate, byte n1, byte n2, byte n3)
    {
        int length = 0;
        int pos1 = candidate;
        int pos2 = (_pos + _windowSize) % _windowSize; // „virtuelle“ Startposition

        while (length < MaxMatchLength)
        {
            byte b1 = _buffer[pos1];
            // b2 aus „aktueller“ Inputposition simulieren
            byte b2 = (length == 0) ? n1 : (length == 1) ? n2 : (length == 2) ? n3 : GetFutureByte(length);

            if (b1 != b2)
                break;

            length++;
            pos1 = (pos1 + 1) % _windowSize;
            pos2 = (pos2 + 1) % _windowSize;
        }

        return length;
    }

    /// <summary>
    /// Gibt das Byte aus der Vergangenheit zurück, das „distance“ weit weg ist.
    /// </summary>
    private byte GetFutureByte(int offset)
    {
        int index = (_pos + offset) % _windowSize;
        return _buffer[index];
    }

    /// <summary>
    /// Berechnet die Distanz (1 = letztes Byte, 2 = vorletztes, ...)
    /// </summary>
    private int DistanceFrom(int candidatePos)
    {
        int distance = _pos - candidatePos;
        if (distance <= 0)
            distance += _windowSize;
        return distance;
    }

    /// <summary>
    /// Berechnet einen simplen Hash für 3 Bytes.
    /// </summary>
    private int ComputeHash(byte b1, byte b2, byte b3)
    {
        // klassisch DEFLATE-ähnlich: Schieben und XOR
        return ((b1 << 8) ^ (b2 << 4) ^ b3) & (_head.Length - 1);
    }
}