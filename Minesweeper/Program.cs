using System;
using System.IO;

namespace Minesweeper
{
    internal class Program
    {
        static void Main()
        {
            byte height;
            do { Console.Write("Enter height: "); }
            while (!byte.TryParse(Console.ReadLine(), out height));

            byte width;
            do { Console.Write("Enter width: "); }
            while (!byte.TryParse(Console.ReadLine(), out width));

            ushort numberOfMines;
            do { Console.Write("Enter number of mines: "); }
            while (!ushort.TryParse(Console.ReadLine(), out numberOfMines));

            Board board = new(height, width, numberOfMines);

            Console.WriteLine(File.ReadAllText(Directory.GetCurrentDirectory() + '\\' + "Minesweeper Tutorial.txt"));
            Console.WriteLine($"Press {ConsoleKey.Enter} to start.");
            Console.ReadLine();

            board.Main();
        }
    }
}
