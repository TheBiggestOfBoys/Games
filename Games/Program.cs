using System;

namespace Wordle
{
    internal class Program
    {
        static void Main()
        {
            string word = "house";
            char[][] grid = new char[5][];

            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = new char[5];
            }

            Console.WriteLine("Try to guess the 5-letter word.");

            for (int x = 0; x < 5; x++)
            {
                string input = Console.ReadLine();
                if (input.Length == word.Length)
                {
                    grid[x] = input.ToCharArray();

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

                    if (AreArraysEqual(grid[x], word.ToCharArray()))
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

        static bool AreArraysEqual(char[] arr1, char[] arr2)
        {
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
