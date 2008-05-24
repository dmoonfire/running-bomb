using BooGame.Video;
using MfGames.RunningBomb;
using MfGames.Utility;
using MfGames.Sprite3;
using MfGames.Sprite3.BooWorks;
using System;
using System.Drawing;

namespace RunningBomb
{
	/// <summary>
	/// Implements a game mode that handles the display and updating
	/// of the heads-up display (HUD) and scoring.
	/// </summary>
	public abstract class HudAbstractMode
	: NullGameMode
	{
		#region Constants
		private const float FontSize = 24;
		#endregion

		#region Constructors
		protected HudAbstractMode()
		{
			font = new FreeTypeFont("DejaVuSansMono-Bold.ttf", FontSize);
		}
		#endregion

		#region Drawing and Rendering
		private PointF playerPoint;

		/// <summary>
		/// Contains the center point for the player.
		/// </summary>
		public PointF PlayerPoint
		{
			get { return playerPoint; }
		}

        /// <summary>
		/// Sets and renders the viewport, then adds the HUD data on
		/// top of it.
        /// </summary>
        /// <param name="args"></param>
        public override void Draw(DrawingArgs args)
		{
			// Set up the OpenGL mask

			// Draw the viewport
			DrawViewport(args);

			// Reset the scissors test and render the frame

			// Display statistics
			PointF playerPoint = State.Player.Point;
		    font.Print(String.Format("  Player: {0:N2}x{1:N2} @ {2:N4}",
					playerPoint.X, playerPoint.Y,
					State.Player.PhysicsBody.State.Position.Angular),
				Color.White, 10, 10 + FontSize * 0,
				ContentAlignment.TopLeft);

		    font.Print(String.Format("Velocity: {0:N2}x{1:N2} @ {2:N4}",
					State.Player.PhysicsBody.State.Velocity.Linear.X,
					State.Player.PhysicsBody.State.Velocity.Linear.Y,
					State.Player.PhysicsBody.State.Velocity.Angular),
				Color.White, 10, 10 + FontSize * 1,
				ContentAlignment.TopLeft);
		}

		/// <summary>
		/// Internal function to display the contents of the viewport.
		/// </summary>
		protected abstract void DrawViewport(DrawingArgs args);

		/// <summary>
        /// Sets the size of the game mode (based on the screen).
        /// </summary>
        public override void OnSizeChanged(Size newSize)
		{
			// Set the size properties for layout arrangement
			playerPoint = new PointF(newSize.Width / 2, 3 * newSize.Height / 4);
		}
		#endregion

		#region Score and Statistics
		private BooGameFont font;
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
