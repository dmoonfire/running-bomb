using AdvanceMath;
using BooGame;
using MfGames.Input;
using MfGames.RunningBomb;
using MfGames.Sprite3;

namespace RunningBomb
{
	/// <summary>
	/// The primary game mode which handles the input and controller
	/// for the game itself.
	/// </summary>
	public class RunningGameMode
	: DisplayUniverseAbstractMode
	{
		/// <summary>
		/// Triggered when the user presses a key or mouse button.
		/// </summary>
		public override bool OnInputActivated(InputEventArgs args)
		{
			// Make some noise
			Log.Debug("OnInputActivated: {0}", args.Token);

			// Figure out what to do
			switch (args.Token)
            {
                case InputTokens.Escape: // Quit
                    Core.Exit();
                    break;
            }

			// Allow it to keep passing it on
			return true;
		}

		/// <summary>
		/// Processes the events for the keyboard down before passing
		/// them on.
		/// </summary>
        public override bool Update(UpdateArgs args)
        {
			float s = (float) args.SecondsSinceLastUpdate * 100;
            
            if (Core.InputManager.IsActivated(InputTokens.A))
				State.Player.PhysicsBody.State.Velocity.Angular += s / 100;
            if (Core.InputManager.IsActivated(InputTokens.S))
				State.Player.PhysicsBody.State.Velocity.Angular -= s / 100;

            if (Core.InputManager.IsActivated(InputTokens.Right))
				State.Player.PhysicsBody.State.Velocity.Linear.X += s;
            if (Core.InputManager.IsActivated(InputTokens.Left))
				State.Player.PhysicsBody.State.Velocity.Linear.X -= s;
            if (Core.InputManager.IsActivated(InputTokens.Up))
				State.Player.PhysicsBody.State.Velocity.Linear.Y -= s;
            if (Core.InputManager.IsActivated(InputTokens.Down))
				State.Player.PhysicsBody.State.Velocity.Linear.Y += s;

			// Call the parent
			return base.Update(args);
        }
	}
}
