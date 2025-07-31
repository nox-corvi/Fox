using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox;

public class Brick
{

}

public class Cell
{
    
}

public class Tetromino
{
    private int[,] Shape;

    public Tetromino(int[,] Shape)
        => this.Shape = Shape;

    public static Tetromino B1 = new(new int[,] { { 1, 1 }, { 1, 1 } });
    public static Tetromino I1 = new(new int[,] { { 1 }, { 1 }, { 1 }, { 1 } });
    public static Tetromino L1 = new(new int[,] { { 1, 0 }, { 1, 0 }, { 1, 0 }, { 1, 1 } });
    public static Tetromino L2 = new(new int[,] { { 0, 1 }, { 0, 1 }, { 0, 1 }, { 1, 1 } });
    public static Tetromino T1 = new(new int[,] { { 0, 1, 0 }, { 1, 1, 1 } });
    public static Tetromino Z1 = new(new int[,] { { 1, 1, 0 }, { 0, 1, 1 } });
    public static Tetromino Z2 = new(new int[,] { { 0, 1, 1 }, { 1, 1, 0 } });

    public static Tetromino[] Tetronimos = { B1, I1, L1, L2, T1, Z1, Z2 };
}

public class Board
{
    private Cell[,] Cells = null!;


}

public class Tetris
    : IGame
{





}
