using BooGame;
using C5;
using Log = MfGames.Utility.Log;
using MfGames.Input;
using MfGames.RunningBomb;
using MfGames.Utility;
using MfGames.Sprite3;
using MfGames.Sprite3.BooWorks;
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
	public class TextScrollGameMode
	: NullGameMode, IThemeCallback
	{
		#region Constructors
		/// <summary>
		/// Constructs a text display mode with a given embedded
		/// resource.
		/// </summary>
		public TextScrollGameMode(string path, IGameMode nextMode)
		{
			// Save the next mode
			this.next = nextMode;

			// Process the assembly manifest and convert it into the
			// scrolling structure with the various line encoding.
			Assembly assembly = GetType().Assembly;

			using (Stream s = assembly.GetManifestResourceStream(path))
			{
				// Wrap in a stream reader
				StreamReader sr = new StreamReader(s);

				// Go through the lines
				string line = null;

				while ((line = sr.ReadLine()) != null)
				{
					// Create the line
					TextScrollLine tsl = new TextScrollLine();

					// Check for header flags
					if (line.StartsWith("=1 "))
					{
						tsl.LineType = LineType.Heading1;
						tsl.Line = line.Substring(3);
					}
					else if (line.StartsWith("=2 "))
					{
						tsl.LineType = LineType.Heading1;
						tsl.Line = line.Substring(3);
					}
					else if (line.StartsWith("=3 "))
					{
						tsl.LineType = LineType.Heading3;
						tsl.Line = line.Substring(3);
					}
					else
					{
						tsl.Line = line;
					}

					// Add the line
					lines.Add(tsl);
				}
			}
		}
		#endregion

		#region Properties
		private IGameMode next;
		private LinkedList<TextScrollLine> lines =
			new LinkedList<TextScrollLine>();
		private bool hasCalculatedLineHeights = false;
		private float lineRate = 2;
		private float screenRate = 0;
		private float position;
		private bool halt = false;
		#endregion

		#region Game Modes
		/// <summary>
		/// Set the next game mode as stored.
		/// </summary>
		private void SwitchToNext()
		{
			// If we are null, then just pop, otherwise set the mode.
			if (next == null)
				GameModeManager.Pop();
			else
				GameModeManager.Set(next);
		}
		#endregion

		#region Drawing and Rendering
		/// <summary>
		/// Figures out the line heights once and stores them.
		/// </summary>
		private void CalculateLineHeights(Theme theme)
		{
			// Get the fonts
			ThemeFont h1 = theme.Fonts["h1"];
			ThemeFont h2 = theme.Fonts["h2"];
			ThemeFont h3 = theme.Fonts["h3"];
			ThemeFont p = theme.Fonts["p"];

			// Go through the lines
			float total = 0;

			foreach (TextScrollLine tsl in lines)
			{
				// Keep track of the current line's height
				ThemeFont current = p;

				switch (tsl.LineType)
				{
					case LineType.Heading1: current = h1; break;
					case LineType.Heading2: current = h2; break;
					case LineType.Heading3: current = h3; break;
				}

				// Set the line
				tsl.Font = current;
				tsl.Height =
					((BooGame.Video.FreeTypeFont) current.Font).LineHeight;

				total += tsl.Height;
				tsl.Position = total;
			}

			// We are done calculating
			hasCalculatedLineHeights = true;
		}

        /// <summary>
		/// Renders the screen by passing it ot the theme engine.
        /// </summary>
        /// <param name="args"></param>
        public override void Draw(DrawingArgs args)
		{
			// Dark-screen everything
			BooGame.Video.Paint.FilledRectangle(0, 0,
				VideoManager.ScreenSize.Width,
				VideoManager.ScreenSize.Height,
				Color.FromArgb(255, 0, 0, 0));

			// Get the theme and render using it
			Theme theme = ThemeManager.Theme;
			theme.Render(Theme.TextLayout, this, args);
		}

		/// <summary>
		/// Internal function to display the contents of the viewport.
		/// </summary>
		public void DrawViewport(DrawingArgs args, RectangleF bounds)
		{
			// See if we have our line heights calculated
			if (!hasCalculatedLineHeights)
				CalculateLineHeights(ThemeManager.Theme);

			// Set the rendering rate so lineRate lines advance every
			// second.
			screenRate =
				((BooGame.Video.FreeTypeFont) ThemeManager.Theme.Fonts["p"]
					.Font).LineHeight * lineRate;

			// Figure out what the top line would be, assuming the
			// position is at the bottom of the screen since we scroll
			// up.
			float topPosition = position - bounds.Height;

			// Render out all the text
			bool displayedLines = false;

			foreach (TextScrollLine tsl in lines)
			{
				// See if the line would be visible
				if (tsl.Position + tsl.Height < topPosition)
					// Above the screen
					continue;

				if (tsl.Position > position)
					// Below the screen
					continue;

				// Display the font
				displayedLines = true;
				tsl.Font.Font.Print(tsl.Line,
					Color.White,
					bounds.X, tsl.Position - topPosition,
					tsl.Font.Alignment);
			}

			// If we haven't displayed a line, we are done
			if (!displayedLines && position > lines.Count * 10)
				SwitchToNext();
		}
		#endregion

		#region Updating
		/// <summary>
		/// Processes the events for the keyboard down before passing
		/// them on.
		/// </summary>
        public override bool Update(UpdateArgs args)
        {
			// Figure out the time
			float s = (float) args.SecondsSinceLastUpdate;

			// Figure out the mode
            if (Core.InputManager.IsActivated(InputTokens.Up))
			{
				halt = false;
				lineRate -= 1;
			}

            if (Core.InputManager.IsActivated(InputTokens.Down))
			{
				halt = false;
				lineRate += 1;
			}

            if (Core.InputManager.IsActivated(InputTokens.Space))
			{
				halt = !halt;
			}

            if (Core.InputManager.IsActivated(InputTokens.Escape))
			{
				SwitchToNext();
				return false;
			}

			// Advance the position
			if (!halt)
				position += screenRate * s;

			if (position < 0)
				position = 0;
            
			// Finish up with the base
			return base.Update(args);
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
