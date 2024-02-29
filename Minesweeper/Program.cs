using System;
using System.Linq;

namespace Minesweeper
{
    internal class Program
    {
        public static readonly Random random = new();

        static void Main()
        {
            Console.Write("Enter height: ");
            byte height = byte.Parse(Console.ReadLine());

            Console.Write("Enter width: ");
            byte width = byte.Parse(Console.ReadLine());

            Tile[][] grid = new Tile[height][];

            for (int y = 0; y < grid.Length; y++)
            {
                grid[y] = new Tile[width];
                for (int x = 0; x < grid[0].Length; x++)
                {
                    grid[y][x] = new();
                }
            }

            Console.Write("Enter number of mines: ");
            byte numberOfMines = byte.Parse(Console.ReadLine());

            // Place mines
            for (int i = 0; i < numberOfMines; i++)
            {
                int randomX = random.Next(width);
                int randomY = random.Next(height);
                // Make new random values until it is an unused tile
                while (grid[randomY][randomX].IsMine)
                {
                    randomX = random.Next(width);
                    randomY = random.Next(height);
                };
                grid[randomY][randomX].IsMine = true;
            }

            GetSurroundingMines(grid);

            bool clickedMine = false;
            byte xCoord = 0;
            byte yCoord = 0;

            ConsoleKey key = ConsoleKey.Escape;
            while (key != ConsoleKey.Q && !GameOver(grid) && !clickedMine)
            {
                switch (key)
                {
                    #region Navigation
                    case ConsoleKey.UpArrow when xCoord > 0:
                        xCoord--;
                        break;
                    case ConsoleKey.DownArrow when xCoord < grid[0].Length:
                        xCoord++;
                        break;
                    case ConsoleKey.LeftArrow when yCoord > 0:
                        yCoord--;
                        break;
                    case ConsoleKey.RightArrow when yCoord < grid.Length:
                        yCoord++;
                        break;
                    #endregion

                    #region Interaction
                    // Dig
                    case ConsoleKey.Enter:
                        grid[xCoord][yCoord].IsHidden = false;
                        if (grid[xCoord][yCoord].IsMine)
                        {
                            clickedMine = true;
                        }
                        break;

                    // Toggle Flag
                    case ConsoleKey.Spacebar:
                        grid[xCoord][yCoord].IsFlagged = !grid[xCoord][yCoord].IsFlagged;
                        break;
                    #endregion
                }
                Console.WriteLine($"Possible Mines Found: ");
                Console.WriteLine($"Possible Mines Left: ");
                Console.WriteLine($"Hidden Tiles: {GetNumberOfHiddenTiles(grid)}");
                PrintArray(grid, xCoord, yCoord);
                key = Console.ReadKey().Key;
                Console.Clear();
            }
        }

        static void GetSurroundingMines(Tile[][] grid)
        {
            // Define relative coordinates for neighboring cells
            sbyte[] dx = [-1, -1, -1, 0, 0, 1, 1, 1];
            sbyte[] dy = [-1, 0, 1, -1, 1, -1, 0, 1];

            // Calculate mine counts for each cell
            for (byte x = 0; x < grid.Length; x++)
            {
                for (byte y = 0; y < grid[0].Length; y++)
                {
                    byte mineCount = 0;
                    if (!grid[x][y].IsMine)
                    {
                        // Count adjacent mines
                        for (byte i = 0; i < 8; i++)
                        {
                            int newX = x + dx[i];
                            int newY = y + dy[i];

                            if (newX >= 0 && newX < grid.Length && newY >= 0 && newY < grid[0].Length && grid[newX][newY].IsMine)
                            {
                                mineCount++;
                            }
                        }
                    }
                    grid[x][y].SurroundingMines = mineCount;
                }
            }
        }

        static void PrintArray(Tile[][] array, byte xCoord, byte yCoord)
        {
            for (byte x = 0; x < array.Length; x++)
            {
                for (byte y = 0; y < array.Length; y++)
                {
                    if (!array[x][y].IsHidden)
                    {
                        Console.ForegroundColor = array[x][y].DetermineColor();
                    }
                    if (x == xCoord && y == yCoord)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    Console.Write(array[x][y]);
                    if (!array[x][y].IsHidden)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    if (x == xCoord && y == yCoord)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        #region Game Functions
        public static bool GameOver(Tile[][] grid)
        {
            if (!grid.Any(row => row.Any(tile => tile.IsHidden || !tile.IsFlagged)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static byte GetNumberOfHiddenTiles(Tile[][] grid)
        {
            byte count = 0;
            foreach (Tile[] row in grid)
            {
                foreach (Tile tile in row)
                {
                    if (tile.IsHidden)
                        count++;
                }
            }
            return count;
        }
        #endregion
    }
}
