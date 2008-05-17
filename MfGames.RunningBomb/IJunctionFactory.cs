namespace MfGames.RunningBomb
{
	/// <summary>
	/// Marks the common interfaces of a junction factory, which is
	/// used to build up the shape of a junction.
	/// </summary>
	public interface IJunctionFactory
	{
		/// <summary>
		/// Constructs the junction node's shape and sets internal
		/// structures to match the new shape.
		/// </summary>
		void Create(JunctionNode junction);
	}
}
