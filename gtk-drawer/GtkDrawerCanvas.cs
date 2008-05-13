using C5;
using Cairo;
using Color = Cairo.Color;
using DrawingColor = System.Drawing.Color;
using Gdk;
using Gtk;
using MfGames.RunningBomb.Draw;
using MfGames.Utility;
using Rsvg;
using System;
using System.Drawing;
using System.IO;

namespace MfGames.RunningBomb.GtkDrawer
{
	/// <summary>
	/// Implements the basic rendering class for using Cairo to render
	/// graphics to the display.
	/// </summary>
	public class GtkDrawerCanvas
	: DrawingArea
	{
		#region Constructors
		public GtkDrawerCanvas()
		{
			// Connect the events
			Events |=
				Gdk.EventMask.VisibilityNotifyMask;
			ExposeEvent += OnExpose;
			ConfigureEvent += OnConfigure;
		}
		#endregion

		#region Drawing
		private Gdk.Color background = new Gdk.Color(128, 128, 128);
		private int width, height;
		private double handleSize = 10;
		private double barSize = 30;
		private double scale = 1.1;
		private double cx, cy;
		private DrawingCommands commands;

		/// <summary>
		/// Contains the background color for rendering.
		/// </summary>
		public Gdk.Color BackgroundColor
		{
			get { return background; }
			set { background = value; }
		}

		/// <summary>
		/// Contains the drawing commands used for this object.
		/// </summary>
		public DrawingCommands DrawingCommands
		{
			get { return commands; }
			set { commands = value; }
		}

		/// <summary>
		/// Sets the scaling value for this context.
		/// </summary>
		public double Scale
		{
			get { return scale; }
			set {
				scale = value;

				if (scale < 0.1)
					scale = 0.1;
			}
		}

		/// <summary>
		/// This method is called when the widget is configured, or
		/// resized.
		/// </summary>
		private void OnConfigure(object sender, ConfigureEventArgs args)
		{
			// Reset the central point
			width = args.Event.Width;
			height = args.Event.Height;
			Log.Debug("OnConfigure: {0}x{1}", width, height);
		}

		/// <summary>
		/// The primary redrawing function, this method is triggered
		/// when the object gets a exposed.
		/// </summary>
		private void OnExpose(object sender, ExposeEventArgs args)
		{
			// Get the cairo context
			Gdk.Window gw = GdkWindow;
			Cairo.Context g = CairoHelper.Create(gw);

			try
			{
				// Figure out the exposure area
				Gdk.Rectangle region = args.Event.Area;
				Gdk.GC gc = new Gdk.GC(gw);
				gc.ClipRectangle = region;

				// First wipe the area to make sure the translucency rules
				// don't get screwed up by redrawing on top of each other.
				gc.RgbFgColor = BackgroundColor;
				gw.DrawRectangle(gc, true, region);

				// Set up the scale
				g.Scale(scale, scale);
				cx = ((double) width / 2) / scale;
				cy = ((double) height / 2) / scale;

				// Draw the commands
				RenderCommands(g);

				// Draw the centering bar
				RenderCentering(g);
			}
			finally
			{
				// Clean up ourselves
				((IDisposable) g).Dispose();
			}
		}
		#endregion

		#region Render
		/// <summary>
		/// Draws a little widget in the center to indicate the center of
		/// the window.
		/// </summary>
		private void RenderCentering(Context g)
		{
			// We draw the centering box and handles independent of scale
			double s2 = (handleSize / 2) / scale;
			double bs = barSize / scale;

			g.Color = new Color(0, 0, 0, 0.5);
			g.LineWidth = 0.5 / scale;
			g.MoveTo(cx - bs, cy);
			g.LineTo(cx + bs, cy);
			g.Stroke();

			g.Color = new Color(0, 0, 0, 0.5);
			g.LineWidth = 0.5;
			g.MoveTo(cx, cy - bs);
			g.LineTo(cx, cy + bs);
			g.Stroke();

			g.Color = new Color(1, 1, 1, 0.5);
			g.MoveTo(cx - s2, cy - s2);
			g.LineTo(cx + s2, cy - s2);
			g.LineTo(cx + s2, cy + s2);
			g.LineTo(cx - s2, cy + s2);
			g.LineTo(cx - s2, cy - s2);
			g.Fill();

			g.Color = new Color(0, 0, 0, 0.5);
			g.MoveTo(cx - s2, cy - s2);
			g.LineTo(cx + s2, cy - s2);
			g.LineTo(cx + s2, cy + s2);
			g.LineTo(cx - s2, cy + s2);
			g.LineTo(cx - s2, cy - s2);
			g.Stroke();
		}

		/// <summary>
		/// Renders out the commands in order from the rendering object.
		/// </summary>
		private void RenderCommands(Context g)
		{
			// Ignore if we don't have commands
			if (commands == null)
				return;

			// Simply go through them in order
			double s = 10;

			foreach (IDrawable id in commands)
			{
				// Make some noise
				Log.Debug("Drawing: {0}", id.GetType());

				// Figure out the type
				g.LineWidth = 1;

				if (id is DrawCircle)
				{
					// Draw it as a circle
					DrawCircle idc = (DrawCircle) id;

					if (idc.FillColor != null)
					{
						DrawingColor c = idc.FillColor.Value;

						g.Color = new Color((double) c.R / 255,
							(double) c.G / 255,
							(double) c.B / 255);
						g.Arc(
							cx + idc.Point.X * s,
							cy + idc.Point.Y * s,
							(idc.Radius) * s,
							0, 2 * Math.PI);
						g.Fill();
					}

					if (idc.LineColor != null)
					{
						DrawingColor c = idc.LineColor.Value;

						g.Color = new Color((double) c.R / 255,
							(double) c.G / 255,
							(double) c.B / 255);
						g.Arc(
							cx + idc.Point.X * s,
							cy + idc.Point.Y * s,
							(idc.Radius) * s,
							0, 2 * Math.PI);
						g.Stroke();
					}
				}
			}
		}
		#endregion

		#region Properties
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
