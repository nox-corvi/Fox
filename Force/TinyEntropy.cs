using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Force;

public class TinyEntropy
{
}

public sealed class DummyEntropyEncoder 
    : IEntropyEncoder
{
    public void EncodeTokens(IEnumerable<LzToken> tokens, BitWriter writer)
    {
        foreach (var token in tokens)
        {
            // Schreibe ein Bit für „Literal“ vs. „Match“
            writer.WriteBit(token.IsLiteral ? 1 : 0);

            if (token.IsLiteral)
            {
                // Literal-Byte in 8 Bits schreiben
                writer.WriteBits(token.Literal!.Value, 8);
            }
            else
            {
                // Placeholder: Matches ignorieren wir erstmal
            }
        }
    }
}

public sealed class DummyEntropyDecoder 
    : IEntropyDecoder
{
    public IEnumerable<LzToken> DecodeTokens(BitReader reader)
    {
        int flag;
        while ((flag = reader.ReadBit()) != -1)
        {
            if (flag == 1)
            {
                uint lit = reader.ReadBits(8);
                yield return new LzToken { Literal = (byte)lit };
            }
            else
            {
                // Matches ignorieren wir erstmal
            }
        }
    }
}