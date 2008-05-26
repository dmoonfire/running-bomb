using AdvanceMath;
using BooGame.Video;
using MfGames.Input;
using MfGames.RunningBomb;
using MfGames.Sprite3;
using MfGames.Sprite3.BooWorks;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using System;
using System.Drawing;

namespace RunningBomb
{
	/// <summary>
	/// Allows for testing of the theme engine.
	/// </summary>
	public class TestThemeGameMode
	: HudAbstractMode
	{
		#region Mode Management
		/// <summary>
		/// Called when this mode is brought to the focus, which
		/// immediately sets up a new game and starts playing.
		/// </summary>
		public override void OnFocused()
		{
			// Set up a new score object, junction and everything
			State.Initialize();
			State.Score.Countdown = Constants.StartingCountdown;
		}
		#endregion

		#region Drawing and Rendering
        /// <summary>
		/// Renders the universe currently loaded into the screen,
		/// then passes the rendering for the HUD to overlay it.
        /// </summary>
        /// <param name="args"></param>
        public override void DrawViewport(
			DrawingArgs args, RectangleF bounds)
		{
			BooGame.Video.Paint.FilledRectangle(bounds, Color.DarkGray);
		}
		#endregion

		#region Input
		/// <summary>
		/// Triggered when the user presses a key or mouse button.
		/// </summary>
		public override bool OnInputActivated(InputEventArgs args)
		{
			// Figure out what to do
			switch (args.Token)
            {
                case InputTokens.Escape: // Quit
                    BooGame.Core.Exit();
					return false;
            }

			GameModeManager.Set(new NewGameMode());
			return true;
		}
		#endregion
    }
}
