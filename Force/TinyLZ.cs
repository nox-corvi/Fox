using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Force;


public class TinyLZ
{

}

public sealed class Lz77Encoder 
    : ILzEncoder
{
    private readonly int _windowSize;
    private readonly int _hashSize;

    public IEnumerable<LzToken> Encode(Stream input)
    {
        var window = new Lz77SlidingWindow(_windowSize, _hashSize);

        // Input puffern, damit wir vorauslesen können
        Queue<byte> lookahead = new Queue<byte>();

        // Erst mal die ersten 3 Bytes einlesen (für Hash)
        for (int i = 0; i < 3; i++)
        {
            int r = input.ReadByte();
            if (r == -1) break;
            lookahead.Enqueue((byte)r);
        }

        while (lookahead.Count > 0)
        {
            // Wenn weniger als 3 Bytes im Lookahead → nur noch Literals möglich
            if (lookahead.Count < 3)
            {
                byte literal = lookahead.Dequeue();
                yield return new LzToken { Literal = literal };
                window.AddByte(literal);

                int r = input.ReadByte();
                if (r != -1) lookahead.Enqueue((byte)r);
                continue;
            }

            // Nächste 3 Bytes für Hash nehmen
            byte n1 = lookahead.ElementAt(0);
            byte n2 = lookahead.ElementAt(1);
            byte n3 = lookahead.ElementAt(2);

            // Match suchen
            var (distance, length) = window.FindMatch(n1, n2, n3);

            if (length >= window.MinMatchLength)
            {
                // Match gefunden: Token ausgeben
                yield return new LzToken
                {
                    Distance = distance,
                    Length = length
                };

                // Die Match-Bytes ins Window füttern und aus lookahead „entfernen“
                for (int i = 0; i < length; i++)
                {
                    window.AddByte(lookahead.Dequeue());

                    // Lookahead auffüllen
                    int r = input.ReadByte();
                    if (r != -1) lookahead.Enqueue((byte)r);
                }
            }
            else
            {
                // Kein Match: Literal ausgeben
                byte literal = lookahead.Dequeue();
                yield return new LzToken { Literal = literal };
                window.AddByte(literal);

                // Lookahead auffüllen
                int r = input.ReadByte();
                if (r != -1) lookahead.Enqueue((byte)r);
            }
        }
    }

    public Lz77Encoder(int windowSize = 32768, int hashSize = 65536)
    {
        _windowSize = windowSize;
        _hashSize = hashSize;
    }
}


public sealed class Lz77Decoder : ILzDecoder
{
    private readonly int _windowSize;

    public Lz77Decoder(int windowSize = 32768)
    {
        _windowSize = windowSize;
    }

    public void Decode(IEnumerable<LzToken> tokens, Stream output)
    {
        var window = new byte[_windowSize];
        int windowPos = 0;

        foreach (var token in tokens)
        {
            if (token.IsLiteral)
            {
                // Literal direkt ausgeben
                byte lit = token.Literal!.Value;
                output.WriteByte(lit);

                // Literal ins Window schreiben
                window[windowPos] = lit;
                windowPos = (windowPos + 1) % _windowSize;
            }
            else if (token.IsMatch)
            {
                int distance = token.Distance!.Value;
                int length = token.Length!.Value;

                // Startposition in Window ermitteln
                int readPos = (windowPos - distance + _windowSize) % _windowSize;

                for (int i = 0; i < length; i++)
                {
                    byte b = window[readPos];
                    readPos = (readPos + 1) % _windowSize;

                    // Byte aus Match ausgeben
                    output.WriteByte(b);

                    // Auch ins Window schieben (für weitere Matches)
                    window[windowPos] = b;
                    windowPos = (windowPos + 1) % _windowSize;
                }
            }
        }
    }
}
