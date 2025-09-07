using System;

namespace LinkedIn
{
	internal class Program
	{
		static void Main()
		{
			Console.WriteLine("Select Game:");
			Console.WriteLine("1) Sudoku");
			Console.WriteLine("2) Zip");
			Console.WriteLine("3) Tango");
			Console.WriteLine("4) Queens");
			Console.WriteLine("5) PinPoint");
			Console.WriteLine("6) CrossClimb");

			ConsoleKey key = Console.ReadKey().Key;
			switch (key)
			{
				case ConsoleKey.D1:
				case ConsoleKey.NumPad1:
					Sudoku sudoku = new();
					sudoku.Play();
					break;
				case ConsoleKey.D2:
				case ConsoleKey.NumPad2:
					Zip zip = new();
					zip.Play();
					break;
				case ConsoleKey.D3:
				case ConsoleKey.NumPad3:
					Tango tango = new();
					tango.Play();
					break;
				case ConsoleKey.D4:
				case ConsoleKey.NumPad4:
					Queens queens = new();
					queens.Play();
					break;
				case ConsoleKey.D5:
				case ConsoleKey.NumPad5:
					PinPoint pinPoint = new();
					pinPoint.Play();
					break;
				case ConsoleKey.D6:
				case ConsoleKey.NumPad6:
					CrossClimb crossClimb = new("C:\\Users\\jrsco\\source\\repos\\Games\\LinkedIn\\Game States\\CrossClimb\\CrossClimbGame.txt");
					crossClimb.Play();
					break;
				default:
					Console.WriteLine("Invalid selection. Exiting.");
					break;
			}
		}
	}
}
