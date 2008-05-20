using C5;
using Gpc;
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
	}
}
