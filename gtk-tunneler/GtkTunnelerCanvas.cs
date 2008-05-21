using C5;
using Cairo;
using Color = Cairo.Color;
using DrawingColor = System.Drawing.Color;
using Gdk;
using Gpc;
using Gtk;
using MfGames.RunningBomb;
using MfGames.Utility;
using Rsvg;
using System;
using System.Drawing;
using System.IO;

namespace MfGames.RunningBomb.GtkTunneler
{
	/// <summary>
	/// Implements the basic rendering class for using Cairo to render
	/// graphics to the display.
	/// </summary>
	public class GtkTunnelerCanvas
	: DrawingArea
	{
		#region Constructors
		public GtkTunnelerCanvas()
		{
			// Connect the events
			Events |=
				Gdk.EventMask.Button1MotionMask |
				Gdk.EventMask.Button2MotionMask |
				Gdk.EventMask.ButtonPressMask |
				Gdk.EventMask.ButtonReleaseMask |
				Gdk.EventMask.VisibilityNotifyMask |
				Gdk.EventMask.PointerMotionMask |
				Gdk.EventMask.PointerMotionHintMask;
			ExposeEvent += OnExpose;
			ConfigureEvent += OnConfigure;
			ButtonReleaseEvent += OnButtonRelease;
			MotionNotifyEvent += OnMotionNotify;

			// Create the root node with a random seed
			rootJunctionNode = new JunctionNode();
			selectedJunctionNode = rootJunctionNode;
		}
		#endregion

		#region Properties
		private JunctionNode rootJunctionNode;
		private JunctionNode selectedJunctionNode;
		#endregion

		#region Drawing
		private Gdk.Color background = new Gdk.Color(128, 128, 128);
		private int width, height;
		private double handleSize = 5;
		private float scale = 1;
		private float cx, cy;

		/// <summary>
		/// Contains the background color for rendering.
		/// </summary>
		public Gdk.Color BackgroundColor
		{
			get { return background; }
			set { background = value; }
		}

		/// <summary>
		/// Sets the scaling value for this context.
		/// </summary>
		public float Scale
		{
			get { return scale; }
			set {
				scale = value;

				if (scale < 0.0001f)
					scale = 0.0001f;
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
				cx = ((float) width / 2) / scale;
				cy = ((float) height / 2) / scale;

				// Draw all the junctions
				junctionHandles.Clear();
				RenderJunction(g, selectedJunctionNode, PointF.Empty, 1);
			}
			finally
			{
				// Clean up ourselves
				((IDisposable) g).Dispose();
			}
		}

		/// <summary>
		/// Rebuilds the internal junction.
		/// </summary>
		public void Rebuild()
		{
			rootJunctionNode = new JunctionNode();
			selectedJunctionNode = rootJunctionNode;
			QueueDraw();
		}

		/// <summary>
		/// Reloads the internal junction.
		/// </summary>
		public void Reload()
		{
			selectedJunctionNode = rootJunctionNode;
			rootJunctionNode.Reset();
			QueueDraw();
		}

		/// <summary>
		/// Renders out a single junction to the canvas.
		/// </summary>
		private void RenderJunction(
			Context g, JunctionNode junction, 
			PointF point,
			int recursion)
		{
			// Get the shape we need to draw and draw it
			IPoly poly = junction.InternalShape;
			RenderPolygon(g, poly, point, new Color(0, 0, 0));

			// See if we are recursive
			if (recursion-- > 0)
			{
				// Got through the shapes
				foreach (Segment s in junction.Segments)
				{
					// Render the junction
					RenderJunction(g,
						s.ChildJunctionNode,
						s.ChildJunctionNode.Point,
						recursion);
				}

				// Got through the shapes
				foreach (Segment s in junction.Segments)
				{
					// Render the segment between the two
					RenderSegment(g, s);

					// Render and save the junction handles
					RenderJunctionHandle(
						g,
						s.ChildJunctionNode.Point,
						s.ChildJunctionNode == hoverJunctionNode);

					// Save the point
					junctionHandles.Add(s.ChildJunctionNode);
				}
			}
		}

		/// <summary>
		/// Draws a little widget in the center to indicate the center of
		/// the junction.
		/// </summary>
		private void RenderJunctionHandle(Context g, PointF point, bool hover)
		{
			// Set up the variables
			PointF p = new PointF(cx + point.X, cy + point.Y);
			double s2 = (handleSize) / scale;

			// Show it as a simple square handle
			if (hover)
				g.Color = new Color(1, 0, 0);
			else
				g.Color = new Color(1, 1, 1, 0.5);

			g.MoveTo(p.X - s2, p.Y - s2);
			g.LineTo(p.X + s2, p.Y - s2);
			g.LineTo(p.X + s2, p.Y + s2);
			g.LineTo(p.X - s2, p.Y + s2);
			g.LineTo(p.X - s2, p.Y - s2);
			g.Fill();
			
			if (hover)
				g.Color = new Color(1, 1, 1);
			else
				g.Color = new Color(0, 0, 0, 0.5);

			g.MoveTo(p.X - s2, p.Y - s2);
			g.LineTo(p.X + s2, p.Y - s2);
			g.LineTo(p.X + s2, p.Y + s2);
			g.LineTo(p.X - s2, p.Y + s2);
			g.LineTo(p.X - s2, p.Y - s2);
			g.Stroke();
		}

		/// <summary>
		/// Renders an arbitrary polygon to the context.
		/// </summary>
		private void RenderPolygon(
			Context g, IPoly poly, PointF point, Color color)
		{
			// Save the first point
			PointF firstPoint = PointF.Empty;

			// Go through the points of the polygon
			for (int i = 0; i < poly.PointCount; i++)
			{
				// Pull out the coordinates
				float x = (float) poly.GetX(i);
				float y = (float) poly.GetY(i);
				PointF p = new PointF(cx + point.X + x, cy + point.Y + y);

				// Either move or line
				if (firstPoint == PointF.Empty)
				{
					firstPoint = p;
					g.MoveTo(p.X, p.Y);
				}
				else
				{
					g.LineTo(p.X, p.Y);
				}
			}

			// Finish up
			g.LineTo(firstPoint.X, firstPoint.Y);
			g.Color = color;
			g.Fill();
		}

		/// <summary>
		/// Renders out the segment between two junctions.
		/// </summary>
		private void RenderSegment(Context g, Segment segment)
		{
			// Get the shape we need to draw and draw it
			IPoly poly = segment.InternalShape;

			if (poly != null)
			{
				// Go through the points and draw with a fill
				JunctionNode junction = segment.ParentJunctionNode;
				RenderPolygon(g,
					poly,
					PointF.Empty,
					new Color(0, 0, 0.5f));
				
				double color = 1;
				
				for (int i = 1; i < poly.InnerPolygonCount; i++)
				{
					RenderPolygon(g,
						poly.GetInnerPoly(i),
						PointF.Empty,
						new Color(color, 0, 0.5f));
					color /= 1.5;
				}
			}
				
			// Draw the center points
			foreach (PointF point in segment.CenterPoints)
				RenderSegmentHandle(g, point);
		}

		/// <summary>
		/// Draws a little widget in the center to indicate the center of
		/// the segment point.
		/// </summary>
		private void RenderSegmentHandle(Context g, PointF segmentPoint)
		{
			// Set up the variables
			PointF p = new PointF(cx + segmentPoint.X, cy + segmentPoint.Y);
			double s2 = (handleSize / 4) / scale;

			// Draw a red circle
			g.Color = new Color(0, 0, 0.75f);
			g.Arc(p.X, p.Y, s2, 0, Math.PI * 2);
			g.Fill();
		}
		#endregion
		
		#region Mouse Movement
		private LinkedList<JunctionNode> junctionHandles =
			new LinkedList<JunctionNode>();
		private JunctionNode hoverJunctionNode;

		/// <summary>
		/// Triggered when the button is released (lifted).
		/// </summary>
		public void OnButtonRelease(object sender, ButtonReleaseEventArgs args)
		{
			// See if we have a hover
			if (hoverJunctionNode != null)
				selectedJunctionNode = hoverJunctionNode;
				
			// Force a redraw
			QueueDraw();
		}

		/// <summary>
		/// Triggered when the mouse is moved around the widget.
		/// </summary>
		public void OnMotionNotify(object sender, MotionNotifyEventArgs args)
		{
			// Get the x and y coordinates
			double x, y;
			
			if (args.Event.IsHint)
			{
				Gdk.ModifierType m;
				int ix, iy;
				args.Event.Window.GetPointer(out ix, out iy, out m);
				x = ix;
				y = iy;
			}
			else
			{
				x = args.Event.X;
				y = args.Event.Y;
			}

			double hs = handleSize / scale;
			x = x / scale - cx;
			y = y / scale - cy;

			// Go through the list and see if we are near a handle
			hoverJunctionNode = null;

			foreach (JunctionNode jn in junctionHandles)
			{
				if (jn.Point.X - hs <= x && x <= jn.Point.X + hs &&
					jn.Point.Y - hs <= y && y <= jn.Point.Y + hs)
				{
					hoverJunctionNode = jn;
					break;
				}
			}
			
			// Force the drawing
			QueueDraw();
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
