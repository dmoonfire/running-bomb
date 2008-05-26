using AdvanceMath;
using BooGame;
using MfGames.Input;
using MfGames.RunningBomb;
using MfGames.Sprite3;
using MfGames.Sprite3.BooWorks;
using System;
using System.Drawing;

namespace RunningBomb
{
	/// <summary>
	/// The primary game mode which handles the input and controller
	/// for the game itself.
	/// </summary>
	public class RunningGameMode
	: DisplayUniverseAbstractMode
	{
		#region Game Mode Changes
		/// <summary>
		/// The game mode is now the top-most game mode.
		/// </summary>
		public override void OnFocused()
		{
			// Set up some events
			base.OnFocused();
			State.JunctionManager.JunctionChanged += OnJunctionChanged;
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
			State.JunctionManager.JunctionChanged -= OnJunctionChanged;
		}
		#endregion

		private bool isCountingDown = true;
		private bool ignoreNextUpdate = true;
		private Vector2D lastPoint;
		private SpeedMeter speed = new SpeedMeter();

		/// <summary>
		/// Triggered when the user presses a key or mouse button.
		/// </summary>
		public override bool OnInputActivated(InputEventArgs args)
		{
			// Make some noise
			//Log.Debug("OnInputActivated: {0}", args.Token);

			// Figure out what to do
			switch (args.Token)
            {
                case InputTokens.Escape: // Quit
					GameModeManager.Push(new MainMenuGameMode());
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
            
            if (Core.InputManager.IsActivated(InputTokens.NumPad7))
				State.Player.PhysicsBody.State.Velocity.Angular += s / 100;
            if (Core.InputManager.IsActivated(InputTokens.NumPad9))
				State.Player.PhysicsBody.State.Velocity.Angular -= s / 100;

            if (Core.InputManager.IsActivated(InputTokens.NumPad6))
				Apply(s, 3);
            if (Core.InputManager.IsActivated(InputTokens.NumPad4))
				Apply(s, 1);
            if (Core.InputManager.IsActivated(InputTokens.NumPad8))
				Apply(s, 4);
            if (Core.InputManager.IsActivated(InputTokens.NumPad2))
				Apply(s, 2);

			if (Core.InputManager.IsActivated(InputTokens.OpenBracket))
				ViewState.Scale += s / 100;
			if (Core.InputManager.IsActivated(InputTokens.CloseBracket))
				ViewState.Scale -= s / 100;

			if (Core.InputManager.IsActivated(InputTokens.NumPad0))
			{
				State.Player.PhysicsBody.State.Velocity.Angular = 0;
				State.Player.PhysicsBody.State.Velocity.Linear.X = 0;
				State.Player.PhysicsBody.State.Velocity.Linear.Y = 0;
			}

			// Update the timer
			UpdateTimer(args);

			// Update the distance
			State.Score.Distance = State.JunctionManager.Junction
				.CalculateDistance(State.Player.Point);

			// Update the speed
			if (!ignoreNextUpdate)
			{
				Vector2D vec = State.Player.PhysicsBody.State.Position.Linear;
				double distance = Vector2D.Distance(vec, lastPoint);
				speed.Update(args, distance);
				State.Score.Speed = (float) speed.Average;
			}

			lastPoint = State.Player.PhysicsBody.State.Position.Linear;

			// Call the parent
			ignoreNextUpdate = false;
			return base.Update(args);
        }

		private void UpdateTimer(UpdateArgs args)
		{
			// Don't bother if we aren't active
			if (!isCountingDown)
				return;

			// See if we should ignore the first one
			if (ignoreNextUpdate)
			{
				// We use this to prevent the update from giving a
				// large one at once.
				return;
			}

			// Calculate the amount of time remaining
			double seconds =
				args.SecondsSinceLastUpdate * State.Score.CountdownMultiplier;
			State.Score.Countdown -= (float) seconds;

			// If we are less than zero, boom.
			if (State.Score.Countdown < 0)
			{
				// Reset it to make it pretty
				State.Score.Countdown = 0;
				isCountingDown = false;

				// Start up the end of game mode on top of us
				GameModeManager.Push(new EndOfGameMode());
			}
		}

		private void Apply(float force, float count)
		{
			float angle = State.Player.Angle;
			float a = angle + (float) (Math.PI / 2) * count;
			float cos = (float) Math.Cos(a) * force * 1000;
			float sin = (float) Math.Sin(a) * force * 1000;

			State.Player.PhysicsBody.ApplyForce(
				new Vector2D(cos, sin));
		}

		#region Events
		/// <summary>
		/// Triggered when the junction changes.
		/// </summary>
		private void OnJunctionChanged(object sender, EventArgs args)
		{
			ignoreNextUpdate = true;
		}
		#endregion
	}
}
