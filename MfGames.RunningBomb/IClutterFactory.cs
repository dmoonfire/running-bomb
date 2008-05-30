namespace MfGames.RunningBomb
{
	/// <summary>
	/// Populates a junction with clutter. This is called on the child
	/// segments  And not on a central one.
	/// </summary>
	public interface IClutterFactory
	{
		/// <summary>
		/// Populates a junction with clutter, bonuses, and other
		/// stuff.
		/// </summary>
		void Create(Segment segment);
	}
}
