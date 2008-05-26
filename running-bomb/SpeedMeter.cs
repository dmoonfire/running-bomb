using MfGames.Sprite3;
using System;

namespace RunningBomb
{
	/// <summary>
	/// A simple guage which attempts to keep track of how many ticks or
	/// activations per second. This is used to calculate the FPS of a
	/// program.
	/// </summary>
	public class SpeedMeter
	{
		#region Constructors
		public SpeedMeter()
		{
			buckets = new double [span];

			for (int i = 0; i < span; i++)
				buckets[i] = 0;
		}

		public SpeedMeter(int newSpan)
		{
			span = newSpan;
			buckets = new double [span];

			for (int i = 0; i < span; i++)
				buckets[i] = 0;
		}
		#endregion

		#region Properties
		private int span = 20;
		private double [] buckets = null;
		private double total;
		private double seconds;
		#endregion

		#region Updating
		/// <summary>
		/// Updates the speed meter by a given amount.
		/// </summary>
		public void Update(UpdateArgs args, double amount)
		{
			// Increment the counter
			seconds += args.SecondsSinceLastUpdate;

			while (seconds >= 1)
			{
				// Remove the last bucket from the total
				total -= buckets[span - 1];

				// Move the buckets over
				for (int i = 1; i < span; i++)
				{
					buckets[i] = buckets[i - 1];
				}
				
				// Reset the last one
				buckets[0] = 0;

				// Decrement the counter
				seconds -= 1;
			}
	
			// Add to the bucket to the current bucket
			buckets[0] += amount;
			total += amount;
		}
		#endregion

		#region Results
		/// <summary>
		/// Retrieves an average amount from all the buckets.
		/// </summary>
		public double Average
		{
			get
			{
				//return total / span;

				double total = 0;

				foreach (double d in buckets)
					total += d;

				return total / span;
			}
		}
		#endregion
	}
}
