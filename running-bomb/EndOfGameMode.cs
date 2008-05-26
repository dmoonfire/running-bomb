using AdvanceMath;
using BooGame;
using BooGame.Video;
using MfGames.Input;
using MfGames.RunningBomb;
using MfGames.Sprite3;
using MfGames.Sprite3.BooWorks;
using RunningBomb.Themes;
using System;
using System.Drawing;

namespace RunningBomb
{
	/// <summary>
	/// The game over mode which handles scoring and display.
	/// </summary>
	public class EndOfGameMode
	: NullGameMode
	{
		/// <summary>
		/// Triggered when the user presses a key or mouse button.
		/// </summary>
		public override bool OnInputActivated(InputEventArgs args)
		{
			// Switch to the main mode
			GameModeManager.Set(new MainMenuGameMode());

			// We are done processing modes
			return false;
		}

		#region Drawing
		/// <summary>
		/// Draws the explosion across the screen.
        /// </summary>
        /// <param name="args"></param>
        public override void Draw(DrawingArgs args)
		{
			// Dark-screen everything
			Paint.FilledRectangle(0, 0,
				VideoManager.ScreenSize.Width,
				VideoManager.ScreenSize.Height,
				Color.FromArgb(222, 64, 0, 0));

			// Get the saved text
			ThemeFont stf0 = ThemeManager.Theme.Fonts["end-of-game"];
			ThemeFont stf1 = ThemeManager.Theme.Fonts["stats"];
			float height = ((FreeTypeFont) stf1.Font).LineHeight;
			float middleY = VideoManager.ScreenSize.Height / 2;

			stf0.Font.Print("Game Over",
				Color.White,
				VideoManager.ScreenSize.Width / 2,
				1 * VideoManager.ScreenSize.Height / 4,
				stf0.Alignment);

			stf1.Font.Print("Population Saved:",
				Color.White,
				VideoManager.ScreenSize.Width / 2,
				middleY,
				stf1.Alignment);

			stf1.Font.Print(State.Score.PopulationSaved.ToString("N0"),
				Color.White,
				VideoManager.ScreenSize.Width / 2,
				middleY + height,
				stf1.Alignment);
		}
		#endregion

		#region Updating the Game
		/// <summary>
		/// Processes the events for the keyboard down before passing
		/// them on.
		/// </summary>
        public override bool Update(UpdateArgs args)
        {
			// We don't allow anything else to update
			return false;
		}
		#endregion
	}
}
