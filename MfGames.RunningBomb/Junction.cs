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
		private double distance;

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
				JunctionNode junction = new JunctionNode(Random.Next());
				junction.ParentJunctionNode = this;
				junction.BuildShapes();

				// Get the segment factory and create the segment
				ISegmentFactory isf =
					FactoryManager.ChooseSegmentFactory(junction);
				Segment segment = isf.Create(junction, point);
				segment.ParentJunctionNode = this;
				segment.ChildJunctionNode = junction;
				segment.ChildJunctionPoint = point;

				// Set the junction's distance
				junction.Distance =
					Distance + 10 * segment.CenterPoints.MaximumRelativeDistance;

				// Check and add the overlap
				if (CheckOverlapIntersection(overlaps, segment))
				{
					// We intersect
					Log.Debug("Rejection because segments overlap");
					continue;
				}

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
		/// Returns true if the segment overlaps with the current list.
		/// </summary>
		private bool CheckOverlapIntersection(
			IList<IPoly> overlaps, Segment segment)
		{
			// First build up the overlap test shape
			IPoly newShape = segment.ChildJunctionNode.InternalShape
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

			// While we check the entire length of the segment to
			// prevent crossthroughs, we only store a slightly smaller
			// junction which excludes the center bit to allow for
			// that constant overlap.
			IPoly circleTest = Geometry.CreateCircle(
				PointF.Empty,
				Constants.OverlapConnectionDistance);
			IPoly toRemove = newShape.Intersection(circleTest);
			overlaps.Add(newShape.Xor(toRemove));

			// No intersections, so return false
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
