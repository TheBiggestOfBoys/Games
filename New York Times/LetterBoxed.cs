using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace New_York_Times
{
	internal class LetterBoxed : Game
	{
		char[][] sides = new char[4][];

		List<string> foundWords = [];

		public LetterBoxed() : base()
        {
			string[] lines = File.ReadAllLines(filePath);

			// Each side is 3 lines, separated by a blank line
			sides[0] = [lines[0][0], lines[1][0], lines[2][0]];
			sides[1] = [lines[4][0], lines[5][0], lines[6][0]];
			sides[2] = [lines[8][0], lines[9][0], lines[10][0]];
			sides[3] = [lines[12][0], lines[13][0], lines[14][0]];
		}

		public void GuessWord()
		{
			string word = string.Empty;
			int lastSide = -1;

			char enteredChar = Console.ReadKey().KeyChar;

			// Initial selection
			for (int i = 0; i < sides.Length; i++)
			{
				if (sides[i].Contains(enteredChar))
				{
					word += enteredChar;
					Console.WriteLine(word);
					lastSide = i;
				}
				else
				{
					Console.Error.WriteLine("Letter in not on a side!");
					break;
				}
			}

			ConsoleKey key = ConsoleKey.None;
			// Next letter (cannot be on the same side)
			while (key != ConsoleKey.Enter)
			{
				enteredChar = Console.ReadKey().KeyChar;
				if (sides[lastSide].Contains(enteredChar))
				{
					Console.Error.WriteLine("Letter in on the same side as the last letter!");
					break;
				}
				else
				{
					word += enteredChar;
					Console.WriteLine(word);
				}
			}

			Console.WriteLine($"Guessed the word: {word}");
		}
	}
}
