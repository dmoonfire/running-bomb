using C5;
using MfGames.Utility;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// A simplistic segment factory that creates a single ragged line
	/// between two junctions.
	/// </summary>
	public class SimpleSegmentFactory
	: ISegmentFactory
	{
		/// <summary>
		/// Creates a segment from a child junction to its parent.
		/// </summary>
		public Segment Create(JunctionNode childNode)
		{
			// Sanity checking
			if (childNode.ParentJunctionNode == null)
				throw new Exception("Cannot generate with a null parent");

			// Get the distance between the two points
			Segment segment = new Segment();
			JunctionNode parentNode = childNode.ParentJunctionNode;

			float distance = (float) Math.Sqrt(
				Math.Pow(parentNode.Point.X - childNode.Point.X, 2) +
				Math.Pow(parentNode.Point.Y - childNode.Point.Y, 2));

			// Figure out how many nodes we should have. Roughly, we
			// want about one every 100 meters. We also want this to
			// be a power of two.
			int nodeCount = Functions.NextPowerOfTwo((int) (distance / 100.0));

			// Add the two end points
			LinkedList<PointF> points = new LinkedList<PointF>();
			LinkedList<PointF> newPoints = new LinkedList<PointF>();
			points.Add(parentNode.Point);
			points.Add(childNode.Point);

			// Split apart the line
			int recursion = 3;

			while (nodeCount > 0)
			{
				// Loop through the points
				for (int i = 0; i < points.Count -1; i++)
				{
					// Pull the two points
					PointF p1 = points[i];
					PointF p2 = points[i + 1];

					// Create a point between these two
					newPoints.Add(p1);
					newPoints.Add(SegmentPoints(p1, p2,
							parentNode.Random,
							distance, recursion));
					newPoints.Add(p2);
				}

				// Copy the points
				points.Clear();
				points.AddAll(newPoints);
				newPoints.Clear();

				// Since we double the number of lines, just halve
				// the node count.
				nodeCount /= 2;
				recursion++;
			}

			// Add the points to the segment
			segment.CenterPoints.AddAll(points);

			// Return the results
			return segment;
		}

		/// <summary>
		/// Creates a mid-point between the two points and modifies to
		/// give it a jagged appearance.
		/// </summary>
		private PointF SegmentPoints(
			PointF p1, PointF p2,
			MersenneRandom random,
			float distance, int recursion)
		{
			// Get the midpoint
			PointF mp = new PointF(
				(p1.X + p2.X) / 2,
				(p1.Y + p2.Y) / 2);

			// Change this point by a small, random amount
			float maxDiff = distance / (float) (recursion * recursion);
			float diff = random.NextSingle(-maxDiff, maxDiff);

			// Calculate a perpendicular line
			float dy = p2.Y - p1.Y;
			float dx = p2.X - p1.X;

			if (dy == 0)
			{
				// This is a horizontal line
				//mp.X += diff;
			}
			else if (dx == 0)
			{
				// This is a vertical line
				//mp.Y += diff;
			}
			else
			{
				// Figure out the slope of the line
				double theta1 = Math.Tanh(dy / dx);
				double theta2 = theta1 - Math.PI / 2;

				mp = new PointF(
					(float) (mp.X + diff * Math.Cos(theta2)),
					(float) (mp.Y + diff * Math.Sin(theta2)));
			}

			// Return the results
			return mp;
		}

		#region Logging
		private Log log;

		/// <summary>
		/// Contains the logging interface which is lazily-loaded.
		/// </summary>
		public Log Log
		{
			get
			{
				if (log == null)
					log = new Log(GetType());

				return log;
			}
		}
		#endregion
	}
}
