using System;

namespace Minesweeper
{
    public struct Tile()
    {
        public bool IsMine = false;
        public bool IsHidden = true;
        public bool IsFlagged = false;

        public byte SurroundingMines;

        public ConsoleColor Color;

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
            if (IsFlagged) return "!";
            if (IsHidden) return "?";
            if (IsMine) return "*";
            return SurroundingMines.ToString();
        }
    }
}
