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
		public const float MinimumConnectionDistance = 4000;

		/// <summary>
		/// Maximum distance between junctions, in meters.
		/// </summary>
		public const float MaximumConnectionDistance = 8000;

		/// <summary>
		/// This is the distance, in meters, from a child junction to
		/// test for overlap.
		/// </summary>
		public const float OverlapConnectionDistance =
			MinimumConnectionDistance / 2;

		/// <summary>
		/// The distance from a child junction to perform the actual
		/// switching of junctions.
		/// </summary>
		public const float JunctionSwitchDistance =
			OverlapConnectionDistance * 0.75f;

		public const float PI = (float) Math.PI;
		public const float PI2 = (float) Math.PI * 2f;

		public const float RadiansToDegrees = 57.2957795f;

		/// <summary>
		/// Determines how fast a fractal breaks down, the closer to 0
		/// the straighter the line.
		/// </summary>
		/// <remarks>
		/// The smaller this number, the straighter the lines and
		/// faster it renders.
		/// </remarks>
		public const float FractalDecay = 0.2f;

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
		public const int SegmentAverageWidth = 150;

		/// <summary>
		/// The average width, in meters, of a junction node.
		/// </summary>
		public const int JunctionAverageWidth = 400;

		#region World Settings
		/// <summary>
		/// Sets the starting population to one trillion and one
		/// people.
		/// </summary>
		public const long StartingPopulation = 1000000000001;

		/// <summary>
		/// The safe distance is one million meters.
		/// </summary>
		public const double BombSafeDistance = 1000000 / 10;

		/// <summary>
		/// The rate of poulation saving.
		/// </summary>
		public const double PopulationSavingRate = 2.5;

		/// <summary>
		/// The distance where the player is safe is double the bomb
		/// safe distance.
		/// </summary>
		public const double PlayerSafeDistance = 2 * BombSafeDistance;

		/// <summary>
		/// Determines the maximum height or width a physics block can
		/// be before it is processed as a junction.
		/// </summary>
		public const float MaximumJunctionPhysicsBlock = 200;

		/// <summary>
		/// The number of seconds left in the countdown.
		/// </summary>
		public const float StartingCountdown = 300;
		#endregion
	}
}
