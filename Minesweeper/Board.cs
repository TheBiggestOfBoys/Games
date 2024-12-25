using System;
using System.Diagnostics;
using System.Text;

namespace Minesweeper
{
    public class Board
    {
        public readonly byte height;
        public readonly byte width;

        public byte xCoord = 0;
        public byte yCoord = 0;

        public readonly ushort NumberOfMines;
        public ushort NumberOfHiddenTiles => GetNumberOfHiddenTiles();
        public ushort NumberOfFlags => GetNumberOfFlags();
        public ushort PotentialRemainingMines => (ushort)(NumberOfFlags - NumberOfFlags);

        public bool GameOver => IsGameOver();
        public bool FirstClick = true;
        public bool ClickedMine = false;

        public float ChanceThatHiddenTileIsMine => 1 / NumberOfHiddenTiles;

        private readonly Tile[,] Grid;

        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public Board(byte h, byte w, ushort mines)
        {
            height = h;
            width = w;

            NumberOfMines = mines;
            Grid = FillBoard(h, w, NumberOfMines);
        }

        private static Tile[,] FillBoard(byte h, byte w, ushort mines)
        {
            Tile[,] tiles = new Tile[h, w];

            Random random = new();
            // Place mines
            for (ushort i = 0; i < mines; i++)
            {
                byte randomX, randomY;
                do
                {
                    randomX = (byte)random.Next(w);
                    randomY = (byte)random.Next(h);
                } while (tiles[randomY, randomX].IsMine);

                tiles[randomY, randomX].IsMine = true;
            }

            // Count surrounding mines
            for (byte row = 0; row < h; row++)
            {
                for (byte col = 0; col < w; col++)
                {
                    tiles[row, col].SurroundingMines = GetSurroundingMines(row, col, tiles);

                    // Then assign color
                    tiles[row, col].Color = tiles[row, col].DetermineColor();

                    // Then hide the tile
                    tiles[row, col].IsHidden = true;
                }
            }

            return tiles;
        }

        #region Acitions
        /// <summary>
        /// Toggles the flag attribute, of the tile at the current <see cref="xCoord"/> & <see cref="yCoord"/>.
        /// </summary>
        public void Flag()
        {
            Grid[yCoord, xCoord].IsFlagged = !Grid[yCoord, xCoord].IsFlagged;
        }

        public void Dig()
        {
            if (FirstClick)
            {
                FirstClick = false;
                Random random = new();

                // Randomly reveal cells until a 0-tile is revealed
                byte randomX, randomY;
                do
                {
                    randomX = (byte)random.Next(width);
                    randomY = (byte)random.Next(height);
                    RevealAdjacentTiles(randomX, randomY);
                } while (!Grid[randomY, randomX].IsHidden);
            }

            Grid[yCoord, xCoord].IsHidden = false;
            if (Grid[yCoord, xCoord].IsMine)
            {
                ClickedMine = true;
            }
        }
        #endregion

        #region Count tile attributes
        private ushort GetNumberOfHiddenTiles()
        {
            ushort hiddenTiles = 0;
            for (byte row = 0; row < height; row++)
            {
                for (byte col = 0; col < width; col++)
                {
                    if (Grid[row, col].IsHidden)
                    {
                        hiddenTiles++;
                    }
                }
            }
            return hiddenTiles;
        }

        private ushort GetNumberOfFlags()
        {
            ushort flagCount = 0;
            for (byte row = 0; row < height; row++)
            {
                for (byte col = 0; col < width; col++)
                {
                    if (Grid[row, col].IsFlagged)
                    {
                        flagCount++;
                    }
                }
            }
            return flagCount;
        }
        private static byte GetSurroundingMines(byte x, byte y, Tile[,] array)
        {
            // Define relative coordinates for neighboring cells
            sbyte[] dx = [-1, -1, -1, 0, 0, 1, 1, 1];
            sbyte[] dy = [-1, 0, 1, -1, 1, -1, 0, 1];

            byte mineCount = 0;
            if (!array[y, x].IsMine)
            {
                // Count adjacent mines
                for (byte i = 0; i < 8; i++)
                {
                    byte newX = (byte)(x + dx[i]);
                    byte newY = (byte)(y + dy[i]);

                    if (newY >= 0 && newY < array.GetLength(0) && newX >= 0 && newX < array.GetLength(1) && array[newY, newX].IsMine)
                    {
                        mineCount++;
                    }
                }
            }
            return mineCount;
        }
        #endregion

        public void RevealAdjacentTiles(byte x, byte y)
        {
            // Check if the coordinates are within the grid and the tile is hidden
            if (!(y >= 0 && y < height && x >= 0 && x < width) || !Grid[y, x].IsHidden)
            {
                return;
            }

            // Reveal the tile
            Grid[y, x].IsHidden = false;

            // If the tile is not a mine and has no surrounding mines, reveal its adjacent tiles
            if (!Grid[y, x].IsMine && Grid[y, x].SurroundingMines == 0)
            {
                // Iterate over each adjacent tile
                for (sbyte dy = -1; dy <= 1; dy++)
                {
                    for (sbyte dx = -1; dx <= 1; dx++)
                    {
                        // Skip the current tile
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        // Compute the coordinates of the adjacent tile
                        byte newX = (byte)(x + dx);
                        byte newY = (byte)(y + dy);

                        // Reveal the adjacent tile
                        RevealAdjacentTiles(newX, newY);
                    }
                }
            }
        }

        /// <summary>
        /// Prints the grid in color
        /// </summary>
        public void PrintArray()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!Grid[y, x].IsHidden)
                    {
                        Console.ForegroundColor = Grid[y, x].Color;
                    }

                    if (y == yCoord && x == xCoord)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                    }

                    Console.Write(Grid[y, x]);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Checks if the game has ended
        /// </summary>
        /// <returns>If the game will end.</returns>
        private bool IsGameOver()
        {
            if (ClickedMine)
            {
                Console.WriteLine($"You clicked a mine at ({xCoord}, {yCoord}).");
                return true;
            }

            // Check if all tiles are revealed (not hidden)
            for (byte row = 0; row < height; row++)
            {
                for (byte col = 0; col < width; col++)
                {
                    if (Grid[row, col].IsHidden)
                    {
                        return false;
                    }
                }
            }

            // Check if all hidden tiles are flagged
            for (byte row = 0; row < height; row++)
            {
                for (byte col = 0; col < width; col++)
                {
                    if (!(Grid[row, col].IsHidden && Grid[row, col].IsFlagged))
                    {
                        return false;
                    }
                }
            }

            // Game is over if either condition is true
            return true;
        }

        public void Main()
        {
            ConsoleKey key = ConsoleKey.Escape;
            while (key != ConsoleKey.Q && !GameOver)
            {
                switch (key)
                {
                    #region Navigation
                    case ConsoleKey.UpArrow when yCoord > 0:
                        yCoord--;
                        break;
                    case ConsoleKey.DownArrow when yCoord < height - 1:
                        yCoord++;

                        break;
                    case ConsoleKey.LeftArrow when xCoord > 0:
                        xCoord--;
                        break;
                    case ConsoleKey.RightArrow when xCoord < width - 1:
                        xCoord++;
                        break;
                    #endregion

                    #region Interaction
                    // Dig
                    case ConsoleKey.Enter:
                        Dig();
                        break;

                    // Toggle Flag
                    case ConsoleKey.Spacebar:
                        Flag();
                        break;
                        #endregion
                }
                PrintArray();
                key = Console.ReadKey().Key;
                Console.Clear();
            }
        }

        public void EndGame()
        {
            stopwatch.Stop();
            if (!ClickedMine)
            {
                Console.WriteLine("You Won!");
            }
            else
            {
                Console.WriteLine("You Lost!");
            }
            Console.WriteLine($"Game lasted: {stopwatch.Elapsed}");
        }

        /// <summary>
        /// The state of the board
        /// </summary>
        /// <returns>The status of the mine and percentages</returns>
        public string Status()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"Possible Mines Found: {NumberOfFlags}");
            stringBuilder.AppendLine($"Possible Mines Left: {PotentialRemainingMines}");
            stringBuilder.AppendLine($"Hidden Tiles: {NumberOfHiddenTiles}");
            stringBuilder.AppendLine($"Chances that tile is mine: {ChanceThatHiddenTileIsMine:P}%");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Indexes the multi-dimensional <see cref="Grid"/>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>The <see cref="Tile"/> at the coordinates</returns>
        public Tile this[int row, int col] => Grid[row, col];

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    stringBuilder.Append(Grid[x, y]);
                }
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }
    }
}
