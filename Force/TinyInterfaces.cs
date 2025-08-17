using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Force;

public interface IBitWriter
{
    public void WriteBit(int bit);
    public void WriteBits(uint value, int count);
    public void Flush();
}

public interface IBitReader
{
    public int ReadBit();
    public uint ReadBits(int count);
}

public interface ILzEncoder
{
    IEnumerable<LzToken> Encode(Stream input);
}

public interface ILzDecoder
{
    void Decode(IEnumerable<LzToken> tokens, Stream output);
}

public interface IEntropyEncoder
{
    void EncodeTokens(IEnumerable<LzToken> tokens, BitWriter writer);
}

public interface IEntropyDecoder
{
    IEnumerable<LzToken> DecodeTokens(BitReader reader);
}


public interface IHuffmanNode
{
    public int Weight { get; }
    public int? Symbol { get; } 

    public Node? Parent { get; }
    public Node? Left { get; }
    public Node? Right { get; }

    public bool IsLeaf { get; }
}
public interface IHuffmanTree
{
    void UpdateFrequency(int symbol);
    void Rebalance(); // optional
    string GetCode(int symbol);
}