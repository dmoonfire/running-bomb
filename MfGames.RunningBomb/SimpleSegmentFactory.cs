using C5;
using Gpc;
using MfGames.Utility;
using System;
#if DEBUG
using System.Diagnostics;
#endif
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
				Math.Pow(childNode.Point.X, 2) +
				Math.Pow(childNode.Point.Y, 2));

			// Add the two end points
			LinkedList<PointF> points = new LinkedList<PointF>();
			points.Add(new PointF(0, 0));
			points.Add(childNode.Point);

			// Split apart the line
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif

			// Stagger (fractalize) the points
			Geometry.StaggerPoints(points, parentNode.Random,
				Constants.MinimumSegmentDistance,
				Constants.StaggerSegmentPasses);

#if DEBUG
			// Show timing information
			stopwatch.Stop();
			Log.Debug("Fractal Time: {0}", stopwatch.Elapsed);
			stopwatch.Reset();
			stopwatch.Start();
#endif

			// Once we generated a set of center points, we go through
			// and add a randomly-sized and angled square to each
			// point, unioning the results to get the shape of the
			// entire segment.
			IPoly shape = null;

			foreach (PointF point in points)
			{
				// Keep track of our retries
				int retry = 1;

				while (true)
				{
					// Create a shape
					IPoly p = Geometry.CreateShape(parentNode, point,
						retry * Constants.SegmentAverageWidth);

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

			// Remove both junction shapes from this
			//shape = shape.Difference(parentNode.InternalShape);
			//shape = shape.Difference(childNode.InternalShape);

#if DEBUG
			// Show timing information
			stopwatch.Stop();
			Log.Debug("Polygon Time: {0}", stopwatch.Elapsed);
			Log.Debug("Polygon Area: {0}", shape.GetArea());
			Log.Debug("Polygon Coun: {0}", shape.PointCount);
			Log.Debug("Polygon Innr: {0}", shape.InnerPolygonCount);
#endif

			// Add the shape to the list
			segment.InternalShape = shape;
			segment.CenterPoints.AddAll(points);

			// Return the results
			return segment;
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
