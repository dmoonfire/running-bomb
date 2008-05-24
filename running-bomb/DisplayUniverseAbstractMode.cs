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
		private Player pp;

		#region Drawing and Rendering
		private Matrix2x3 matrix;
		private PointF playerPoint;

		/// <summary>
		/// Creates the transformation matrix used by the rendering.
		/// </summary>
		private void AdjustMatrix()
		{
			// Adjust for the player's angle
			playerPoint = State.Player.Point;

			// Figure out the rotation around the player
			Matrix2x3 translate1 = Matrix2x3.FromTranslate2D(
				new Vector2D(-playerPoint.X, -playerPoint.Y));
			Matrix2x3 rotate = Matrix2x3.FromRotationZ(-State.Player.Angle);

			// Set the matrix
			matrix = rotate * translate1;
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

			// Figure out the coordinates for the player
			RenderPlayer(State.Player);

			// Add a second player
			if (pp == null)
			{
				State.Player.PhysicsBody.Collided += OnBodyCollided;
				pp = new Player();
				pp.Point = new PointF(50, 50);
				pp.PhysicsBody.Collided += OnBodyCollided;
				pp.PhysicsBody.IsCollidable = true;
				State.Physics.Add(pp);
			}

			RenderPlayer(pp);

			// If we need it, draw the outline of the physics object
#if DEBUG
			RenderJunctions();
#endif
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

		private void RenderPlayer(Player player)
		{
			RenderPolygon((PolygonShape) player.PhysicsBody.Shape,
				player.PhysicsBody.Matrices.ToWorld);
			/*
			// Get some of the values we need
			PointF p = player.Point;
			Color color = Color.Red;

			// Get the starting points
			PointF p1 = ToScalePoint(p.X - 20, p.Y - 20, player.Angle);
			PointF p2 = ToScalePoint(p.X - 20, p.Y + 20, player.Angle);
			PointF p3 = ToScalePoint(p.X + 20, p.Y + 20, player.Angle);
			PointF p4 = ToScalePoint(p.X + 20, p.Y - 20, player.Angle);

			// Draw some lines
			Paint.Line(p1.X, p1.Y, p2.X, p2.Y, color);
			Paint.Line(p2.X, p2.Y, p3.X, p3.Y, color);
			Paint.Line(p3.X, p3.Y, p4.X, p4.Y, color);
			Paint.Line(p4.X, p4.Y, p1.X, p1.Y, Color.Yellow);
			*/
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

			// Call the parent
			return base.Update(args);
		}
		#endregion
	}
}
