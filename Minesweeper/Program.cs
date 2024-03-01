using System;
using System.Diagnostics;
using System.Linq;

namespace Minesweeper
{
    internal class Program
    {
        public static readonly Random random = new();

        public static byte xCoord = 0;
        public static byte yCoord = 0;

        public static byte height;
        public static byte width;

        public static Tile[][] grid;

        static void Main()
        {
            byte height;
            do { Console.Write("Enter height: "); }
            while (!byte.TryParse(Console.ReadLine(), out height));

            byte width;
            do { Console.Write("Enter width: "); }
            while (!byte.TryParse(Console.ReadLine(), out width));

            grid = new Tile[height][];

            // Initialize grid and place mines
            for (byte y = 0; y < height; y++)
            {
                grid[y] = new Tile[width];
                for (byte x = 0; x < width; x++)
                {
                    grid[y][x] = new Tile();
                }
            }

            ushort numberOfMines;
            do { Console.Write("Enter number of mines: "); }
            while (!ushort.TryParse(Console.ReadLine(), out numberOfMines));

            // Place mines
            for (ushort i = 0; i < numberOfMines; i++)
            {
                byte randomX, randomY;
                do
                {
                    randomX = (byte)random.Next(width);
                    randomY = (byte)random.Next(height);
                } while (grid[randomY][randomX].IsMine);

                grid[randomY][randomX].IsMine = true;
            }

            // Count the surrounding mines
            for (byte y = 0; y < height; y++)
            {
                for (byte x = 0; x < width; x++)
                {
                    grid[y][x].SurroundingMines = GetSurroundingMines(x, y);
                    grid[y][x].Color = grid[y][x].DetermineColor();
                }
            }

            bool clickedMine = false;
            bool firstClick = true;

            byte flaggedPotentialMines = 0;
            byte potentialRemainingMines = (byte)(numberOfMines - flaggedPotentialMines);

            ushort hiddenTiles = (ushort)GetNumberOfHiddenTiles();

            double chanceThatHiddenTileIsMine = (double)potentialRemainingMines / hiddenTiles;

            #region Tutorial
            Console.WriteLine("The goal of minesweeper is to flag all the mines without accidentally clicking on one.");
            Console.WriteLine($"You can flag suspected mines by pressing {ConsoleKey.Spacebar}, turning the '?' into a '!'.");
            Console.WriteLine($"You can dig suspected safe tiles by pressing {ConsoleKey.Enter}, turning the '?' into the number of surrounding mines, or '*' if you guessed wrong (it was a mine).");
            Console.WriteLine($"Navigate up and down on the grid using {ConsoleKey.UpArrow} & {ConsoleKey.DownArrow}.");
            Console.WriteLine($"Navigate left and right on the grid using {ConsoleKey.LeftArrow} & {ConsoleKey.RightArrow}.");
            Console.WriteLine("Key:");
            Console.WriteLine("\t'?' is a hidden tile");
            Console.WriteLine("\t'!' is a flagged tile");
            Console.WriteLine("\t'*' is a mine");
            Console.WriteLine("\t'2' etc. amount of surrounding mines");
            #endregion

            Stopwatch stopwatch = Stopwatch.StartNew();

            ConsoleKey key = ConsoleKey.Escape;
            while (key != ConsoleKey.Q && !GameOver() && !clickedMine)
            {
                switch (key)
                {
                    #region Navigation
                    case ConsoleKey.UpArrow when yCoord > 0:
                        yCoord--;
                        break;
                    case ConsoleKey.DownArrow when yCoord < grid.Length - 1:
                        yCoord++;

                        break;
                    case ConsoleKey.LeftArrow when xCoord > 0:
                        xCoord--;
                        break;
                    case ConsoleKey.RightArrow when xCoord < grid[0].Length - 1:
                        xCoord++;
                        break;
                    #endregion

                    #region Interaction
                    // Dig
                    case ConsoleKey.Enter:
                        if (firstClick)
                        {
                            firstClick = false;

                            // Randomly reveal cells until a 0-tile is revealed
                            byte randomX, randomY;
                            do
                            {
                                randomX = (byte)random.Next(width);
                                randomY = (byte)random.Next(height);
                                RevealAdjacentTiles(randomX, randomY);
                            } while (!grid[randomY][randomX].IsHidden);
                        }

                        grid[yCoord][xCoord].IsHidden = false;
                        if (grid[yCoord][xCoord].IsMine)
                        {
                            clickedMine = true;
                        }
                        break;

                    // Toggle Flag
                    case ConsoleKey.Spacebar:
                        if (grid[yCoord][xCoord].IsFlagged)
                        {
                            flaggedPotentialMines--;
                        }
                        if (!grid[yCoord][xCoord].IsFlagged)
                        {
                            flaggedPotentialMines++;
                        }
                        grid[yCoord][xCoord].IsFlagged = !grid[yCoord][xCoord].IsFlagged;
                        break;
                        #endregion
                }
                hiddenTiles = (ushort)GetNumberOfHiddenTiles();
                chanceThatHiddenTileIsMine = (double)potentialRemainingMines / hiddenTiles;

                Console.WriteLine($"Possible Mines Found: {flaggedPotentialMines}");
                Console.WriteLine($"Possible Mines Left: {potentialRemainingMines}");
                Console.WriteLine($"Hidden Tiles: {hiddenTiles}");
                Console.WriteLine($"Chances that tile is mine: {chanceThatHiddenTileIsMine * 100}%");
                PrintArray();
                key = Console.ReadKey().Key;
                Console.Clear();
            }
            stopwatch.Stop();

            if (clickedMine)
            {
                Console.WriteLine($"You clicked a mine at ({xCoord}, {yCoord}).");
            }
            else
            {
                Console.WriteLine("You won!");
            }
            Console.WriteLine($"The Game lasted {stopwatch.ElapsedMilliseconds / 1000.0} seconds.");
        }

        static byte GetSurroundingMines(byte x, byte y)
        {
            // Define relative coordinates for neighboring cells
            sbyte[] dx = [-1, -1, -1, 0, 0, 1, 1, 1];
            sbyte[] dy = [-1, 0, 1, -1, 1, -1, 0, 1];

            byte mineCount = 0;
            if (!grid[y][x].IsMine)
            {
                // Count adjacent mines
                for (byte i = 0; i < 8; i++)
                {
                    byte newX = (byte)(x + dx[i]);
                    byte newY = (byte)(y + dy[i]);

                    if (newY >= 0 && newY < grid.Length && newX >= 0 && newX < grid[0].Length && grid[newY][newX].IsMine)
                    {
                        mineCount++;
                    }
                }
            }
            return mineCount;
        }

        static void PrintArray()
        {
            for (int y = 0; y < grid.Length; y++)
            {
                for (int x = 0; x < grid[y].Length; x++)
                {
                    if (!grid[y][x].IsHidden)
                    {
                        Console.ForegroundColor = grid[y][x].Color;
                    }

                    if (y == yCoord && x == xCoord)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                    }

                    Console.Write(grid[y][x]);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        public static bool GameOver()
        {
            // Check if all tiles are revealed (not hidden)
            bool allRevealed = !grid.Any(row => row.Any(tile => tile.IsHidden));

            // Check if all hidden tiles are flagged
            bool allHiddenFlagged = grid.All(row => row.All(tile => !tile.IsHidden || tile.IsFlagged));

            // Game is over if either condition is true
            return allRevealed || allHiddenFlagged;
        }

        public static int GetNumberOfHiddenTiles() => grid.SelectMany(row => row).Count(tile => tile.IsHidden);

        public static void RevealAdjacentTiles(byte x, byte y)
        {
            // Check if the coordinates are within the grid and the tile is hidden
            if (!(y >= 0 && y < height && x >= 0 && x < width) || !grid[y][x].IsHidden)
            {
                return;
            }

            // Reveal the tile
            grid[y][x].IsHidden = false;

            // If the tile is not a mine and has no surrounding mines, reveal its adjacent tiles
            if (!grid[y][x].IsMine && grid[y][x].SurroundingMines == 0)
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
    }
}
