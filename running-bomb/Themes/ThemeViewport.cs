using MfGames.Sprite3;
using MfGames.Sprite3.Backends;
using System;
using System.Drawing;
using System.Xml;

namespace RunningBomb.Themes
{
	/// <summary>
	/// Defines the common functionality of theme rendering commands.
	/// </summary>
	public class ThemeViewport
	: IThemeCommand
	{
		#region Constructors
		/// <summary>
		/// Creates the viewport command from the XML stream.
		/// </summary>
		public ThemeViewport(XmlReader xml)
		{
			x = new ThemeSingle(xml["x"]);
			y = new ThemeSingle(xml["y"]);
			w = new ThemeSingle(xml["w"]);
			h = new ThemeSingle(xml["h"]);
		}
		#endregion

		#region Properties
		private ThemeSingle x, y, w, h;
		#endregion

		#region Rendering
		/// <summary>
		/// Renders the theme command out.
		/// </summary>
		public void Render(ThemeContext context)
		{
			// Get the values
			float rx = x.GetValue(context);
			float ry = y.GetValue(context);
			float rw = w.GetValue(context);
			float rh = h.GetValue(context);
			RectangleF bounds = new RectangleF(rx, ry, rw, rh);

			// Set the clipping area
			IBackendDrawingArgs ibda = context.DrawingArgs.Backend
				.SetClippingRegion(
					context.DrawingArgs.BackendDrawingArgs, bounds);
			context.DrawingArgs.BackendDrawingArgs = ibda;

			// Call the viewport
			context.Callback.DrawViewport(context.DrawingArgs, bounds);

			// Clear the clip
			context.DrawingArgs.BackendDrawingArgs =
				context.DrawingArgs.Backend.ClearClippingRegion(ibda);
		}
		#endregion
	}
}
