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
		public Segment Create(JunctionNode childNode, PointF childPoint)
		{
			// Sanity checking
			if (childNode.ParentJunctionNode == null)
				throw new Exception("Cannot generate with a null parent");

			// Get the distance between the two points
			Segment segment = new Segment();
			JunctionNode parentNode = childNode.ParentJunctionNode;

			// Add the two end points
			CenterPointList points = new CenterPointList();
			points.Add(new CenterPoint(0, 0));
			points.Add(new CenterPoint(childPoint));

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

			foreach (CenterPoint point in points)
			{
				// Keep track of our retries
				int retry = 1;

				while (true)
				{
					// Create a shape
					IPoly p = Geometry.CreateShape(parentNode, point.Point,
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
					if (!shape.HasIntersection(p))
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
			Log.Debug("  Shape Time: {0}", stopwatch.Elapsed);
			stopwatch.Reset();
			stopwatch.Start();
#endif

			// Add the shape to the list
			segment.InternalShape = shape;

			// Set up the center line and optimize it
			segment.CenterPoints.AddAll(points);
			segment.CenterPoints.Optimize();

#if DEBUG
			// Show timing information
			stopwatch.Stop();
			Log.Debug("Optimze Time: {0}", stopwatch.Elapsed);
#endif

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
