using System;

namespace New_York_Times
{
	public abstract class Game
	{
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
