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
		private double distance;
		private long killed;

		/// <summary>
		/// Contains the distance in distance that the shape has
		/// traveled.
		/// </summary>
		public double Distance
		{
			get { return distance; }
			set
			{
				if (value < 0)
					throw new Exception("Cannot set a negative distance");

				distance = value;
			}
		}

		/// <summary>
		/// Contains the percentage saved, in terms of 0 to 1.
		/// </summary>
		public float PercentageSaved
		{
			get
			{
				return (float) PopulationSaved
					/ (float) Constants.StartingPopulation;
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
				// If we are zero, there is no one
				if (Distance <= 0)
					return 0;

				// If we are past the player safe distance, we only
				// have those killed by the player.
				if (Distance >= Constants.PlayerSafeDistance)
					return Constants.StartingPopulation - PopulationKilled;

				// If we are past the bomb safe distance, we have the
				// player and anyone they killed
				if (Distance >= Constants.BombSafeDistance)
					return Constants.StartingPopulation - PopulationKilled - 1;

				// Figure out our constants
				double safe = Constants.BombSafeDistance;
				double rate = Constants.PopulationSavingRate;
				double completed = Distance / safe;
				double t0 = Math.Tan(-rate / 2);
				double tX = -2 * t0;
				
				// Figure out the number of people saved
				double saved =
					(Math.Tan(rate * (completed - 0.5)) - t0) / tX;
				saved *= Constants.StartingPopulation;

				// Otherwise, it is a formula based on distance
				return (long) saved;
			}
		}

		/// <summary>
		/// Calculates the score of the player.
		/// </summary>
		public int PlayerScore
		{
			get
			{
				double score = 1000
					* Math.Log(PopulationSaved, 10)
					* Math.Log(Distance, 10)
					* BadgeTotal;
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
				return 1;
			}
		}
		#endregion
	}
}
