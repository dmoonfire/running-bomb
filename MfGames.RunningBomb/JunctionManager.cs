using C5;
using Gpc;
using MfGames.Utility;
using Physics2DDotNet;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Encapsulates the code for managing junctions, including
	/// loading and switching elements within the physics engine,
	/// adding junction elements to the physics, and swapping out
	/// junctions.
	/// </summary>
	public class JunctionManager
	{
		#region Junction
		private Junction junction;

		/// <summary>
		/// Setting this junction sets the various elements of the
		/// game to manage the junction, including adding it to the
		/// physics and swapping out any junction already loaded.
		/// </summary>
		public Junction Junction
		{
			get { return junction; }
			set
			{
				// See if we have an old junction
				if (junction != null)
				{
					// Swap out the old junction
					RemoveJunctionPhysics();
				}

				// Sets the new junction
				junction = value;

				// We are done if we are null
				if (junction == null)
					return;

				// Add ourselves to the physics
				AddJunctionPhysics(0);
			}
		}
		#endregion

		#region Physics
		private LinkedList<Body> junctionBodies = new LinkedList<Body>();

		/// <summary>
		/// Returns a read-only list of bodies for the junctions.
		/// </summary>
		public IList<Body> JunctionBodies
		{
			get { return junctionBodies; }
		}

		/// <summary>
		/// Adds a junction data to the physics layer.
		/// </summary>
		private void AddJunctionPhysics(int depth)
		{
			// Create the initial junction, then create a box slightly
			// larger than it and XOR the results to get an inverse
			// shape (i.e. with the hole being where the players go).
			IPoly shape = junction.Shape.Duplicate();
			RectangleF bounds = shape.Bounds;
			bounds.Inflate(10, 10);
			IPoly rectangle = CreateRectangle(bounds);
			IPoly inverse = shape.Xor(rectangle);

			// Recursively process and add the junctions
			AddJunctionPhysics(depth, inverse, bounds);

			// Make some noise
			Log.Info("Adding {0} junction physic blocks", junctionBodies.Count);
		}

		/// <summary>
		/// Recursively process the polygon to attempt to find a
		/// single polygon element that could be added to the physics
		/// layer without any holes or additional physics.
		/// </summary>
		private void AddJunctionPhysics(
			int depth, IPoly poly, RectangleF bounds)
		{
			// Ignore empty polygons
			if (poly.InnerPolygonCount == 0)
				return;

			// See if we are a solid polygon
			double areaDifference = bounds.Width * bounds.Height - poly.Area;

			if (poly.InnerPolygonCount == 1 &&
				poly.PointCount == 4 &&
				areaDifference <= 0.1f)
			{
				// We appear to be at least mostly solid, drop it
				return;
			}

			// If we have more than one polygon, split it
			if (poly.InnerPolygonCount > 1 ||
				bounds.Width > Constants.MaximumJunctionPhysicsBlock ||
				bounds.Height > Constants.MaximumJunctionPhysicsBlock)
			{
				// We split the polygon into quads and process each
				// one to add it recursively.
				AddJunctionPhysics(depth + 1, poly, bounds, 0, 0);
				AddJunctionPhysics(depth + 1, poly, bounds, 0, 1);
				AddJunctionPhysics(depth + 1, poly, bounds, 1, 1);
				AddJunctionPhysics(depth + 1, poly, bounds, 1, 0);
				return;
			}

			// We should never get a hole
			if (poly.IsHole())
			{
				// We shouldn't get this
				Log.Error("Got a top-level polygon hole");
				return;
			}

			// Create a physics object from this point
			junctionBodies.Add(State.Physics.AddImmobile(poly));
		}

		/// <summary>
		/// Splits apart a polygon into a quad and processes it for physics.
		/// </summary>
		private void AddJunctionPhysics(
			int depth,
			IPoly poly, RectangleF bounds,
			int row, int col)
		{
			// Figure out the desired size and shape
			SizeF size = new SizeF(bounds.Width / 2, bounds.Height / 2);
			PointF point = new PointF(
				bounds.X + col * bounds.Width / 2,
				bounds.Y + row * bounds.Height / 2);
			RectangleF rect = new RectangleF(point, size);

			// Create the GPC polygon and calculate the intersection
			IPoly rectangle = CreateRectangle(rect);
			IPoly intersection = rectangle.Intersection(poly);

			// Process this one
			AddJunctionPhysics(depth, intersection, rect);
		}

		/// <summary>
		/// Constructs a rectangle shape for GPC from the given
		/// bounds.
		/// </summary>
		private IPoly CreateRectangle(RectangleF bounds)
		{
			IPoly rectangle = new PolyDefault();
			rectangle.Add(bounds.Left, bounds.Top);
			rectangle.Add(bounds.Right, bounds.Top);
			rectangle.Add(bounds.Right, bounds.Bottom);
			rectangle.Add(bounds.Left, bounds.Bottom);
			return rectangle;
		}

		/// <summary>
		/// Removes the junction data from the physics layer.
		/// </summary>
		private void RemoveJunctionPhysics()
		{
			// Go through the list
			foreach (Body body in junctionBodies)
				body.Lifetime.IsExpired = true;

			// Clear it since it will remove itself
			junctionBodies.Clear();
		}
		#endregion

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
					log = new Log(typeof(JunctionManager));

				return log;
			}
		}
		#endregion
	}
}
