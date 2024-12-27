using System;
using System.Collections.Generic;
using System.Linq;

namespace Catan
{
    /// <summary>
    /// Represents a player in the Catan game.
    /// </summary>
    public class Player
    {
        public List<Resources> ResourceCards = new();
        public List<Road> Roads = new();
        public List<Building> Buildings = new();
        public List<DevelopmentCard> DevelopmentCards = new();

        public readonly ConsoleColor Color;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class with the specified color.
        /// </summary>
        /// <param name="color">The color representing the player.</param>
        public Player(ConsoleColor color)
        {
            Color = color;
        }

        /// <summary>
        /// Gets the known victory points based on the player's buildings.
        /// </summary>
        public int KnownVictoryPoints => Buildings.Sum(b => b.IsCity ? 2 : 1);

        /// <summary>
        /// Gets the actual victory points including development cards.
        /// </summary>
        public int ActualVictoryPoints => KnownVictoryPoints + DevelopmentCards.Sum(d => d.VictoryPoints);

        /// <summary>
        /// Gets a value indicating whether the player has won the game.
        /// </summary>
        public bool HasWon => ActualVictoryPoints >= 10;

        /// <summary>
        /// Determines whether the player has the specified resources.
        /// </summary>
        /// <param name="resources">The resources to check.</param>
        /// <returns><c>true</c> if the player has the resources; otherwise, <c>false</c>.</returns>
        private bool HasResources(Resources[] resources)
        {
            Dictionary<Resources, int> resourceCounts = ResourceCards.GroupBy(r => r)
                                              .ToDictionary(g => g.Key, g => g.Count());

            foreach (Resources resource in resources)
            {
                if (!resourceCounts.TryGetValue(resource, out int value) || value == 0)
                {
                    return false;
                }
                resourceCounts[resource] = --value;
            }

            return true;
        }

        /// <summary>
        /// Removes the specified resources from the player's resource cards.
        /// </summary>
        /// <param name="resources">The resources to remove.</param>
        private void RemoveResources(Resources[] resources)
        {
            foreach (Resources resource in resources)
            {
                ResourceCards.Remove(resource);
            }
        }

        /// <summary>
        /// Builds the specified type of item.
        /// </summary>
        /// <param name="type">The type of item to build.</param>
        /// <param name="isCity">if set to <c>true</c>, the building is a city.</param>
        public void Build(Type type, bool isCity = false)
        {
            // Determine the recipe based on the type
            Resources[] recipe = type switch
            {
                Type t when t == typeof(Road) => Catan.RoadRecipe,
                Type t when t == typeof(Building) && !isCity => Catan.SettlementRecipe,
                Type t when t == typeof(Building) && isCity => Catan.CityRecipe,
                Type t when t == typeof(DevelopmentCard) => Catan.DevelopmentCardRecipe,
                _ => throw new ArgumentException("Invalid type."),
            };

            if (HasResources(recipe))
            {
                RemoveResources(recipe);

                // Add the crafted item based on the type
                switch (type)
                {
                    case Type t when t == typeof(Road):
                        Roads.Add(new(this));
                        break;
                    case Type t when t == typeof(Building):
                        Building tempBuilding = new(this);
                        if (isCity)
                        {
                            tempBuilding.Upgrade();
                        }
                        Buildings.Add(tempBuilding);
                        break;
                    case Type t when t == typeof(DevelopmentCard):
                        DevelopmentCards.Add(new(DevelopmentCard.DevelopmentCardType.Knight));
                        break;
                    default:
                        throw new ArgumentException("Invalid type.");
                }
            }
            else
            {
                Console.WriteLine("Not enough resources to build.");
            }
        }

        /// <summary>
        /// Returns a string that represents the current player.
        /// </summary>
        /// <returns>A string that represents the current player.</returns>
        public override string ToString() =>
            $"Victory Points: {KnownVictoryPoints}+?" + Environment.NewLine +
            $"Resource Cards: {ResourceCards.Count}" + Environment.NewLine +
            $"Roads: {Roads.Count}" + Environment.NewLine +
            $"Settlements: {Buildings.Count(a => !a.IsCity)}" + Environment.NewLine +
            $"Cities: {Buildings.Count(a => a.IsCity)}" + Environment.NewLine +
            $"Development Cards: {DevelopmentCards.Count}";
    }
}