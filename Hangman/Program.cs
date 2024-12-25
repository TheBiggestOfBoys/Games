using System;
using System.Collections.Generic;
using System.Linq;

namespace Hangman
{
    internal class Program
    {
        public static string word = "hello";
        public static bool[] revealed = new bool[word.Length];
        public static List<char> guesses = [];
        public static byte guessCount = 0;

        static void Main()
        {
            // While all letters aren't revealed
            while (!revealed.All(b => b == true))
            {
                Console.WriteLine(guessCount + " Guesses: " + GuessesToString());
                PrintHangman();
                char guessedLetter = Console.ReadKey().KeyChar;
                if (!guesses.Contains(guessedLetter))
                {
                    guesses.Add(guessedLetter);
                    guessCount++;
                    CheckGuess(guessedLetter);
                }
                Console.Clear();
            }
            Console.WriteLine("Congratulations! You guessed the word, which was '" + word + "', in " + guessCount + " guesses.");
        }

        static void PrintHangman()
        {
            for (int x = 0; x < word.Length; x++)
            {
                if (revealed[x] == true)
                {
                    Console.Write(word[x]);
                }
                else
                {
                    Console.Write('_');
                }
                Console.Write(' ');
            }
            Console.WriteLine();
        }

        static void CheckGuess(char guess)
        {
            if (word.Contains(guess))
            {
                for (int i = 0; i < word.Length; i++)
                {
                    if (word[i] == guess)
                    {
                        revealed[i] = true;
                    }
                }
            }
        }

        public static string GuessesToString()
        {
            string tempString = string.Empty;
            foreach (char character in guesses)
            {
                tempString += character;
            }
            return tempString;
        }
    }
}
