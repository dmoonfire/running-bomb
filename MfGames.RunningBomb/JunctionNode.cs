using C5;
using Gpc;
using MfGames.Utility;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// A junction node is the primary point of the game. Each node is
	/// where there is potentially a special situation and where the
	/// tunnel system branches out.
	/// </summary>
	public class JunctionNode
	{
		#region Constructors
		/// <summary>
		/// Constructs the junction node with a random seed for the
		/// number generator, this should only be done for the initial
		/// node.
		/// </summary>
		public JunctionNode()
		: this(Entropy.Next())
		{
		}

		/// <summary>
		/// Constructs the junction node with a given seed.
		/// </summary>
		public JunctionNode(int seed)
		{
			randomSeed = seed;
			random = new MersenneRandom(seed);
		}
		#endregion

		#region Random Generation
		private int randomSeed;
		private MersenneRandom random;

		/// <summary>
		/// This is the parent junction. For the top-level, this will
		/// be null, otherwise it will contain the junction node that
		/// generated this node.
		/// </summary>
		public JunctionNode ParentJunctionNode;

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
			segments.Clear();

			// Reset the random value
			random = new MersenneRandom(randomSeed);
		}
		#endregion

		#region Geometry
		private bool builtShape = false;
		private IPoly internalShape;

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
		/// Contains the point in 2D space where this junction is
		/// centered on. Typically, the starting one is located at
		/// (0,0), but the rest of them would be a number of meters
		/// away.
		/// </summary>
		public PointF Point;

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
		#endregion

		#region Segments and Junctions
		private bool builtConnections = false;
		private LinkedList<Segment> segments =
			new LinkedList<Segment>();

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
			// Don't bother if we have connections already
			if (builtConnections)
				return;

			// Make sure our shapes are built to keep the order consistent
			BuildShapes();

			// Keep track of the overlap detection code
			LinkedList<IPoly> overlaps = new LinkedList<IPoly>();

			// Process the parent element
			if (ParentJunctionNode != null)
			{
				// Find our input segment
				Segment ps = ParentJunctionNode.GetSegment(this);

				if (ps == null)
					throw new Exception("We got a null segment from parent");

				// Add the parent segment with swapped directions
				ps = ps.Swap();
				segments.Add(ps);

				// Add both the parent junction and the segment to the
				// overlap test to prevent overlapping.
				JunctionNode pj = ps.ChildJunctionNode;
				IPoly circleTest = Geometry.CreateCircle(
					pj.Point,
					Constants.OverlapConnectionDistance);
				IPoly overlapTest = pj.InternalShape.Translate(Point)
					.Union(ps.InternalShape);
				overlapTest = overlapTest.Intersection(circleTest);
				overlaps.Add(overlapTest);
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
				JunctionNode junction = new JunctionNode(Random.Next());
				junction.ParentJunctionNode = this;
				junction.Point = point;
				junction.BuildShapes();

				// Start the overlap testing with just the junction to
				// short-circuit it faster if there is an overlap.
				if (HasIntersection(overlaps, junction.InternalShape))
				{
					Log.Debug("Rejected because junctions overlap");
					continue;
				}

				// Get the segment factory
				ISegmentFactory isf =
					FactoryManager.ChooseSegmentFactory(junction);
				Segment segment = isf.Create(junction);
				segment.ParentJunctionNode = this;
				segment.ChildJunctionNode = junction;

				// Test to see if there is an overlap with the current
				// segments and junctions.
				IPoly circleTest = Geometry.CreateCircle(
					junction.Point, Constants.OverlapConnectionDistance);
				IPoly overlapTest = junction.InternalShape.Translate(point)
					.Union(segment.InternalShape);
				overlapTest = overlapTest.Intersection(circleTest);

				// If we have an overlap, we'll have too much
				// difficulty figuring out where the player is going,
				// so reject it.
				if (HasIntersection(overlaps, segment.InternalShape))
				{
					Log.Debug("Rejection because segments overlap");
					continue;
				}

				// We don't overlap, so add it to the test
				overlaps.Add(overlapTest);

				// Add it to the segments
				segments.Add(segment);
			}

			// We are done
			builtConnections = true;
		}

		/// <summary>
		/// Retrieves a segment based on the child junction.
		/// </summary>
		public Segment GetSegment(JunctionNode junction)
		{
			foreach (Segment s in segments)
				if (s.ChildJunctionNode == junction)
					return s;

			return null;
		}

		/// <summary>
		/// Returns true if the shape intersects any of the shapes in
		/// the list.
		/// </summary>
		private bool HasIntersection(IList<IPoly> overlaps, IPoly shape)
		{
			// Go through the list
			foreach (IPoly ip in overlaps)
				if (shape.HasIntersection(ip))
					return true;

			// No intersections
			return false;
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
