using BooGame.Video;
using MfGames.RunningBomb;
using MfGames.Utility;
using MfGames.Sprite3;
using MfGames.Sprite3.BooWorks;
using RunningBomb.Themes;
using System;
using System.Drawing;

namespace RunningBomb
{
	/// <summary>
	/// Implements a game mode that handles the display and updating
	/// of the heads-up display (HUD) and scoring.
	/// </summary>
	public abstract class HudAbstractMode
	: NullGameMode, IThemeCallback
	{
		#region Drawing and Rendering
        /// <summary>
		/// Sets and renders the viewport, then adds the HUD data on
		/// top of it.
        /// </summary>
        /// <param name="args"></param>
        public override void Draw(DrawingArgs args)
		{
			// Get the theme and render using it
			Theme theme = ThemeManager.Theme;
			theme.Render(Theme.DisplayLayout, this, args);

			/*
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

			// Format the time
		    font.Print("Countdown: " + State.Score.CountdownString,
				Color.White, 10, 10 + FontSize * 2,
				ContentAlignment.TopLeft);
			*/
		}

		/// <summary>
		/// Internal function to display the contents of the viewport.
		/// </summary>
		public abstract void DrawViewport(
			DrawingArgs args, RectangleF bounds);
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
