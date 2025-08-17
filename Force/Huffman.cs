using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Force;

public sealed class Node
{
    public int Weight { get; set; }
    public int? Symbol { get; set; } = null;

    public int Order { get; init; }

    public int Parent { get; set; } = -1;
    public int Left { get; set; } = -1;   
    public int Right { get; set; } = -1;  

    public bool IsLeaf => Symbol.HasValue;
    public bool IsNYT => (Symbol.HasValue && Symbol.Value == -1);
}

public sealed class AdaptiveHuffmanTree
{
    private readonly List<Node> Nodes = new();
    private readonly Dictionary<int, int> SymbolMapping = new();

    private int _order = 0;
    private int _root = 0;
    private int _nyt = 0;
    private int _last = 0;

    private Node NewNode(int Parent = -1, int Left = -1, int Right = -1, int? Symbol = null)
        => new() { Order = _order++, Weight = 0, Parent = Parent, Left = Left, Right = Right, Symbol = Symbol };

    private int NextNode()
        => Nodes.Count();

    private void CreateTree()
    {
        Nodes.Clear();
        Nodes.Add(NewNode(Symbol: -1));
        
        SymbolMapping.Clear();
    }

    public void Insert(int Symbol)
    {
        var RT = Nodes[_root];
        var NYT = Nodes[_nyt];

        var NewNYT = NewNode(Parent: _nyt, Symbol: -1);
        var NewSymbol  = NewNode(Parent: _nyt, Symbol: Symbol);

        // new nyt is left of the old nyt
        NYT.Left = _nyt = NextNode();
        Nodes.Add(NewNYT);

        // symbol is right of the old nyt
        NYT.Right = _last = NextNode();
        Nodes.Add(NewSymbol);

        // release old nyt to become a root-node
        NYT.Symbol = null;

        SymbolMapping[Symbol] = _last;
    }

    private (int Code, int Length) GetSymbolCode(int Symbol)
    {
        var CodeSequence = new Stack<int>();
        if (SymbolMapping.ContainsKey(Symbol))
        {
            int current = SymbolMapping[Symbol], p;
            while ((p = Nodes[current].Parent) != -1)
            {
                if (Nodes[p].Left == current)
                    CodeSequence.Push(0);
                
                if (Nodes[p].Right == current)
                    CodeSequence.Push(1);

                current = p;
            }

            int result = 0;
            int c = CodeSequence.Count();
            while (CodeSequence.Count() > 0)
            {
                result <<= 1;

                if (CodeSequence.Pop() == 1)
                    result++;
            }

            return (result, c);
        }
        else
            return (-1, 0);
    }

    public void UpdateWeight(int Symbol, int Weight = 1)
    {

    }

    #region Print

    private int GetNodeDepth(int index)
    {
        int current = index, p, c = 0;
        while ((p = Nodes[current].Parent) != -1)
        {
            c++;
            current = p;
        }

        return c;
    }

    private int GetLeafDepth(int Symbol)
    {
        if (SymbolMapping.ContainsKey(Symbol))
        {
            int current = SymbolMapping[Symbol], p, c = 0;
            
            return GetNodeDepth(current);
        }
        else
            return -1;
    }

    private int GetMaxTreeDepth()
    {
        var c = 0;
        foreach (var Node in Nodes)
        {
            if (Node.Symbol.HasValue)
            {
                c = int.Max(c, GetLeafDepth(Node.Symbol.Value));
            }
        }

        return c;
    }
    #endregion

    public AdaptiveHuffmanTree()
    {
        CreateTree();
    }
}
