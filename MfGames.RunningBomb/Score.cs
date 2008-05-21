using System;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// This object manages the score and all the score-related
	/// elements of the game.
	/// </summary>
	public class Score
	{
		#region Properties
		private double meters;
		private long killed;

		/// <summary>
		/// Contains the distance in meters that the shape has
		/// traveled.
		/// </summary>
		public double Meters
		{
			get { return meters; }
			set
			{
				if (value < 0)
					throw new Exception("Cannot set a negative meters");

				meters = value;
			}
		}

		/// <summary>
		/// Sets the number of people killed by actions of the player.
		/// </summary>
		public long PopulationKilled
		{
			get { return killed; }
			set
			{
				if (killed < 0)
					throw new Exception("Cannot set a negative kill");

				killed = value;
			}
		}

		/// <summary>
		/// Returns the number of the population saved by distance
		/// minus those killed.
		/// </summary>
		public long PopulationSaved
		{
			get
			{
				// If we are past the player safe distance, we only
				// have those killed by the player.
				if (Meters >= Constants.PlayerSafeDistance)
					return Constants.StartingPopulation - PopulationKilled - 1;

				// If we are past the bomb safe distance, we have the
				// player and anyone they killed
				if (Meters >= Constants.BombSafeDistance)
					return Constants.StartingPopulation - PopulationKilled;
				
				// Otherwise, it is a formula based on distance
				return (long) Math.Min(0,
					Constants.StartingPopulation
					- Math.Pow(Meters, 2)
					- PopulationKilled);
			}
		}

		/// <summary>
		/// Calculates the score of the player.
		/// </summary>
		public int PlayerScore
		{
			get
			{
				double score = Math.Round(1000.0
					* Math.Log(PopulationSaved, 10)
					* Math.Log(Meters, 10)
					* BadgeTotal, 0);
				return (int) score;
			}
		}
		#endregion

		#region Badges
		/// <summary>
		/// Returns the total value of all the badges the player
		/// earned.
		/// </summary>
		public double BadgeTotal
		{
			get
			{
				return 0;
			}
		}
		#endregion
	}
}
