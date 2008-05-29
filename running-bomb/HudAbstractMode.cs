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
			// Reset our clear color
			BooGame.Core.ClearColor = 
				new BooGame.Video.ColorF((float) State.Score.Stress / 2, 0, 0);

			// Get the theme and render using it
			Theme theme = ThemeManager.Theme;
			theme.Render(Theme.UniverseLayout, this, args);
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
