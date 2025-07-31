using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Force;


public class TinyLZ
{

}








public sealed class DummyLzEncoder : ILzEncoder
{
    public IEnumerable<LzToken> Encode(Stream input)
    {
        int b;
        while ((b = input.ReadByte()) != -1)
            yield return new LzToken { Literal = (byte)b };
    }
}

public sealed class DummyLzDecoder : ILzDecoder
{
    public void Decode(IEnumerable<LzToken> tokens, Stream output)
    {
        foreach (var token in tokens)
        {
            if (token.IsLiteral)
                output.WriteByte(token.Literal!.Value);
            // Matches werden noch nicht verarbeitet – kommen erst mit LZ77-Logik
        }
    }
}