using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace New_York_Times
{
	internal class Connections : Game
	{
		private readonly Dictionary<string, string[]> Categories = new(4);

		public string[,] Board;

		public Connections()
		{
			LoadCategoriesFromFile();
		}

		private void LoadCategoriesFromFile()
		{
			string filePath = Path.Combine("Game States", "Connections", "Conections.txt");
			if (!File.Exists(filePath))
			{
				Console.Error.WriteLine($"Category file not found: {filePath}");
				return;
			}

			foreach (var line in File.ReadLines(filePath))
			{
				if (string.IsNullOrWhiteSpace(line)) continue;
				var parts = line.Split("--");
				if (parts.Length != 2) continue;

				string category = parts[0].Trim();
				string[] items = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				Categories.TryAdd(category, items);
			}
		}

		public override void Main()
		{
			// Prepare the board
			Board = GenerateBoard(Categories);

			// Track found categories
			HashSet<string> foundCategories = new();

			while (foundCategories.Count < Categories.Count)
			{
				Console.Clear();
				Console.WriteLine("Connections Game");
				Console.WriteLine("Find all categories by selecting 4 related items at a time.");
				Console.WriteLine();

				// Display the board
				List<string> remainingItems = new();
				for (int i = 0; i < Board.GetLength(0); i++)
				{
					for (int j = 0; j < Board.GetLength(1); j++)
					{
						string item = Board[i, j];
						// Only show items not already found
						if (!foundCategories.Any(cat => Categories[cat].Contains(item)))
						{
							remainingItems.Add(item);
						}
					}
				}

				if (remainingItems.Count == 0)
				{
					Console.WriteLine("Congratulations! You found all categories!");
					return;
				}

				Console.WriteLine("Available items:");
				Console.WriteLine(string.Join(", ", remainingItems));
				Console.WriteLine();

				Console.Write("Enter 4 items (comma-separated), or type 'exit' to quit: ");
				string? input = Console.ReadLine();
				if (input == null || input.Trim().ToLower() == "exit")
					break;

				string[] selections = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

				if (selections.Length != 4)
				{
					Console.WriteLine("Please enter exactly 4 items. Press any key to try again...");
					Console.ReadKey(true);
					continue;
				}

				// Check if all selections are in the remaining items
				if (!selections.All(item => remainingItems.Contains(item)))
				{
					Console.WriteLine("One or more selections are invalid or already found. Press any key to try again...");
					Console.ReadKey(true);
					continue;
				}

				// Check if the selection matches a category
				bool found = false;
				foreach (var kvp in Categories)
				{
					if (foundCategories.Contains(kvp.Key)) continue;
					if (kvp.Value.All(selections.Contains) && selections.All(kvp.Value.Contains))
					{
						Console.WriteLine($"Correct! You found the category: {kvp.Key}");
						foundCategories.Add(kvp.Key);
						found = true;
						break;
					}
				}

				if (!found)
				{
					Console.WriteLine("Incorrect group. Try again.");
				}

				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(true);
			}

			Console.WriteLine("Game over! Thanks for playing.");
		}

		public bool CheckSelections(string[] selections)
		{
			if (selections.Length == 4)
			{
				foreach (KeyValuePair<string, string[]> kvp in Categories)
				{
					int matches = 0;
					foreach (string item in selections)
					{
						if (kvp.Value.Contains(item))
						{
							matches++;
						}
					}
					if (matches == 4)
					{
						Console.WriteLine($"Found Category: {kvp.Key}");
						return true;
					}
				}
				return true;
			}
			else
			{
				Console.Error.WriteLine("Not enough selected!");
				return false;
			}
		}

		public string[,] GenerateBoard(Dictionary<string, string[]> categories)
		{
			string[,] board = new string[categories.Count, categories.Count];

			for (int x = 0; x < Categories.Keys.Count; x++)
			{
				for (int y = 0; y < Categories[Categories.Keys.ElementAt(x)].Length; y++)
				{
					board[x, y] = Categories[Categories.Keys.ElementAt(x)][y];
				}
			}

			return Shuffle(board);
		}

		public static string[,] Shuffle(string[,] board)
		{
			Random rng = new();
			for (int i = 0; i < board.GetLength(0); i++)
			{
				for (int j = 0; j < board.GetLength(1); j++)
				{
					int randomRow = rng.Next(board.GetLength(0));
					int randomCol = rng.Next(board.GetLength(1));
					string temp = board[i, j];
					board[i, j] = board[randomRow, randomCol];
					board[randomRow, randomCol] = temp;
				}
			}
			return board;
		}
	}
}
