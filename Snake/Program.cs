using System;

namespace Snake
{
	internal class Program
	{
		static void Main()
		{
			Random random = new();

			byte xPostion = 0;
			byte yPosition = 0;

			// Create Grid
			char[][] grid = new char[16][];

			for (byte y = 0; y < grid.Length; y++)
			{
				grid[y] = new char[16];
				for (byte x = 0; x < grid[0].Length; x++)
				{
					grid[y][x] = ' ';
				}
			}

			grid[random.Next(grid.Length)][random.Next(grid[0].Length)] = '*';
			ConsoleKey key = ConsoleKey.None;

			while (key != ConsoleKey.Q)
			{
				Console.ReadKey();
				grid[yPosition][xPostion] = ' ';
				switch (key)
				{
					case ConsoleKey.UpArrow when yPosition > 0:
						yPosition--;
						break;
					case ConsoleKey.DownArrow when yPosition < grid.Length - 1:
						yPosition++;
						break;
					case ConsoleKey.LeftArrow when xPostion > 0:
						xPostion--;
						break;
					case ConsoleKey.RightArrow when xPostion < grid[0].Length - 1:
						xPostion++;
						break;
					default:
						break;
				}
				grid[yPosition][xPostion] = '#';

				Console.Clear();
				foreach (char[] row in grid)
				{
					foreach (char cell in row)
					{
						Console.Write(cell);
					}
					Console.WriteLine();
				}
			}
		}
	}
}
