namespace Catan
{
	/// <summary>
	/// Represents a hex tile in the Catan game.
	/// </summary>
	public class Hex
	{
		/// <summary>
		/// Gets the resource type of the hex tile.
		/// </summary>
		public readonly Resources? Resource;

		/// <summary>
		/// Gets the roll value of the hex tile.
		/// </summary>
		public readonly int RollValue;

		/// <summary>
		/// Gets or sets a value indicating whether the hex tile has a robber.
		/// </summary>
		public bool HasRobber;

		/// <summary>
		/// Initializes a new instance of the <see cref="Hex"/> class with the specified resource and roll value.
		/// </summary>
		/// <param name="resource">The resource type of the hex tile.</param>
		/// <param name="rollValue">The roll value of the hex tile.</param>
		public Hex(Resources? resource, int rollValue)
		{
			Resource = resource;
			RollValue = rollValue;
		}

		/// <summary>
		/// The 6 connected hexes. Null if there is no hex.
		/// <para></para>
		/// Index key (-1 for 0-based indexing):
		/// <list type="number">
		///     <item>Top Left</item>
		///     <item>Left</item>
		///     <item>Bottom Left</item>
		///     <item>Bottom Right</item>
		///     <item>Right</item>
		///     <item>Top Right</item>
		/// </list>
		/// </summary>
		public Hex?[] Edges = new Hex?[6];

		/// <summary>
		/// The 6 connected roads. Null if there is no road.
		/// <para></para>
		/// Index key (-1 for 0-based indexing):
		/// <list type="number">
		///     <item>Top Left</item>
		///     <item>Left</item>
		///     <item>Bottom Left</item>
		///     <item>Bottom Right</item>
		///     <item>Right</item>
		///     <item>Top Right</item>
		/// </list>
		/// </summary>
		public Road?[] Roads = new Road?[6];

		/// <summary>
		/// The 6 connected buildings. Null if there is no building.
		/// <para></para>
		/// Index key (-1 for 0-based indexing):
		/// <list type="number">
		///     <item>Top</item>
		///     <item>Top Right</item>
		///     <item>Bottom Right</item>
		///     <item>Bottom</item>
		///     <item>Bottom Left</item>
		///     <item>Top Left</item>
		/// </list>
		/// </summary>
		public Building?[] Vertices = new Building?[6];

		/// <summary>
		/// Returns a string that represents the current hex tile.
		/// </summary>
		/// <returns>A string that represents the current hex tile.</returns>
		public override string ToString() => (HasRobber ? "😈" : RollValue.ToString()) + (Resource is null ? "❌" : Catan.ResourceToString((Resources)Resource));
	}
}