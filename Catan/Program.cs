using System;
using System.Text;

namespace Catan
{
	internal class Program
	{
		static void Main()
		{
			Console.OutputEncoding = Encoding.UTF8;
			Console.WriteLine("Welcome to Settlers of Catan!");
			Console.WriteLine("This is a text-based version of the popular board game.");
			Console.WriteLine();

			int numPlayers;
			do { Console.Write("Enter the number of players: "); }
			while (!int.TryParse(Console.ReadLine(), out numPlayers) && numPlayers > 1 && numPlayers < 5);

			Catan catan = new(numPlayers);
			catan.Play();
		}
	}
}
