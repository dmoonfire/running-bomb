using MfGames.Sprite3;
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
		private float countdown;
		private float countdownMultiplier = 1;
		private float speed, maxSpeed;
		private float explosion;

		/// <summary>
		/// Contains the number of seconds left in the countdown.
		/// </summary>
		public float Countdown
		{
			get { return countdown; }
			set { countdown = value; }
		}

		public string CountdownString
		{
			get
			{
				// Pull out the minutes
				float time = countdown;
				int minutes = (int) (countdown / 60);
				time -= minutes * 60;

				// Format the seconds to 4 digits
				string seconds = time.ToString("F4");

				if (seconds.Length < 7)
					seconds = "0" + seconds;

				// Return the results
				return String.Format("{0}:{1}", minutes, seconds);
			}
		}

		/// <summary>
		/// Contains the modifier to the countdown speed.
		/// </summary>
		public float CountdownMultiplier
		{
			get { return countdownMultiplier; }
			set
			{
				if (value <= 0)
					return;

				countdownMultiplier = value;
			}
		}

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
		/// The point where the bomb actually explodes which is
		/// normally 0 to 10 seconds after the countdown.
		/// </summary>
		public float Explosion
		{
			get { return explosion; }
			set { explosion = value; }
		}

		/// <summary>
		/// Contains the current speed of the player. This also
		/// calculates the maximum speed.
		/// </summary>
		public float MaximumSpeed
		{
			get { return maxSpeed; }
		}

		/// <summary>
		/// Contains the formatted maximum speed.
		/// </summary>
		public string MaximumSpeedString
		{
			get
			{
				// Returns the maximum speed as a formatted
				// string of meters per second.
				return String.Format("{0:N1} m/s", MaximumSpeed);
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
		/// Contains the percentage saved, in terms of 0 to 1.
		/// </summary>
		public string PercentageSavedString
		{
			get
			{
				return String.Format("{0:N3} %", PercentageSaved * 100);
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

		/// <summary>
		/// Contains the current speed of the player. This also
		/// calculates the maximum speed.
		/// </summary>
		public float Speed
		{
			get { return speed; }
			set
			{
				maxSpeed = (float) Math.Max(speed, value);
				speed = value;
			}
		}

		/// <summary>
		/// Contains the formatted speed.
		/// </summary>
		public string SpeedString
		{
			get
			{
				// Returns the maximum speed as a formatted
				// string.
				return String.Format("{0:N1} k/s", speed);
			}
		}

		/// <summary>
		/// Contains a stress rating which is a number between 0 and 1
		/// where 1 is the highest stress the player can have.
		/// </summary>
		public double Stress
		{
			get
			{
				return Math.Min(1,
					1 - (countdown / Constants.StartingCountdown));
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
