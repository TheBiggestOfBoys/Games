using System;
using System.Collections.Generic;

namespace LinkedIn
{
	internal class Tango : Game
	{
		/// <summary>
		/// <c>null</c> is empty, <c>true</c> is sun ☀️, <c>false</c> is moon 🌙.
		/// </summary>
		private bool?[,] Grid = new bool?[6, 6];

		private readonly bool?[,] SolutionGrid;

		private readonly Dictionary<(int, int), bool> PresetCells = [];
		private readonly Constraint[] Constraints = [];

		private static readonly Random _random = new();

		internal readonly struct Constraint(int[] firstCell, int[] lastCell, bool condition)
		{
			public readonly int[] FirstCell = firstCell;
			public readonly int[] LastCell = lastCell;
			/// <summary>
			/// <c>true</c> means equal, <c>false</c> means opposite.
			/// </summary>
			public readonly bool Condition = condition;
		}

		public Tango()
		{
			SolutionGrid = GenerateValidBoard();
			// Optionally, copy SolutionGrid to Grid to start with a filled board,
			// or leave Grid empty for the user to solve.
			// Here, we start with an empty board for solving:
			Grid = new bool?[6, 6];
		}

		public void SetCell(int row, int column, bool? value)
		{
			Grid[row, column] = value;
		}

		public void DisplayGrid()
		{
			for (int r = 0; r < Grid.GetLength(0); r++)
			{
				for (int c = 0; c < Grid.GetLength(1); c++)
				{
					if (Grid[r, c] == true)
						Console.Write("☀️ ");
					else if (Grid[r, c] == false)
						Console.Write("🌙 ");
					else
						Console.Write(". ");
				}
				Console.WriteLine();
			}
		}

		public bool CheckBoard()
		{
			// Check rows
			for (int r = 0; r < Grid.GetLength(0); r++)
			{
				int moonsCountRow = 0;
				int sunsCountRow = 0;
				int consecutiveSuns = 0;
				int consecutiveMoons = 0;

				for (int c = 0; c < Grid.GetLength(1); c++)
				{
					if (Grid[r, c] == true)
					{
						sunsCountRow++;
						consecutiveSuns++;
						consecutiveMoons = 0;
					}
					else if (Grid[r, c] == false)
					{
						moonsCountRow++;
						consecutiveMoons++;
						consecutiveSuns = 0;
					}
					else
					{
						consecutiveSuns = 0;
						consecutiveMoons = 0;
					}

					if (sunsCountRow > 3)
					{
						Console.Error.WriteLine($"Row {r} has too many suns ☀️.");
						return false;
					}

					if (moonsCountRow > 3)
					{
						Console.Error.WriteLine($"Row {r} has too many moons 🌙.");
						return false;
					}

					if (consecutiveSuns > 2)
					{
						Console.Error.WriteLine($"Row {r} has more than 2 consecutive suns ☀️.");
						return false;
					}

					if (consecutiveMoons > 2)
					{
						Console.Error.WriteLine($"Row {r} has more than 2 consecutive moons 🌙.");
						return false;
					}
				}
			}

			// Check cols
			for (int c = 0; c < Grid.GetLength(1); c++)
			{
				int moonsCountCol = 0;
				int sunsCountCol = 0;
				int consecutiveSuns = 0;
				int consecutiveMoons = 0;

				for (int r = 0; r < Grid.GetLength(0); r++)
				{
					if (Grid[r, c] == true)
					{
						sunsCountCol++;
						consecutiveSuns++;
						consecutiveMoons = 0;
					}
					else if (Grid[r, c] == false)
					{
						moonsCountCol++;
						consecutiveMoons++;
						consecutiveSuns = 0;
					}
					else
					{
						consecutiveSuns = 0;
						consecutiveMoons = 0;
					}

					if (sunsCountCol > 3)
					{
						Console.Error.WriteLine($"Col {c} has too many suns ☀️.");
						return false;
					}

					if (moonsCountCol > 3)
					{
						Console.Error.WriteLine($"Col {c} has too many moons 🌙.");
						return false;
					}

					if (consecutiveSuns > 2)
					{
						Console.Error.WriteLine($"Col {c} has more than 2 consecutive suns ☀️.");
						return false;
					}

					if (consecutiveMoons > 2)
					{
						Console.Error.WriteLine($"Col {c} has more than 2 consecutive moons 🌙.");
						return false;
					}
				}
			}

			return true;
		}

		private static bool?[,] GenerateValidBoard()
		{
			bool?[,] grid = new bool?[6, 6];
			if (Fill(grid, 0, 0))
				return grid;
			throw new Exception("Failed to generate a valid Tango board.");
		}

		private static bool Fill(bool?[,] grid, int row, int col)
		{
			if (row == 6) return true;
			int nextRow = (col == 5) ? row + 1 : row;
			int nextCol = (col + 1) % 6;

			bool[] vals = new[] { true, false };
			_random.Shuffle(vals);

			foreach (bool val in vals)
			{
				grid[row, col] = val;
				if (IsValid(grid, row, col) && Fill(grid, nextRow, nextCol))
					return true;
				grid[row, col] = null;
			}
			return false;
		}

		private static bool IsValid(bool?[,] grid, int row, int col)
		{
			// Row checks
			int sunsRow = 0, moonsRow = 0, consecutiveSuns = 0, consecutiveMoons = 0;
			for (int c = 0; c < 6; c++)
			{
				if (grid[row, c] == true)
				{
					sunsRow++;
					consecutiveSuns++;
					consecutiveMoons = 0;
				}
				else if (grid[row, c] == false)
				{
					moonsRow++;
					consecutiveMoons++;
					consecutiveSuns = 0;
				}
				else
				{
					consecutiveSuns = 0;
					consecutiveMoons = 0;
				}
				if (consecutiveSuns > 2 || consecutiveMoons > 2)
					return false;
			}
			if (sunsRow > 3 || moonsRow > 3)
				return false;

			// Column checks
			int sunsCol = 0, moonsCol = 0;
			consecutiveSuns = 0;
			consecutiveMoons = 0;
			for (int r = 0; r < 6; r++)
			{
				if (grid[r, col] == true)
				{
					sunsCol++;
					consecutiveSuns++;
					consecutiveMoons = 0;
				}
				else if (grid[r, col] == false)
				{
					moonsCol++;
					consecutiveMoons++;
					consecutiveSuns = 0;
				}
				else
				{
					consecutiveSuns = 0;
					consecutiveMoons = 0;
				}
				if (consecutiveSuns > 2 || consecutiveMoons > 2)
					return false;
			}
			if (sunsCol > 3 || moonsCol > 3)
				return false;

			return true;
		}
	}
}
