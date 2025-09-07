using System;
using System.Diagnostics;

namespace LinkedIn
{
	internal abstract class Game
	{
		Stopwatch Timer = new();

		public void Play()
		{
			Timer.Start();
			Main();
			Timer.Stop();
			Console.WriteLine($"Completed in {Timer.Elapsed.TotalSeconds} seconds.");
		}

		public virtual void Main()
		{
			throw new NotImplementedException();
		}
	}
}
