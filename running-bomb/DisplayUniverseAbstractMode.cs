using AdvanceMath;
using BooGame.Video;
using MfGames.RunningBomb;
using MfGames.Sprite3;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using System;
using System.Drawing;

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

		/// <summary>
		/// Creates the transformation matrix used by the rendering.
		/// </summary>
		private void AdjustMatrix()
		{
			// Adjust for the player's angle
			playerPoint = State.Player.Point;
		}

        /// <summary>
		/// Renders the universe currently loaded into the screen,
		/// then passes the rendering for the HUD to overlay it.
        /// </summary>
        /// <param name="args"></param>
        protected override void DrawViewport(DrawingArgs args)
		{	
			// Set up the matrix
			AdjustMatrix();

			// If we need it, draw the outline of the physics object
#if DEBUG
			RenderJunctions();
#endif

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
				
				Paint.Line(p1.X, p1.Y, p2.X, p2.Y, Color.Gray);
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
			x += PlayerPoint.X;
			y += PlayerPoint.Y;

			// Return the results
			return new PointF(x, y);
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
