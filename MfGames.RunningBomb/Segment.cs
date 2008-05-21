using C5;
using Gpc;
using MfGames.Utility;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// A segment is a connection between two junctions. It constists
	/// of multiple SegmentNode objects and additional data.
	/// </summary>
	public class Segment
	{
		#region Junctions
		private JunctionNode parent, child;
		private bool swapped;

		/// <summary>
		/// Sets the child junction node for this segment.
		/// </summary>
		public JunctionNode ChildJunctionNode
		{
			get { return child; }
			set
			{
				if (value == null)
					throw new Exception("Cannot set a null child node");

				child = value;
			}
		}

		/// <summary>
		/// Sets the parent junction node for this segment.
		/// </summary>
		public JunctionNode ParentJunctionNode
		{
			get { return parent; }
			set
			{
				if (value == null)
					throw new Exception("Cannot set a null parent node");

				parent = value;
			}
		}

		/// <summary>
		/// Returns true if this is a swapped or reversed segment.
		/// </summary>
		public bool IsSwapped
		{
			get { return swapped; }
		}
		#endregion

		#region Nodes
		private IPoly internalShape;
		private LinkedList<PointF> centerPoints =
			new LinkedList<PointF>();

		/// <summary>
		/// Contains a list of the center-line points for the segment.
		/// </summary>
		public IList<PointF> CenterPoints
		{
			get { return centerPoints; }
		}

		/// <summary>
		/// Contains the internal shape to this segment.
		/// </summary>
		public IPoly InternalShape
		{
			get { return internalShape; }
			set { internalShape = value; }
		}
		#endregion

		#region Swapping
		/// <summary>
		/// Creates a new segment with the ends swapped and the
		/// polygons reversed.
		/// </summary>
		public Segment Swap()
		{
			// Create a new segment
			Segment s = new Segment();
			s.swapped = !swapped;
			s.child = parent;
			s.parent = child;

			// Reverse the polygon of the segment
			double dx = -child.Point.X;
			double dy = -child.Point.Y;
			PolyDefault ps = new PolyDefault();
			int count = internalShape.PointCount;

			for (int i = 0; i < count; i++)
			{
				double x = dx + internalShape.GetX(i);
				double y = dy + internalShape.GetY(i);

				ps.Add(x, y);
			}

			s.internalShape = ps;

			// Reverse the center lines
			foreach (PointF pf in centerPoints)
			{
				s.centerPoints.Add(
					new PointF((float) dx + pf.X, (float) dy + pf.Y));
			}

			// Return the results
			return s;
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
