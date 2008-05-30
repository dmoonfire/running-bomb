using MfGames.Utility;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Centralized function for managing various factorys and
	/// construction controls.
	/// </summary>
	public static class FactoryManager
	{
		#region Junction Factories
		/// <summary>
		/// Chooses the random clutter factory which populates the
		/// contents of a junction.
		/// </summary>
		public static IClutterFactory ChooseClutterFactory(
			MersenneRandom random)
		{
			return new SimpleClutterFactory();
		}

		/// <summary>
		/// Selects a random junction factory. For a given seed, this
		/// will always return the same junction.
		/// </summary>
		public static IJunctionFactory ChooseJunctionFactory(
			MersenneRandom random)
		{
			return new SimpleJunctionFactory();
		}
		#endregion

		#region Segment Factories
		/// <summary>
		/// Selects a random segment factory. For a given seed, this
		/// will always return the same segment.
		/// </summary>
		public static ISegmentFactory ChooseSegmentFactory(
			Junction childJunction)
		{
			return new SimpleSegmentFactory();
		}
		#endregion
	}
}
