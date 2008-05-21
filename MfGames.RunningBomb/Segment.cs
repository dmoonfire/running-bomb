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
		/// Contains the point in 2D space where this junction is
		/// related to the parent node.
		/// </summary>
		public PointF ChildJunctionPoint;

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
			s.child = parent;
			s.parent = child;
			s.ChildJunctionPoint = new PointF(
				-ChildJunctionPoint.X,
				-ChildJunctionPoint.Y);

			// Reverse the relative coordinates of the segment
			double dx = -ChildJunctionPoint.X;
			double dy = -ChildJunctionPoint.Y;

			// Shift the polygon over as appropriate
			s.internalShape = internalShape.Translate(dx, dy);

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
