using System;
using System.Drawing;
using System.Xml;

namespace MfGames.RunningBomb.Draw
{
	/// <summary>
	/// Abstract class that contains the most common elements that
	/// most drawing elements share (a single point, color, and fill).
	/// </summary>
	public abstract class CommonDrawShape
	: IDrawable
	{	
		#region Constructors
		/// <summary>
		/// Creates the shape with common elements.
		/// </summary>
		protected CommonDrawShape(XmlReader xml)
		{
			// Read our elements
			float x = Single.Parse(xml["x"]);
			float y = Single.Parse(xml["y"]);
			Point = new PointF(x, y);

			// Add the colors, if we have one
			if (xml["fill-color"] != null)
			{
				if (xml["fill-color"].ToLower() == "none")
					FillColor = null;
				else
					FillColor = ColorTranslator.FromHtml(xml["fill-color"]);
			}

			if (xml["line-color"] != null)
			{
				if (xml["line-color"].ToLower() == "none")
					LineColor = null;
				else
					LineColor = ColorTranslator.FromHtml(xml["line-color"]);
			}
		}
		#endregion
		
		#region Properties
		private float lineWidth = 0.01f;

		/// <summary>
		/// The color to fill the shape if this is not null. If it is
		/// null, then the fill is not drawn.
		/// </summary>
		public Color? FillColor = null;

		/// <summary>
		/// The color to draw the outline of the shape. If this is
		/// null, then the line is not drawn.
		/// </summary>
		public Color? LineColor = Color.White;

		public float LineWidth
		{
			get { return lineWidth; }
			set
			{
				if (value <= 0)
					throw new Exception(
						"Cannot assign line width <= 0");

				lineWidth = value;
			}
		}

		/// <summary>
		/// Contains the relative point to the center of the object, in meters.
		/// </summary>
		public PointF Point;
		#endregion
	}
}
