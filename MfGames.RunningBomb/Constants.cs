using System;

namespace MfGames.RunningBomb
{
	public static class Constants
	{
		public const int BuildMinimumConnections = 2;
		public const int BuildMaximumConnections = 5;

		/// <summary>
		/// Minimum distance between junctions, in meters.
		/// </summary>
		public const float MinimumConnectionDistance = 2500;

		/// <summary>
		/// Maximum distance between junctions, in meters.
		/// </summary>
		public const float MaximumConnectionDistance = 7500;

		/// <summary>
		/// This is the distance, in meters, from a child junction to
		/// test for overlap.
		/// </summary>
		public const float OverlapConnectionDistance = 1500;

		public const float PI = (float) Math.PI;
		public const float PI2 = (float) Math.PI * 2f;

		/// <summary>
		/// Determines how fast a fractal breaks down, the closer to 0
		/// the straighter the line.
		/// </summary>
		public const float FractalDecay = 0.25f;

		/// <summary>
		/// The minimum number of meters to break a segment into.
		/// </summary>
		public const float MinimumSegmentDistance = 200;

		/// <summary>
		/// The number of passes to stagger the segment lines.
		/// </summary>
		public const int StaggerSegmentPasses = 8;

		/// <summary>
		/// The average width, in meters, of a segment.
		/// </summary>
		public const int SegmentAverageWidth = 100;

		/// <summary>
		/// The average width, in meters, of a junction node.
		/// </summary>
		public const int JunctionAverageWidth = 300;

		#region World Settings
		/// <summary>
		/// Sets the starting population to one trillion and one
		/// people.
		/// </summary>
		public const long StartingPopulation = 1000000000001;

		/// <summary>
		/// The safe distance is one million meters.
		/// </summary>
		public const double BombSafeDistance = 1000000;

		/// <summary>
		/// The distance where the player is safe is double the bomb
		/// safe distance.
		/// </summary>
		public const double PlayerSafeDistance = 2 * BombSafeDistance;
		#endregion
	}
}
