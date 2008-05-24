using AdvanceMath;
using BooGame;
using MfGames.Input;
using MfGames.RunningBomb;
using MfGames.Sprite3;
using System;

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
            
            if (Core.InputManager.IsActivated(InputTokens.NumPad4))
				State.Player.PhysicsBody.State.Velocity.Angular += s / 100;
            if (Core.InputManager.IsActivated(InputTokens.NumPad6))
				State.Player.PhysicsBody.State.Velocity.Angular -= s / 100;

            if (Core.InputManager.IsActivated(InputTokens.NumPad9))
				Apply(s, 3);
            if (Core.InputManager.IsActivated(InputTokens.NumPad7))
				Apply(s, 1);
            if (Core.InputManager.IsActivated(InputTokens.NumPad8))
				Apply(s, 4);
            if (Core.InputManager.IsActivated(InputTokens.NumPad5))
				Apply(s, 2);

			if (Core.InputManager.IsActivated(InputTokens.P))
				ViewState.Scale += s / 100;
			if (Core.InputManager.IsActivated(InputTokens.O))
				ViewState.Scale -= s / 100;

			if (Core.InputManager.IsActivated(InputTokens.NumPad0))
			{
				State.Player.PhysicsBody.State.Velocity.Angular = 0;
				State.Player.PhysicsBody.State.Velocity.Linear.X = 0;
				State.Player.PhysicsBody.State.Velocity.Linear.Y = 0;
			}

			// Call the parent
			return base.Update(args);
        }

		private void Apply(float force, float count)
		{
			float angle = State.Player.Angle;
			float a = angle + (float) (Math.PI / 2) * count;
			float cos = (float) Math.Cos(a) * force;
			float sin = (float) Math.Sin(a) * force;

			State.Player.PhysicsBody.State.Velocity.Linear.X += cos;
			State.Player.PhysicsBody.State.Velocity.Linear.Y += sin;
		}
	}
}
