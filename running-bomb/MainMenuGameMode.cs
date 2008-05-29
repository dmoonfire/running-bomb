using BooGame;
using BooGame.Video;
using C5;
using Log = MfGames.Utility.Log;
using MfGames.Input;
using MfGames.RunningBomb;
using MfGames.Utility;
using MfGames.Sprite3;
using MfGames.Sprite3.BooWorks;
using RunningBomb.Audio;
using RunningBomb.Themes;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace RunningBomb
{
	/// <summary>
	/// Implements a basic mode that just displays text for the user.
	/// </summary>
	public class MainMenuGameMode
	: NullGameMode, IThemeCallback
	{
		#region Constructors
		/// <summary>
		/// Constructs a text display mode with a given embedded
		/// resource.
		/// </summary>
		public MainMenuGameMode()
		{
			// Populate the list
			items.Add("New Game");
			items.Add("Help");
			items.Add("Story");
			items.Add("Credits");
			items.Add("Exit Game");
		}
		#endregion

		#region Game Mode Changes
		/// <summary>
		/// The game mode is now the top-most game mode.
		/// </summary>
		public override void OnFocused()
		{
			// Do the normal process
			base.OnFocused();

			// See if we aren't on top
			if (GameModeManager.GameModes.Count > 1)
			{
				index = items.Count;
				items.Add("Resume");
			}
			else
			{
				// Reset our music
				AudioManager.ResetBackgroundSamples();
				Core.ClearColor = new ColorF(Color.Black);
			}
		}

		/// <summary>
		/// This method is called when the game mode is no longer the
		/// top-most one, either by another game pushing it on the
		/// mode or it being removed. Any changes to the game stack
		/// will be done before this is called.
		/// </summary>
		public override void OnUnfocused()
		{
			// Clean up our events
			base.OnUnfocused();
		}
		#endregion

		#region Properties
		private LinkedList<string> items = new LinkedList<string>();
		private int index = 0;
		#endregion

		#region Drawing and Rendering
        /// <summary>
		/// Renders the screen by passing it ot the theme engine.
        /// </summary>
        /// <param name="args"></param>
        public override void Draw(DrawingArgs args)
		{
			// Dark-screen everything
			Paint.FilledRectangle(0, 0,
				VideoManager.ScreenSize.Width,
				VideoManager.ScreenSize.Height,
				Color.FromArgb(222, 0, 0, 0));

			// Get the theme and render using it
			Theme theme = ThemeManager.Theme;
			theme.Render(Theme.TitleLayout, this, args);
		}

		/// <summary>
		/// Internal function to display the contents of the viewport.
		/// </summary>
		public void DrawViewport(DrawingArgs args, RectangleF bounds)
		{
			// Figure out the center of our viewport
			PointF center = new PointF(
				(bounds.Left + bounds.Right) / 2,
				(bounds.Top + bounds.Bottom) / 2);

			// Put the current one right in the middle
			ThemeFont tf = ThemeManager.Theme.Fonts["menu"];
			float height = ((BooGame.Video.FreeTypeFont) tf.Font).LineHeight;

			tf.Font.Print(items[index],
				Color.White,
				center.X, center.Y,
				tf.Alignment);

			// Add the item above and below it
			ThemeFont stf = ThemeManager.Theme.Fonts["smallmenu"];

			stf.Font.Print(items[GetIndex(index - 1)],
				Color.FromArgb(128, 255, 255, 255),
				center.X, center.Y - 55,
				stf.Alignment);
			stf.Font.Print(items[GetIndex(index + 1)],
				Color.FromArgb(128, 255, 255, 255),
				center.X, center.Y + height,
				stf.Alignment);
		}

		/// <summary>
		/// Retrieves a normalized index that wraps around.
		/// </summary>
		private int GetIndex(int ndx)
		{
			if (ndx >= 0 && ndx < items.Count)
				return ndx;

			while (ndx >= items.Count)
				ndx -= items.Count;

			while (ndx < 0)
				ndx += items.Count;

			return ndx;
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
				case InputTokens.Up:
				case InputTokens.NumPad8:
					index = GetIndex(index - 1);
					break;
				case InputTokens.Down:
				case InputTokens.NumPad2:
					index = GetIndex(index + 1);
					break;
				case InputTokens.Right:
				case InputTokens.Enter:
				case InputTokens.Space:
					switch (index)
					{
						case 0:
							GameModeManager.Set(new NewGameMode());
							break;
						case 1:
							GameModeManager.Push(new TextScrollGameMode(
									"help.txt", null));
							break;
						case 2:
							GameModeManager.Push(new TextScrollGameMode(
									"story.txt", null));
							break;
						case 3:
							GameModeManager.Push(new TextScrollGameMode(
									"credits.txt", null));
							break;
						case 4:
							Core.Exit();
							break;
						case 5:
							// Pop off the mode and resume the game
							GameModeManager.Pop();
							break;
					}
					break;
                case InputTokens.Escape: // Quit
                    Core.Exit();
                    break;
            }

			// Allow it to keep passing it on
			return true;
		}
		#endregion

		#region Updating
		/// <summary>
		/// Processes the events for the keyboard down before passing
		/// them on.
		/// </summary>
        public override bool Update(UpdateArgs args)
        {
			// Update the sound manager
			AudioManager.Update(args);

			// Don't let it update since we might be playing a game
			return false;
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
