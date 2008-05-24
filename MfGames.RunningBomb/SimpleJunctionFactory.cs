using C5;
using Gpc;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Implements a simplistic junction which is just a large series
	/// of random shapes.
	/// </summary>
	public class SimpleJunctionFactory
	: IJunctionFactory
	{
		/// <summary>
		/// Constructs the junction node's shape and sets internal
		/// structures to match the new shape.
		/// </summary>
		public void Create(Junction junction)
		{
			// We always start with one shape
			IPoly shape = Geometry.CreateShape(junction, PointF.Empty,
				Constants.JunctionAverageWidth);
			
			// Figure out the basic points by creating an array
			// simples, which is determined by the random number generator.
			int simpleCount = junction.Random.Next(8, 32);

			// Populate the simple points
			for (int i = 0; i < simpleCount; i++)
			{
				// Create the length from a random number in meters.
				float length = junction.Random
					.NextSingle(0, Constants.JunctionAverageWidth);

				// Figure out the angle we are calculating for the
				// internal shape, then use that to calculate the
				// internal point. These points are relative to (0,0).
				float angle = (float) Math.PI * 2 * i / simpleCount;
				float px = (float) Math.Cos(angle) * length;
				float py = (float) Math.Sin(angle) * length;
				int retry = 0;

				while (true)
				{
					// Create a shape at that point
					IPoly p = Geometry.CreateShape(junction,
						new PointF(px, py),
						retry * Constants.JunctionAverageWidth);
					
					// If we have no shape, this is automatically
					// included.
					if (shape == null)
					{
						shape = p;
						break;
					}
					
					// We have another shape, so we want to build up
					// an intersection to make sure they are touching.
					IPoly intersect = shape.Intersection(p);
					
					if (intersect.PointCount == 0)
					{
						// If there is no intersection, then we need
						// to try again and make the radius range larger.
						retry++;
						continue;
					}
					
					// Make a union of the two shapes
					IPoly union = shape.Union(p);
					
					// See if we have two shapes
					if (union.InnerPolygonCount > 1)
					{
						// We didn't quite reach each other
						retry++;
						continue;
					}
					
					// Set the new shape
					shape = union;
					break;
				}
			}

			// TODO: Do some fractal generation

			// TODO add solid clutter

			// Set the polygons in the junction
			junction.InternalShape = shape;
		}
	}
}
