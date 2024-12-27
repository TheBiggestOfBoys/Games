namespace Catan
{
    public class Building(Player owner)
    {
        public bool IsCity;
        public readonly Player Owner = owner;

        public void Upgrade() => IsCity = true;

        override public string ToString() => IsCity ? "🏠" : "🏙️";
    }
}