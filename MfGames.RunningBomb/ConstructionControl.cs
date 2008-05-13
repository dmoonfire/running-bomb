namespace MfGames.RunningBomb
{
	/// <summary>
	/// This class contains and manages the various construction
	/// controls and guidelines which is used for the dynamic
	/// generation. Every JunctionNode has a control which is then propagated
	/// through the created SegmentNode to help generate the various
	/// segments.
	/// </summary>
	public class ConstructionControl
	{
		#region Properties
		/// <summary>
		/// Indicates the amount of bubble tech (the game's primary
		/// theme) that this node has. Typically, this is reduced the
		/// further away from Bubble City but there may be temporary
		/// increases for outposts.
		/// </summary>
		public float BubbleTechInfluence;
		#endregion
	}
}
