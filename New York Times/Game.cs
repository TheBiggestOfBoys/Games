using System;
using System.IO;

namespace New_York_Times
{
	public abstract class Game
	{
		readonly public string filePath;

		public Game()
		{
			filePath = Path.Combine("Game States", GetType().Name + ".txt");
		}

		public void Play()
		{
			Main();
		}

		public virtual void Main()
		{
			throw new NotImplementedException();
		}

		public virtual string ShareableResults()
		{
			throw new NotImplementedException();
		}
	}
}
