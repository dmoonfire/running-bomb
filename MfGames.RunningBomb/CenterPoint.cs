using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Implements a center point which contains processing
	/// information for calculating distance from a junction.
	/// </summary>
	public class CenterPoint
	{
		#region Constructors
		public CenterPoint()
		{
		}

		public CenterPoint(float x, float y)
		{
			X = x;
			Y = y;
		}

		public CenterPoint(PointF point)
		{
			X = point.X;
			Y = point.Y;
		}
		#endregion

		#region Properties
		private float distance;

		/// <summary>
		/// Creates a new PointF for this point.
		/// </summary>
		public PointF Point
		{
			get { return new PointF(X, Y); }
		}

		/// <summary>
		/// Contains the relative distance from the parent junction.
		/// </summary>
		public float RelativeDistance
		{
			get { return distance; }
			set
			{
				if (value < 0)
					throw new Exception("Cannot set negative distance");

				distance = value;
			}
		}

		/// <summary>
		/// Contains the X coordinate of this point.
		/// </summary>
		public float X;

		/// <summary>
		/// Contains the Y coordinate of this point.
		/// </summary>
		public float Y;
		#endregion

		#region Geometry
		/// <summary>
		/// Gets the distance between two points.
		/// </summary>
		public float GetDistance(CenterPoint point)
		{
			return Geometry.CalculateDistance(Point, point.Point);
		}
		#endregion
	}
}
