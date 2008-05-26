using AdvanceMath;
using BooGame.Video;
using C5;
using Gpc;
using MfGames.RunningBomb;
using MfGames.Sprite3;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using System;
using System.Drawing;
using Tao.OpenGl;
using Tesselate;

namespace RunningBomb
{
	/// <summary>
	/// Encapsulates the code for displaying the universe in the HUD.
	/// </summary>
	public abstract class DisplayUniverseAbstractMode
	: HudAbstractMode
	{
		#region Drawing and Rendering
		private PointF playerPoint;
		private PointF screenPoint;

        /// <summary>
		/// Renders the universe currently loaded into the screen,
		/// then passes the rendering for the HUD to overlay it.
        /// </summary>
        /// <param name="args"></param>
        public override void DrawViewport(
			DrawingArgs args, RectangleF bounds)
		{
			// Set the player point
			screenPoint.X = bounds.X + bounds.Width / 2;
			screenPoint.Y = bounds.Y + 3 * bounds.Height / 4;

			// Adjust for the player's angle
			playerPoint = State.Player.Point;

			// If we need it, draw the outline of the physics object
#if DEBUG
			//RenderJunctions();
#endif
			RenderJunctionShape(bounds);

			// Render all the mobiles
			foreach (Mobile m in State.Physics.Mobiles)
				Render(m);
		}

		/// <summary>
		/// Triggered when an object hits something.
		/// </summary>
		private void OnBodyCollided(object sender, EventArgs args)
		{
			Log.Info("Collission!");
		}

		/// <summary.
		/// Renders out the various junction shapes to the user.
		/// </summary>
		private void RenderJunctions()
		{
			// Go through their bodies
			foreach (Body body in State.JunctionManager.JunctionBodies)
			{
				RenderPolygon((PolygonShape) body.Shape, body.Matrices.ToWorld);
			}
		}

		private void RenderPolygon(PolygonShape pps, Matrix2x3 matrix)
		{
			/* HACK: This is a hack. I couldn't figure out the
			// calculations at this point, so I just dove into the
			// physics system to figure out the points. */
			
			// Try to figure out the real shape based on the
			// physics object
			for (int i = 0; i < pps.Vertexes.Length; i++)
			{
				Vector2D v1, v2;
				int i1 = i;
				int i2 = i + 1;
				
				if (i2 >= pps.Vertexes.Length)
					i2 = 0;
				
				Vector2D.Transform(ref matrix,
					ref pps.Vertexes[i1], out v1);
				Vector2D.Transform(ref matrix,
					ref pps.Vertexes[i2], out v2);
				PointF p1 = ToPoint(v1.X, v1.Y);
				PointF p2 = ToPoint(v2.X, v2.Y);
				
				Color color = Color.Gray;

				if (State.Score.Countdown == 0)
					color = Color.Red;

				Paint.Line(p1.X, p1.Y, p2.X, p2.Y, color);
			}
		}

		private void Render(Mobile mobile)
		{
			RenderPolygon((PolygonShape) mobile.PhysicsBody.Shape,
				mobile.PhysicsBody.Matrices.ToWorld);
		}

		/// <summary>
		/// Scales a value to the current scale value.
		/// </summary>
		private float ToScale(float value)
		{
			return value * ViewState.Scale;
		}

		/// <summary>
		/// Scales and adjusts a value to correlate to the
		/// player-specific view.
		/// </summary>
		private PointF ToPoint(float x, float y)
		{
			// Adjust the point to center on the player
			x -= State.Player.Point.X;
			y -= State.Player.Point.Y;

			// Figure out the angle of this object to the center
			double distance = Math.Sqrt(x * x + y * y);
			double angle = Math.Atan2(x, y) + State.Player.Angle - Constants.PI;

			// Recalculate position based on angle
			x = (float) (Math.Cos(angle) * distance);
			y = (float) (Math.Sin(angle) * distance);

			// Adjust for scale
			x *= ViewState.Scale;
			y *= ViewState.Scale;

			// Adjust for the center of the screen
			x += screenPoint.X;
			y += screenPoint.Y;

			// Return the results
			return new PointF(x, y);
		}
		#endregion

		#region Junction Rendering
		private LinkedList<PointF> renderPoints = new LinkedList<PointF>();

		/// <summary>
		/// Renders out the junction shape to the screen.
		/// </summary>
		private void RenderJunctionShape(RectangleF bounds)
		{
			// Create a new polygon intersection
			PointF pp = State.Player.Point;
			PointF p1 = ToPoint(pp.X - bounds.Width, pp.Y - bounds.Height);
			PointF p2 = ToPoint(pp.X + bounds.Width, pp.Y + bounds.Height);
			IPoly screenRect = Geometry.CreateRectangle(
				new RectangleF(p1, new SizeF(p2.X - p1.X, p2.Y - p1.Y)));

			// Go through the internal shape
			IPoly shape = State.JunctionManager.Junction.Shape;
			IPoly shape0 = new PolyDefault();

			for (int i = 0; i < shape.PointCount; i++)
			{
				PointF p0 =
					ToPoint((float) shape.GetX(i), (float) shape.GetY(i));
				shape0.Add(p0);
			}

			IPoly poly = screenRect.Intersection(shape0);

			// Create the AGG tesselator and set it up
			Tesselator tess = new Tesselator();
			tess.callCombine += OnTesselatorCombine;
			tess.callVertex += OnVertex;
			tess.callMesh += OnMesh;
			tess.callBegin += OnTesselatorBegin;
			tess.callEnd += OnTesselatorEnd;

			// OpenGL setup
            Gl.glPushMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            BooGame.Camera.ApplyMatrix();

			// Go through the junction's internal shape
			renderPoints.Clear();
			tess.BeginPolygon();
			tess.BeginContour();

			for (int i = 0; i < poly.PointCount; i++)
			{
				// Get the point
				PointF p = new PointF(
					(float) poly.GetX(i), (float) poly.GetY(i));
					
				// Add the vertex with the index it will have when we
				// refer to it later.
				tess.AddVertex(
					new double [] { p.X, p.Y, 0 }, renderPoints.Count);
				renderPoints.Add(p);
			}

			// Finish up the polygon, which renders the rendering
			// commands
			//Log.Info("Tesselating {0} points", renderPoints.Count);
			tess.EndContour();
			tess.EndPolygon();

			// Finish up OpenGl
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();
		}

		/// <summary>
		/// Called to start up the GL processing.
		/// </summary>
		private void OnTesselatorBegin(Tesselator.TriangleListType type)
		{
			// Figure out the type of start
			switch (type)
			{
				case Tesselator.TriangleListType.Triangles:
					Gl.glBegin(Gl.GL_TRIANGLES);
					break;
					
				case Tesselator.TriangleListType.TriangleFan:
					Gl.glBegin(Gl.GL_TRIANGLE_FAN);
					break;
					
				case Tesselator.TriangleListType.TriangleStrip:
					Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
					break;
					
				default:
					throw new Exception("unknown TriangleListType: " + type);
			}
		}

		/// <summary>
		/// Finishes up the OpenGL command.
		/// </summary>
		private void OnTesselatorEnd()
		{
			Gl.glEnd();
		}

		/// <summary>
		/// Vertex callback on the tesselator.
		/// </summary>
		private void OnVertex(int data)
		{
			// Adjust the point for the current view
			PointF p = renderPoints[data];

			// Figure out the distance from the core and use that to
			// set the color of the vertex.
			double distance =
				State.JunctionManager.Junction.CalculateDistance(p);

			SetColor(distance);

			// Render the vertex
			Gl.glVertex2f(p.X, p.Y);
		}

		private void OnTesselatorCombine(
			double [] coords3, int [] data4, double[] weight4, out int outData)
		{
			// Create a new point and add it
			PointF p = new PointF((float) coords3[0], (float) coords3[1]);
			outData = renderPoints.Count;
			renderPoints.Add(p);
		}

		private void OnMesh(Mesh mesh)
		{
			//Log.Debug("  mesh!");
		}

		/// <summary>
		/// Figures out the base background color for the polygon.
		/// </summary>
		private void SetColor(double distance)
		{
			// Convert this to a ratio
			double ratio = distance / Constants.BombSafeDistance;
			double r = 1;
			double g = 1;
			double b = 0.25;

			// Set the red tones
			if (ratio <= 0.25)
				r = 1 - (3 * ratio);
			else
				r = 0.25;

			// Set the green tones
			if (ratio <= 0.50)
				g = 1 - (1.5 * ratio);
			else
				g = 0.25;

			// Set the blue tones
			if (ratio >= 0.50)
				b = (ratio - 0.50) / 2;

			// Use the distance to figure out the color
			Gl.glColor4f((float) r, (float) g, (float) b, 1);
		}
		#endregion

		#region Updating
		/// <summary>
		/// Updates the physics engine
		/// </summary>
		public override bool Update(UpdateArgs args)
		{
			// Update it
			State.Physics.Update(args);
			State.JunctionManager.Update(args);

			// Call the parent
			return base.Update(args);
		}
		#endregion
	}
}
