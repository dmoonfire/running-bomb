using C5;
using Gpc;
using MfGames.Utility;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// General 2D functions geometry, used from various other
	/// factories.
	/// </summary>
	public static class Geometry
	{
		#region Points
		/// <summary>
		/// Calculates the distance between two points.
		/// </summary>
		public static float CalculateDistance(PointF p1, PointF p2)
		{
			return (float) Math.Sqrt(
				Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
		}
		#endregion

		#region Shapes Generation
		/// <summary>
		/// Creates a standard circle with some point that
		/// approximates a circle.
		/// </summary>
		public static IPoly CreateCircle(
			PointF point,
			float radius)
		{
			// Create the polygon
			PolyDefault poly = new PolyDefault();

			// Create the points by just going in a circle
			int points = 32;
			double angle = 0;

			for (int i = 0; i < points; i++)
			{
				poly.Add(new PointF(
						point.X + (float) Math.Cos(angle) * radius,
						point.Y + (float) Math.Sin(angle) * radius));
				angle += Constants.PI2 / points;
			}

			// Return the results
			return poly;
		}

		/// <summary>
		/// Constructs a rectangle shape for GPC from the given
		/// bounds.
		/// </summary>
		public static IPoly CreateRectangle(RectangleF bounds)
		{
			IPoly rectangle = new PolyDefault();
			rectangle.Add(bounds.Left, bounds.Top);
			rectangle.Add(bounds.Right, bounds.Top);
			rectangle.Add(bounds.Right, bounds.Bottom);
			rectangle.Add(bounds.Left, bounds.Bottom);
			return rectangle;
		}

		/// <summary>
		/// Creates a polygon-based shape around a single point of a
		/// random size. The radiusMultiplier is used to increase the
		/// size to ensure that it connects to another shape (if that
		/// is the requirement).
		/// </summary>
		public static IPoly CreateShape(
			Junction parent,
			PointF point,
			float radius)
		{
			// Figure out the size of the square, as a radius. On each
			// retry, we make it a bit bigger.
			radius *= parent.Random.NextSingle(0.5f, 1.5f);
			
			// Get a random angle
			int count = parent.Random.Next(3, 7);
			float angle = parent.Random.NextSingle(0, Constants.PI2);

			// Create the square we are mapping
			PolyDefault poly = new PolyDefault();

			for (int i = 0; i < count; i++)
			{
				poly.Add(new PointF(
						point.X + (float) Math.Cos(angle) * radius,
						point.Y + (float) Math.Sin(angle) * radius));
				angle += Constants.PI2 / count;
			}

			// Return the results
			return poly;
		}
		#endregion

		#region Fractals
		/// <summary>
		/// Cycles through a list of points and creates a fractal
		/// version of the points by splitting each one in half and
		/// moving it a random amount.
		/// </summary>
		/// <returns>A true if at least one point was changed.</returns>
		public static bool StaggerPoints(
			CenterPointList points,
			MersenneRandom random,
			float minimumDistance)
		{
			// Ignores blanks and null
			if (points == null)
				throw new ArgumentException("points cannot be null");

			if (random == null)
				throw new ArgumentException("random cannot be null");

			if (points.Count < 2)
				return false;

			// Go through each set of points
			CenterPointList newPoints = new CenterPointList();
			bool changed = false;

			for (int i = 0; i < points.Count - 1; i++)
			{
				// Get the points
				CenterPoint p1 = points[i];
				CenterPoint p2 = points[i + 1];

				// Add the first point
				newPoints.Add(p1);

				// Get the distance
				float distance = CalculateDistance(p1.Point, p2.Point);

				if (distance > minimumDistance)
				{
					// These two are far enough to calculate a new point
					CenterPoint mp =
						new CenterPoint((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
					float delta = distance * Constants.FractalDecay;
					float diff = random.NextSingle(-delta, delta);

					// Calculate the new point
					float dy = p2.Y - p1.Y;
					float dx = p2.X - p1.X;
					
					if (dy == 0)
					{
						// This is a horizontal line
						mp.Y += diff;
					}
					else if (dx == 0)
					{
						// This is a vertical line
						mp.X += diff;
					}
					else
					{
						// Figure out the slope of the line
						double theta1 = Math.Tanh(dy / dx);
						double theta2 = theta1 - Math.PI / 2;
						
						mp.X = (float) (mp.X + diff * Math.Cos(theta2));
						mp.Y = (float) (mp.Y + diff * Math.Sin(theta2));
					}

					// Add the created point
					newPoints.Add(mp);
					changed = true;
				}

				// Add the second point
				newPoints.Add(p2);
			}

			// See if we changes something
			if (changed)
			{
				// Swap the points
				points.Clear();
				points.AddAll(newPoints);
				newPoints.Clear();
			}

			// Return our status
			return changed;
		}

		/// <summary>
		/// Loops through the points and staggers them repeatedly.
		/// </summary>
		public static void StaggerPoints(
			CenterPointList points,
			MersenneRandom random,
			float minimumDistance,
			int passes)
		{
			// Go through the minimum number of passes
			for (int i = 0; i < passes; i++)
			{
				if (!StaggerPoints(points, random, minimumDistance))
					break;
			}
		}
		#endregion
	}
}
