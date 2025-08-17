using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Force;

public sealed class CompressionCodec
{
    private readonly ILzEncoder _lzEncoder;
    private readonly IEntropyEncoder _entropyEncoder;

    private readonly ILzDecoder _lzDecoder;
    private readonly IEntropyDecoder _entropyDecoder;

    public CompressionCodec(
        ILzEncoder lzEncoder,
        ILzDecoder lzDecoder,
        IEntropyEncoder entropyEncoder,
        IEntropyDecoder entropyDecoder)
    {
        _lzEncoder = lzEncoder;
        _lzDecoder = lzDecoder;
        _entropyEncoder = entropyEncoder;
        _entropyDecoder = entropyDecoder;
    }

    public void Compress(Stream input, Stream output)
    {
        using var bitWriter = new BitWriter(output);
        var tokens = _lzEncoder.Encode(input);
        _entropyEncoder.EncodeTokens(tokens, bitWriter);
        bitWriter.Flush();
    }

    public void Decompress(Stream input, Stream output)
    {
        var bitReader = new BitReader(input);
        var tokens = _entropyDecoder.DecodeTokens(bitReader);
        _lzDecoder.Decode(tokens, output);
    }

    public static CompressionCodec LZ77 = new(
        new Lz77Encoder(), new Lz77Decoder(), null, null); //new AdaptiveHuffmanEncoder(), new AdaptiveHuffmanDecoder());
}
