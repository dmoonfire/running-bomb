using C5;
using Gpc;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Implements a simplistic junction which just consists of random
	/// radials in various directions which are then linked together
	/// into a polygon.
	/// </summary>
	public class RadialJunctionFactory
	: IJunctionFactory
	{
		/// <summary>
		/// Constructs the junction node's shape and sets internal
		/// structures to match the new shape.
		/// </summary>
		public void Create(Junction junction)
		{
			// Figure out the basic points by creating an array
			// radials, which is determined by the random number generator.
			int radialCount = junction.Random.Next(8, 32);
			LinkedList<PointF> internalPoints = new LinkedList<PointF>();

			// Populate the radial points
			for (int i = 0; i < radialCount; i++)
			{
				// Create the length from a random number in meters.
				float length = junction.Random.NextSingle(50, 250);

				// Figure out the angle we are calculating for the
				// internal shape, then use that to calculate the
				// internal point. These points are relative to (0,0).
				float angle = (float) Math.PI * 2 * i / radialCount;
				float px = (float) Math.Cos(angle) * length;
				float py = (float) Math.Sin(angle) * length;
				
				internalPoints.Add(new PointF(px, py));
			}

			// TODO: Do some fractal generation

			// TODO add solid clutter

			// Create a polygon from the two results. The external
			// then has the internal taken out of the middle to
			// represent the "hollow" space of the polygon.
			IPoly internalPolygon = CreatePolygon(internalPoints);

			// Set the polygons in the junction
			junction.InternalShape = internalPolygon;
		}

		/// <summary>
		/// Creates a polygon from the given list of points.
		/// </summary>
		private IPoly CreatePolygon(IList<PointF> points)
		{
			// Create the polygon
			PolyDefault pd = new PolyDefault();

			// Go through the points
			foreach (PointF p in points)
				pd.Add(p.X, p.Y);
			
			// Return the results
			return pd;
		}
	}
}
