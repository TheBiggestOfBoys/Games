using System;

namespace Minesweeper
{
    internal struct Tile()
    {
        public bool IsMine = false;
        public bool IsHidden = true;
        public bool IsFlagged = false;

        public byte SurroundingMines;

        public readonly ConsoleColor DetermineColor() => SurroundingMines switch
        {
            1 => ConsoleColor.Blue,
            2 => ConsoleColor.Green,
            3 => ConsoleColor.Red,
            4 => ConsoleColor.Magenta,
            5 => ConsoleColor.Yellow,
            6 => ConsoleColor.Cyan,
            7 => ConsoleColor.Gray,
            8 => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };

        public override readonly string ToString()
        {
            if (IsHidden) return "?";
            else if (IsFlagged) return "!";
            else if (IsMine && !IsHidden) return "*";
            else return SurroundingMines.ToString();
        }
    }
}
