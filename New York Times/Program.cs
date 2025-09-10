using System;
using System.Collections.Generic;

namespace New_York_Times
{
	internal class Program
	{
		static void Main()
		{
			List<(string Name, Game GameInstance)> games =
			[
				("Wordle", new Wordle()),
				("Connections", new Connections()),
				("SpellingBee", new SpellingBee()),
				("Pips", new Pips()),
				("Strands", new Strands()),
				("Tiles", new Tiles()),
				("Crossword", new Crossword()),
				("LetterBoxed", new LetterBoxed()),
			];

			while (true)
			{
				Console.Clear();
				Console.WriteLine("Choose a game (press the corresponding number key):");
				for (int i = 0; i < games.Count; i++)
				{
					Console.WriteLine($"{i + 1}. {games[i].Name}");
				}
				Console.WriteLine("0. Exit");

				ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

				int index = -1;
				switch (keyInfo.Key)
				{
					case ConsoleKey.D0:
					case ConsoleKey.NumPad0:
						index = 0;
						break;
					case ConsoleKey.D1:
					case ConsoleKey.NumPad1:
						index = 1;
						break;
					case ConsoleKey.D2:
					case ConsoleKey.NumPad2:
						index = 2;
						break;
					case ConsoleKey.D3:
					case ConsoleKey.NumPad3:
						index = 3;
						break;
					case ConsoleKey.D4:
					case ConsoleKey.NumPad4:
						index = 4;
						break;
					case ConsoleKey.D5:
					case ConsoleKey.NumPad5:
						index = 5;
						break;
					case ConsoleKey.D6:
					case ConsoleKey.NumPad6:
						index = 6;
						break;
					case ConsoleKey.D7:
					case ConsoleKey.NumPad7:
						index = 7;
						break;
					case ConsoleKey.D8:
					case ConsoleKey.NumPad8:
						index = 8;
						break;
					case ConsoleKey.D9:
					case ConsoleKey.NumPad9:
						index = 9;
						break;
				}

				if (index == 0)
				{
					Console.WriteLine("\nGoodbye!");
					break;
				}
				else if (index > 0 && index <= games.Count)
				{
					Console.Clear();
					try
					{
						games[index - 1].GameInstance.Play();
					}
					catch (NotImplementedException)
					{
						Console.WriteLine($"{games[index - 1].Name} is not implemented yet.");
					}
					Console.WriteLine("\nPress any key to return to the menu...");
					Console.ReadKey(true);
				}
				else
				{
					Console.WriteLine("\nInvalid choice. Press any key to try again...");
					Console.ReadKey(true);
				}
			}
		}
	}
}
