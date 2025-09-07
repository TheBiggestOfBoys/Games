using System;

namespace New_York_Times
{
	public class Wordle : Game
	{
		public void Play()
		{
			string word = "house";
			string[] grid = new string[5];

			for (int i = 0; i < grid.Length; i++)
			{
				grid[i] = new string('\0', 5);
			}

			Console.WriteLine("Try to guess the 5-letter word.");

			for (int x = 0; x < 5; x++)
			{
				string input = Console.ReadLine();
				if (input.Length == word.Length)
				{
					grid[x] = input;

					for (int i = 0; i < 5; i++)
					{
						if (grid[x][i] == word[i])
							Console.BackgroundColor = ConsoleColor.DarkGreen;
						else if (word.Contains(grid[x][i]))
							Console.BackgroundColor = ConsoleColor.DarkYellow;

						Console.Write(grid[x][i]);
						Console.BackgroundColor = ConsoleColor.Black;
					}
					Console.WriteLine();

					if (grid[x] == word)
					{
						Console.WriteLine($"You guessed the word, it was {word}.");
						return;
					}
				}
				else
				{
					Console.WriteLine("Please enter a 5-letter word.");
				}
			}
			Console.WriteLine($"You didn't guess the word, it was {word}.");
		}
	}
}