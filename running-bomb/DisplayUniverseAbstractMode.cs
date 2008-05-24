using MfGames.RunningBomb;
using MfGames.Sprite3;
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
        /// <summary>
		/// Renders the universe currently loaded into the screen,
		/// then passes the rendering for the HUD to overlay it.
        /// </summary>
        /// <param name="args"></param>
        protected override void DrawViewport(DrawingArgs args)
		{
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
		}

		/// <summary>
		/// Triggered when an object hits something.
		/// </summary>
		private void OnBodyCollided(object sender, EventArgs args)
		{
			Log.Info("Collission!");
		}

		private void RenderPlayer(Player player)
		{
			PointF pPoint = player.Point;
			float rotation = player.PhysicsBody.State.Position.Angular *
				Constants.RadiansToDegrees;
			
			BooGame.Video.Paint.Rectangle(
				PlayerPoint.X + pPoint.X - 20,
				PlayerPoint.Y + pPoint.Y - 20, 40, 40,
				new BooGame.Video.ColorF(1, 0, 0),
				rotation,
				PlayerPoint.X + pPoint.X,
				PlayerPoint.Y + pPoint.Y);
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
