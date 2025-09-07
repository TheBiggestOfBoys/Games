using System;
using System.Collections.Generic;
using System.Linq;

namespace Catan
{
	/// <summary>
	/// Represents the Catan game.
	/// </summary>
	public class Catan(int playersCount)
	{
		private int PlayerIndex = 0;

		private Player[] Players = GeneratePlayers(playersCount);
		private Player CurrentPlayer => Players[PlayerIndex];

		private List<DevelopmentCard> DevelopmentCards = GenerateDevelopmentCards();

		private static readonly Random random = new();

		private static readonly ConsoleColor[] PlayerColors = [ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.White];

		#region Crafting Recipies
		public static readonly Resources[] RoadRecipe = [Resources.Brick, Resources.Wood];

		public static readonly Resources[] SettlementRecipe = [Resources.Brick, Resources.Wood, Resources.Wheat, Resources.Sheep];

		public static readonly Resources[] CityRecipe = [Resources.Wheat, Resources.Wheat, Resources.Stone, Resources.Stone, Resources.Stone];

		public static readonly Resources[] DevelopmentCardRecipe = [Resources.Sheep, Resources.Wheat, Resources.Stone];
		#endregion

		/// <summary>
		/// Gets a value indicating whether any player has won the game.
		/// </summary>
		public bool HasWinner => Players.Any(p => p.HasWon);

		private Hex[][] Board = GenerateBoard();

		/// <summary>
		/// Generates the game board with hex tiles.
		/// </summary>
		/// <returns>A 2D array of hex tiles.</returns>
		public static Hex[][] GenerateBoard()
		{
			List<int> rollValues = [2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12];
			List<Resources> resourceTypes = Enum.GetValues(typeof(Resources))
												.Cast<Resources>()
												.SelectMany(r => Enumerable.Repeat(r, 4))
												.ToList();

			Hex[][] tempBoard = [
				new Hex[3],
						new Hex[4],
						new Hex[5],
						new Hex[4],
						new Hex[3]
			];

			// Shuffle rollValues and resourceTypes
			rollValues = [.. rollValues.OrderBy(x => random.Next())];
			resourceTypes = [.. resourceTypes.OrderBy(x => random.Next())];

			int rollIndex = 0;
			int resourceIndex = 0;

			for (int row = 0; row < tempBoard.Length; row++)
			{
				for (int col = 0; col < tempBoard[row].Length; col++)
				{
					if (rollIndex < rollValues.Count && resourceIndex < resourceTypes.Count)
					{
						tempBoard[row][col] = new Hex(resourceTypes[resourceIndex], rollValues[rollIndex]);
						rollIndex++;
						resourceIndex++;
					}
					// Robber's desert tile
					else
					{
						tempBoard[row][col] = new Hex(null, 0)
						{
							HasRobber = true
						};
					}
				}
			}

			return tempBoard;
		}

		/// <summary>
		/// Generates a shuffled deck of development cards.
		/// </summary>
		/// <returns>A list of shuffled development cards.</returns>
		public static List<DevelopmentCard> GenerateDevelopmentCards()
		{
			List<DevelopmentCard> cards = new(25);

			Dictionary<DevelopmentCard.DevelopmentCardType, int> cardCounts = new()
					{
						{ DevelopmentCard.DevelopmentCardType.Knight, 14 },
						{ DevelopmentCard.DevelopmentCardType.VictoryPoint, 5 },
						{ DevelopmentCard.DevelopmentCardType.RoadBuilding, 2 },
						{ DevelopmentCard.DevelopmentCardType.Monopoly, 2 },
						{ DevelopmentCard.DevelopmentCardType.YearOfPlenty, 2 },
					};

			foreach (KeyValuePair<DevelopmentCard.DevelopmentCardType, int> cardCount in cardCounts)
			{
				for (int i = 0; i < cardCount.Value; i++)
				{
					cards.Add(new(cardCount.Key));
				}
			}

			// Shuffle the deck
			return [.. cards.OrderBy(x => random.Next())];
		}

		/// <summary>
		/// Generates players for the game.
		/// </summary>
		/// <param name="count">The number of players.</param>
		/// <returns>An array of players.</returns>
		public static Player[] GeneratePlayers(int count)
		{
			Player[] players = new Player[count];
			for (int i = 0; i < count; i++)
			{
				players[i] = new(PlayerColors[i]);
			}
			return players;
		}

		/// <summary>
		/// Handles the placement of starting settlements and roads for each player.
		/// </summary>
		public void PlaceStartingSettlements()
		{
			for (int round = 1; round <= 2; round++)
			{
				Console.WriteLine("Round #" + round);
				for (int i = 0; i < Players.Length; i++)
				{
					Console.WriteLine("Player #" + (i + 1) + " place your first settlement.");
					PlaceItem(typeof(Building));
					Console.WriteLine("Player #" + (i + 1) + " place your first road.");
					PlaceItem(typeof(Road));
				}
			}
		}

		/// <summary>
		/// Simulates a roll of 2-6-sided dice.
		/// The 2 random calls better represents the probability as opposed to using 1-12-sided die.
		/// </summary>
		/// <returns>A number between 1 & 12.</returns>
		public static int RollDice() => random.Next(1, 7) + random.Next(1, 7);

		/// <summary>
		/// Starts the game and handles the main game loop.
		/// </summary>
		public void Play()
		{
			DisplayGameBoard();
			PlaceStartingSettlements();

			while (!HasWinner)
			{
				Console.Clear();

				DisplayScoreboard();
				DisplayGameBoard();

				Player currentPlayer = CurrentPlayer;
				Console.Write("It is ");
				Console.ForegroundColor = currentPlayer.Color;
				Console.Write($"Player #{PlayerIndex + 1}");
				Console.ResetColor();
				Console.Write("'s turn.");
				Console.WriteLine();
				int roll = RollDice();
				Console.WriteLine($"Rolled a {roll}");

				foreach (Hex[] row in Board)
				{
					foreach (Hex hex in row)
					{
						if (hex.RollValue == roll && !hex.HasRobber && hex.Resource is not null)
						{
							foreach (Building building in hex.Vertices)
							{
								building?.Owner.ResourceCards.Add((Resources)hex.Resource);
							}
						}
					}
				}

				ConsoleKey key = ConsoleKey.Escape;
				while (key != ConsoleKey.Q)
				{
					// Player Build
					Console.WriteLine("What would you like to build?");
					Console.WriteLine("1) Road 🛣️");
					Console.WriteLine("2) Settlement 🏠");
					Console.WriteLine("3) City 🏙️");
					Console.WriteLine("4) Development Card 🎴");
					Console.WriteLine("'Q' End Turn");

					key = Console.ReadKey().Key;

					switch (key)
					{
						// Road
						case ConsoleKey.D1 or ConsoleKey.NumPad1:
							currentPlayer.Build(typeof(Road));
							break;
						// Settlement
						case ConsoleKey.D2 or ConsoleKey.NumPad2:
							currentPlayer.Build(typeof(Building));
							break;
						// City
						case ConsoleKey.D3 or ConsoleKey.NumPad3:
							currentPlayer.Build(typeof(Building), true);
							break;
						// Development Card
						case ConsoleKey.D4 or ConsoleKey.NumPad4:
							currentPlayer.Build(typeof(DevelopmentCard));
							break;
					}
				}

				// Next player, loops back to 0 if it exceeds the player count
				PlayerIndex = (PlayerIndex >= Players.Length) ? 0 : PlayerIndex += 1;
			}
		}

		/// <summary>
		/// Converts a resource to its string representation.
		/// </summary>
		/// <param name="resource">The resource to convert.</param>
		/// <returns>A string representation of the resource.</returns>
		public static string ResourceToString(Resources resource) => resource switch
		{
			Resources.Wood => "🌲",
			Resources.Brick => "🧱",
			Resources.Stone => "⛰️",
			Resources.Wheat => "🌾",
			Resources.Sheep => "🐑",
			_ => "❌"
		};

		/// <summary>
		/// Displays the scoreboard showing the current status of each player.
		/// </summary>
		public void DisplayScoreboard()
		{
			for (int i = 0; i < Players.Length; i++)
			{
				Player player = Players[i];
				Console.ForegroundColor = player.Color;
				Console.WriteLine("Player #" + (i + 1) + ':');
				Console.WriteLine(player);
				Console.ResetColor();
				Console.WriteLine();
			}
		}

		/// <summary>
		/// Handles the placement of an item (building or road) on the board.
		/// </summary>
		/// <param name="type">The type of item to place.</param>
		public void PlaceItem(Type type)
		{
			Console.WriteLine("Enter the row and column of the hex you want to place the item on.");

			int row;
			do { Console.Write("Enter the row: "); }
			while (!int.TryParse(Console.ReadLine(), out row) && row > 5 && row >= 0);

			int col;
			do { Console.Write("Enter the col: "); }
			while (!int.TryParse(Console.ReadLine(), out col) && col > Board[row].Length && col >= 0);

			int index;
			do { Console.Write("Enter the vertex/edge index to place the building on: "); }
			while (!int.TryParse(Console.ReadLine(), out index) && col > 6 && col >= 0);

			if (CanPlaceSettlement(ref Board[row][col], index))
			{
				if (type == typeof(Building))
				{
					Building building = new(CurrentPlayer);
					Board[row][col].Vertices[index] = building;
					CurrentPlayer.Buildings.Add(building);
				}
				else
				{
					Road road = new(CurrentPlayer);
					Board[row][col].Roads[index] = road;
					CurrentPlayer.Roads.Add(road);
				}
			}
			else
			{
				Console.WriteLine("Invalid placement.");
			}
		}

		/// <summary>
		/// Determines whether a settlement can be placed at the specified location.
		/// </summary>
		/// <param name="location">The hex location to check.</param>
		/// <param name="vertexIndex">The vertex index to check.</param>
		/// <returns><c>true</c> if a settlement can be placed; otherwise, <c>false</c>.</returns>
		public static bool CanPlaceSettlement(ref Hex location, int vertexIndex)
		{
			// If there is already a building at the location, return false
			if (location.Vertices[vertexIndex] is not null)
			{
				return false;
			}
			// Set the previous and next array indices so they stay withing 0 and 5
			int previousIndex = (vertexIndex - 1 + 6) % 6;
			int nextIndex = (vertexIndex + 1) % 6;

			// Check the adjacent vertices in the same hex
			if (location.Vertices[previousIndex] is not null || location.Vertices[nextIndex] is not null)
			{
				return false;
			}

			// Check the corresponding vertex in the adjacent hexagon
			Hex? adjacentHex = location.Edges[vertexIndex];
			int adjacentVertexIndex = (vertexIndex + 1) % 6;
			if (adjacentHex is not null && adjacentHex.Vertices[adjacentVertexIndex] is not null)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Displays the game board.
		/// </summary>
		public void DisplayGameBoard()
		{
			for (int row = 0; row < Board.Length; row++)
			{
				Console.Write(new string(' ', (5 - Board[row].Length) * 2));
				for (int col = 0; col < Board[row].Length; col++)
				{
					Console.Write(Board[row][col]);
					if (col != Board[row].Length - 1)
					{
						Console.Write('|');
					}
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		/// <summary>
		/// Moves the robber from one hex to another.
		/// </summary>
		/// <param name="from">The hex to move the robber from.</param>
		/// <param name="to">The hex to move the robber to.</param>
		public static void MoveRobber(Hex from, Hex to)
		{
			from.HasRobber = false;
			to.HasRobber = true;
		}
	}
}