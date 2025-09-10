using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace New_York_Times
{
	internal class SpellingBee : Game
	{
		readonly char centerLetter;
		char[] otherLetters = new char[6];

		List<string> foundWords = [];

		int points = 0;

		public SpellingBee() : base()
		{
			string[] lines = File.ReadAllLines(filePath);

			centerLetter = lines[0][0];

			for (int i = 2; i < lines.Length; i++)
			{
				otherLetters[i - 2] = lines[i][0];
			}
		}

		public bool IsValidWord(string word)
		{
			if (word.Length < 4) return false;
			if (!word.Contains(centerLetter)) return false;
			foreach (char c in word)
			{
				if (c != centerLetter && !otherLetters.Contains(c))
				{
					return false;
				}
			}
			return true;
		}

		public override void Main()
		{
			ConsoleKey key = ConsoleKey.None;

			while (key != ConsoleKey.Escape)
			{
				if (key == ConsoleKey.Delete)
				{
					otherLetters = ShuffleOuterLetters(otherLetters);
				}

				string guessedWord = Console.ReadLine() ?? string.Empty;

				if (IsValidWord(guessedWord))
				{
					foundWords.Add(guessedWord);
					points += guessedWord.Length;
				}

				DisplayGame();

				Console.WriteLine($"Press {ConsoleKey.Escape} to exit or {ConsoleKey.Delete} to shuffle the letters.");
				key = Console.ReadKey(intercept: true).Key;
			}
		}

		public void DisplayLetters()
		{
			// Assumes otherLetters.Length == 6
			// Layout:
			//    0   1
			//  5   C   2
			//    4   3

			// Top row
			Console.WriteLine($"  {otherLetters[0]}  {otherLetters[1]}");

			// Middle row
			Console.Write($"{otherLetters[5]}  ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(centerLetter);
			Console.ResetColor();
			Console.WriteLine($"  {otherLetters[2]}");

			// Bottom row
			Console.WriteLine($"  {otherLetters[4]}  {otherLetters[3]}");
		}

		public void DisplayGame()
		{
			Console.Clear();
			Console.WriteLine($"Points: {points}");
			Console.WriteLine("Found Words:");
			foreach (string word in foundWords)
			{
				Console.WriteLine(word);
			}
			Console.WriteLine();
			DisplayLetters();
		}

		public static char[] ShuffleOuterLetters(char[] characters)
		{
			Random random = new();
			random.Shuffle(characters);
			return characters;
		}
	}
}
