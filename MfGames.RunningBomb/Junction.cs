using C5;
using Gpc;
using MfGames.Utility;
using Physics2DDotNet.Shapes;
using System;
#if DEBUG
using System.Diagnostics;
#endif
using System.Drawing;
using Vector2D = AdvanceMath.Vector2D;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// A junction node is the primary point of the game. Each node is
	/// where there is potentially a special situation and where the
	/// tunnel system branches out.
	/// </summary>
	public class Junction
	{
		#region Constructors
		/// <summary>
		/// Constructs the junction node with a random seed for the
		/// number generator, this should only be done for the initial
		/// node.
		/// </summary>
		public Junction()
		: this(Entropy.Next())
		{
		}

		/// <summary>
		/// Constructs the junction node with a given seed.
		/// </summary>
		public Junction(int seed)
		{
			randomSeed = seed;
			random = new MersenneRandom(seed);
		}
		#endregion

		#region Static Controls
		/// <summary>
		/// For applications that don't need physics data, set this
		/// static property to false and physics data will not be
		/// generated automatically.
		/// </summary>
		public static bool GeneratePhysics = true;
		#endregion

		#region Random Generation
		private int randomSeed;
		private MersenneRandom random;

		/// <summary>
		/// This is the parent junction. For the top-level, this will
		/// be null, otherwise it will contain the junction node that
		/// generated this node.
		/// </summary>
		public Junction ParentJunction;

		/// <summary>
		/// This is the random number generator used for this junction
		/// node. Be very careful about using this since using up
		/// entropy will create inconsistent results.
		/// </summary>
		public MersenneRandom Random
		{
			get { return random; }
		}

		/// <summary>
		/// Resets the junction node's status, including purging all
		/// generated data and resetting to a known seed value.
		/// </summary>
		public void Reset()
		{
			// Reset the flags
			builtShape = false;
			builtConnections = false;

			// Purge the stored data
			internalShape = null;
			shape = null;
			segments.Clear();
			physicsShapes.Clear();

			// Reset the random value
			random = new MersenneRandom(randomSeed);
		}
		#endregion

		#region Geometry
		private bool builtShape = false;
		private IPoly internalShape, shape;
		private double distance;

		/// <summary>
		/// Retrieves a list of all center points in this junction and
		/// all segments.
		/// </summary>
		public IList<CenterPoint> CenterPoints
		{
			get
			{
				LinkedList<CenterPoint> list = new LinkedList<CenterPoint>();

				foreach (Segment s in Segments)
					list.AddAll(s.CenterPoints);

				return list;
			}
		}

		/// <summary>
		/// Contains the distance of the center of the junction node
		/// from its parent.
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
		/// This is the poly that is used to describe the internal
		/// shape of the junction, where the shape and elements can
		/// safely go.
		/// </summary>
		public IPoly InternalShape
		{
			get { BuildShapes(); return internalShape; }
			set
			{
				internalShape = value;
				builtShape = true;
			}
		}

		/// <summary>
		/// The returned polygon is the shape of the junction plus all
		/// child junctions and segments.
		/// </summary>
		public IPoly Shape
		{
			get { BuildCombinedShape(); return shape; }
		}

		/// <summary>
		/// Generates the shape of the junction node.
		/// </summary>
		public void BuildShapes()
		{
			// We don't do anything if we already have a built shape
			if (builtShape)
				return;

			// Get the factory
			IJunctionFactory ijf = FactoryManager.ChooseJunctionFactory(random);
			ijf.Create(this);
		}

		/// <summary>
		/// Builds the internal shape of the junction and all
		/// children.
		/// </summary>
		private void BuildCombinedShape()
		{
			// Don't bother if we aren't null
			if (shape != null)
				return;

			// Get the internal shape (this builds the internal)
			shape = InternalShape.Duplicate();
			
			// Add all the segments (this builds segments)
			foreach (Segment s in segments)
			{
				// Add the segment
				shape = shape.Union(s.InternalShape);
				shape = shape.Union(s.ChildJunction.InternalShape.Translate(
						s.ChildJunctionPoint.X, s.ChildJunctionPoint.Y));
			}
			
			Log.Debug("Created shape 1: {0} inner {1} points",
				shape.InnerPolygonCount, shape.PointCount);

			// See if we have multiple inner shapes
			if (shape.InnerPolygonCount > 1)
			{
				// Try to merge them properly together
				IPoly pd = new PolyDefault();
				
				for (int i = 0; i < shape.InnerPolygonCount; i++)
				{
					IPoly ip = shape.GetInnerPoly(i);
					
					if (ip.IsHole())
					{
						pd.Add(ip);
						continue;
					}
					else
					{
						pd = pd.Union(ip);
					}
				}
				
				shape = pd;
				Log.Debug("Created shape 2: {0} inner {1} points",
					shape.InnerPolygonCount, shape.PointCount);
			}
		}

		/// <summary>
		/// Calculates the world distance from a given relative point
		/// in the junction.
		/// </summary>
		public double CalculateDistance(PointF point)
		{
			// Calculate from the center of each segment
			double dis = Geometry.CalculateDistance(PointF.Empty, point)
				+ this.Distance;

			// See if we have a shorter distance from one of the
			// segments, which will handle the parent segment or the others.
			foreach (Segment s in Segments)
				dis = Math.Min(dis,
					Geometry.CalculateDistance(s.ChildJunctionPoint, point)
					+ s.ChildJunction.Distance);

			// Return the results
			return dis;
		}
		#endregion

		#region Segments and Junctions
		private bool builtConnections = false;
		private bool isBuildingConnections = false;
		private LinkedList<Segment> segments =
			new LinkedList<Segment>();

		/// <summary>
		/// Returns true if the connections have been built.
		/// </summary>
		public bool HasBuiltConnections
		{
			get { return builtConnections; }
		}

		/// <summary>
		/// Returns a read-only list of the junction nodes from this
		/// element.
		/// </summary>
		public IList<Segment> Segments
		{
			get
			{
				BuildConnections();
				return new GuardedList<Segment>(segments);
			}
		}

		/// <summary>
		/// Creates the various node connections made to this
		/// junction.
		/// </summary>
		public void BuildConnections()
		{
			lock (this)
			{
				BuildConnectionsLocked();
			}
		}

		/// <summary>
		/// The internal version of the building that functions while
		/// thread-safe.
		/// </summary>
		private void BuildConnectionsLocked()
		{
			// Don't bother if we have connections already
			if (builtConnections)
				return;

			// Sanity checking
			if (isBuildingConnections)
				throw new Exception("Building connections is not reentrant");

			isBuildingConnections = true;

			// Make sure our shapes are built to keep the order consistent
			BuildShapes();

			// Keep track of the overlap detection code
			LinkedList<IPoly> overlaps = new LinkedList<IPoly>();

			// Process the parent element
			if (ParentJunction != null)
			{
				// Find our input segment
				Segment ps = ParentJunction.GetSegment(this);

				if (ps == null)
					throw new Exception("We got a null segment from parent");

				// Add the parent segment with swapped directions
				ps = ps.Swap();
				segments.Add(ps);

				// Add to the overlap, but don't bother checking (we
				// are guarenteed to the first).
				CheckOverlapIntersection(overlaps, ps);
			}

			// We want to create a random number of connections
			int connectionCount = Random.Next(
				Constants.BuildMinimumConnections,
				Constants.BuildMaximumConnections);

			// Go through each connection
			for (int i = 0; i < connectionCount; i++)
			{
				// Build up a connection in a random direction
				float angle = Random.NextSingle(0, 2 * (float) Math.PI);
				float length = Random.NextSingle(
					Constants.MinimumConnectionDistance,
					Constants.MaximumConnectionDistance);

				// Junction points are relative to the parent node
				PointF point = new PointF(
					(float) Math.Cos(angle) * length,
					(float) Math.Sin(angle) * length);

				// Create a new junction at this point
				Junction junction = new Junction(Random.Next());
				junction.ParentJunction = this;
				junction.BuildShapes();

				// Get the segment factory and create the segment
				ISegmentFactory isf =
					FactoryManager.ChooseSegmentFactory(junction);
				Segment segment = isf.Create(junction, point, Distance);
				segment.ParentJunction = this;
				segment.ChildJunction = junction;
				segment.ChildJunctionPoint = point;

				// Set the junction's distance
				junction.Distance =
					Distance + segment.CenterPoints.MaximumRelativeDistance;

				// Check and add the overlap
				if (CheckOverlapIntersection(overlaps, segment))
				{
					// We intersect
					Log.Debug("Rejection because segments overlap");
					continue;
				}

				// Add some clutter
				IClutterFactory icf =
					FactoryManager.ChooseClutterFactory(Random);
				icf.Create(segment);

				// Add it to the segments
				segments.Add(segment);
			}

			// Create the physics shapes
			if (GeneratePhysics)
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				BuildCombinedShape();
#if DEBUG
				// Show timing information
				stopwatch.Stop();
				Log.Debug("   Physics Shape: {0}", stopwatch.Elapsed);
				stopwatch.Reset();
				stopwatch.Start();
#endif
				CreateJunctionPhysics(0);
				
#if DEBUG
				// Show timing information
				stopwatch.Stop();
				Log.Debug("Physics Creation: {0}", stopwatch.Elapsed);
#endif
			}

			// We are done
			builtConnections = true;
			isBuildingConnections = false;
		}

		/// <summary>
		/// Retrieves a segment based on the child junction.
		/// </summary>
		public Segment GetSegment(Junction junction)
		{
			foreach (Segment s in segments)
				if (s.ChildJunction == junction)
					return s;

			return null;
		}

		/// <summary>
		/// Returns true if the segment overlaps with the current list.
		/// </summary>
		private bool CheckOverlapIntersection(
			IList<IPoly> overlaps, Segment segment)
		{
			// First build up the overlap test shape
			IPoly newShape = segment.ChildJunction.InternalShape
				.Translate(
					segment.ChildJunctionPoint.X,
					segment.ChildJunctionPoint.Y)
				.Union(segment.InternalShape);

			// Now check for an intersection with all the overlaps
			foreach (IPoly overlap in overlaps)
			{
				// If we intersect, then there is an overlap
				if (overlap.HasIntersection(newShape))
					return true;
			}

			// Store the circle that represents this junction to
			// prevent anything from getting too close.
			IPoly circleTest = Geometry.CreateCircle(
				segment.ChildJunctionPoint,
				Constants.OverlapConnectionDistance);
			//IPoly toRemove = newShape.Intersection(circleTest);
			overlaps.Add(circleTest); //newShape.Xor(toRemove));

			// No intersections, so return false
			return false;
		}
		#endregion

		#region Physics
		private LinkedList<IShape> physicsShapes =
			new LinkedList<IShape>();

		/// <summary>
		/// Contains a list of shapes for the physics engine, an
		/// inverse of the area's shape.
		/// </summary>
		public IList<IShape> PhysicsShapes
		{
			get { BuildConnections(); return physicsShapes; }
		}

		/// <summary>
		/// Adds a junction data to the physics layer.
		/// </summary>
		private void CreateJunctionPhysics(int depth)
		{
			// Create the initial junction, then create a box slightly
			// larger than it and XOR the results to get an inverse
			// shape (i.e. with the hole being where the players
			// go). We call Shape to force the creation of the shape.
			IPoly poly = Shape;
			RectangleF bounds = poly.Bounds;
			bounds.Inflate(10, 10);
			IPoly rectangle = Geometry.CreateRectangle(bounds);
			IPoly inverse = poly.Xor(rectangle);

			// Recursively process and add the junctions
			CreateJunctionPhysics(depth, inverse, bounds);

			// Make some noise
			Log.Info("Adding {0} physics shapes", physicsShapes.Count);
		}

		/// <summary>
		/// Recursively process the polygon to attempt to find a
		/// single polygon element that could be added to the physics
		/// layer without any holes or additional physics.
		/// </summary>
		private void CreateJunctionPhysics(
			int depth, IPoly poly, RectangleF bounds)
		{
			// Ignore empty polygons
			if (poly.InnerPolygonCount == 0)
				return;

			// See if we are a solid polygon
			double areaDifference = bounds.Width * bounds.Height - poly.Area;

			if (poly.InnerPolygonCount == 1)
			{
				if (poly.IsHole())
					// Don't add holes
					return;

				if (poly.PointCount == 4 && areaDifference <= 0.1f)
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
				CreateJunctionPhysics(depth + 1, poly, bounds, 0, 0);
				CreateJunctionPhysics(depth + 1, poly, bounds, 0, 1);
				CreateJunctionPhysics(depth + 1, poly, bounds, 1, 1);
				CreateJunctionPhysics(depth + 1, poly, bounds, 1, 0);
				return;
			}

			// We should never get a hole
			if (poly.IsHole())
			{
				// We shouldn't get this
				Log.Error("Got a top-level polygon hole");
				return;
			}

			// Create a polygon shape as vectors
			LinkedList<Vector2D> vectors = new LinkedList<Vector2D>();

			for (int i = 0; i < poly.PointCount; i++)
			{
				// Get the coordinates
				float x = (float) poly.GetX(i);
				float y = (float) poly.GetY(i);

				// Create the vector
				vectors.Add(new Vector2D(x, y));
			}

			// Convert it into a physics2d polygon shape. Making the
			// PolygonShape second parameter too small makes the game
			// basically unusable in terms of stage generation but
			// more accurate for impacts with the side.
			Vector2D [] array = vectors.ToArray();
			IShape ps = new PolygonShape(array, 5f);
			physicsShapes.Add(ps);
		}

		/// <summary>
		/// Splits apart a polygon into a quad and processes it for physics.
		/// </summary>
		private void CreateJunctionPhysics(
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
			IPoly rectangle = Geometry.CreateRectangle(rect);
			IPoly intersection = rectangle.Intersection(poly);

			// Process this one
			CreateJunctionPhysics(depth, intersection, rect);
		}
		#endregion

		#region Mobiles
		private LinkedList<Mobile> mobiles = new LinkedList<Mobile>();

		/// <summary>
		/// Contains a list of all mobiles in the junction.
		/// </summary>
		public IList<Mobile> Mobiles
		{
			get { return mobiles; }
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
					log = new Log(GetType());

				return log;
			}
		}
		#endregion
	}
}
