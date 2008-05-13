using System;
using System.Xml;

namespace MfGames.RunningBomb.Draw
{
	/// <summary>
	/// Represents an abstracted circle with common drawing
	/// attributes.
	/// </summary>
	public class DrawCircle
	: CommonDrawShape
	{
		#region Constructors
		public DrawCircle(XmlReader xml)
			: base(xml)
		{
			// Read our elements
			radius = Single.Parse(xml["radius"]);
		}
		#endregion
		
		#region Properties
		private float radius = 1;

		/// <summary>
		/// Contains the radius of the circle, in meters.
		/// </summary>
		public float Radius
		{
			get { return radius; }
			set
			{
				if (value <= 0)
					throw new Exception(
						"Cannot create a circle with radius <= 0");

				radius = value;
			}
		}
		#endregion
	}
}
