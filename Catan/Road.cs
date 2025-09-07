namespace Catan
{
	public class Road(Player owner)
	{
		public readonly Player Owner = owner;

		override public string ToString() => "🛣️";
	}
}