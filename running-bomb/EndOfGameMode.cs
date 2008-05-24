using AdvanceMath;
using BooGame;
using MfGames.Input;
using MfGames.RunningBomb;
using MfGames.Sprite3;
using MfGames.Sprite3.BooWorks;
using System;

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
			// Figure out what to do
			switch (args.Token)
            {
                case InputTokens.Escape: // Quit
                    Core.Exit();
                    break;
            }

			// We are done processing modes
			return false;
		}

		/// <summary>
		/// Processes the events for the keyboard down before passing
		/// them on.
		/// </summary>
        public override bool Update(UpdateArgs args)
        {
			// We don't allow anything else to update
			return false;
		}
	}
}
