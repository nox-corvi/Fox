using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Force;

//public sealed class AdaptiveHuffmanEncoder : IEntropyEncoder
//{
//    private readonly AdaptiveHuffmanTree _tree;

//    public AdaptiveHuffmanEncoder()
//    {
//        _tree = new AdaptiveHuffmanTree(adaptiveMode: true);
//    }

//    public void EncodeTokens(IEnumerable<LzToken> tokens, BitWriter writer)
//    {
//        foreach (var token in tokens)
//        {
//            if (token.IsLiteral)
//            {
//                // Flag für Literal (z.B. 0)
//                EncodeSymbol(writer, 0);

//                // Literal-Byte
//                EncodeSymbol(writer, token.Literal!.Value);
//            }
//            else if (token.IsMatch)
//            {
//                // Flag für Match (z.B. 1)
//                EncodeSymbol(writer, 1);

//                // Distance
//                EncodeSymbol(writer, token.Distance!.Value);

//                // Length
//                EncodeSymbol(writer, token.Length!.Value);
//            }
//            else
//            {
//                throw new InvalidOperationException("LzToken ist weder Literal noch Match!");
//            }
//        }

//        writer.Flush();
//    }

//    private void EncodeSymbol(BitWriter writer, int symbol)
//    {
//        // Symbol schon bekannt?
//        if (!_tree.HasSymbol(symbol))
//        {
//            // NYT-Code schreiben
//            foreach (var bit in _tree.GetCode(-1)) // -1 = NYT
//                writer.WriteBit(bit);

//            // Symbol roh als Byte schreiben (später evtl. variabel für Distance/Length)
//            writer.WriteBits((uint)symbol, 8);
//        }
//        else
//        {
//            // Symbol-Code schreiben
//            foreach (var bit in _tree.GetCode(symbol))
//                writer.WriteBit(bit);
//        }

//        // Baum updaten
//        _tree.UpdateFrequency(symbol);
//    }
//}

//public sealed class AdaptiveHuffmanDecoder : IEntropyDecoder
//{
//    private readonly AdaptiveHuffmanTree _tree;

//    public AdaptiveHuffmanDecoder()
//    {
//        _tree = new AdaptiveHuffmanTree(adaptiveMode: true);
//    }

//    public IEnumerable<LzToken> DecodeTokens(BitReader reader)
//    {
//        while (true)
//        {
//            int flag = DecodeSymbol(reader);
//            if (flag == -1) yield break; // EOF

//            if (flag == 0)
//            {
//                // Literal
//                int literal = DecodeSymbol(reader);
//                yield return new LzToken { Literal = (byte)literal };
//            }
//            else
//            {
//                // Match
//                int distance = DecodeSymbol(reader);
//                int length = DecodeSymbol(reader);

//                yield return new LzToken { Distance = distance, Length = length };
//            }
//        }
//    }

//    private int DecodeSymbol(BitReader reader)
//    {
//        int nodeIndex = _tree.RootIndex;

//        while (true)
//        {
//            var node = _tree[nodeIndex];

//            if (node.IsLeaf)
//            {
//                if (node.IsNYT)
//                {
//                    // Neues Symbol einlesen
//                    int sym = (int)reader.ReadBits(8);
//                    _tree.UpdateFrequency(sym);
//                    return sym;
//                }
//                else
//                {
//                    int sym = node.Symbol.Value;
//                    _tree.UpdateFrequency(sym);
//                    return sym;
//                }
//            }

//            int bit = reader.ReadBit();
//            if (bit == -1) return -1;

//            nodeIndex = (bit == 0) ? node.Left : node.Right;
//        }
//    }
//}
