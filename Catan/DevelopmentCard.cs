using static Catan.DevelopmentCard;

namespace Catan
{
    public class DevelopmentCard(DevelopmentCardType cardType)
    {
        public readonly DevelopmentCardType CardType = cardType;
        public readonly int VictoryPoints = (cardType == DevelopmentCardType.VictoryPoint) ? 1 : 0;

        public override string ToString() => $"{CardType}: {VictoryPoints}";

        public enum DevelopmentCardType
        {
            Knight,
            VictoryPoint,
            RoadBuilding,
            YearOfPlenty,
            Monopoly
        }
    }
}