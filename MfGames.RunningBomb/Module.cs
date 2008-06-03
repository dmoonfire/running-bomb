using AdvanceMath;
using C5;
using Physics2DDotNet;
using Physics2DDotNet.Joints;
using System;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Represents a module in the game, which is something that can
	/// be attached to a player's ship.
	/// </summary>
	public abstract class Module
	: Mobile
	{
		#region Module Connections
		/// <summary>
		/// Contains the engine power of this module.
		/// </summary>
		public virtual float EnginePower
		{
			get { return 0; }
			set {}
		}
		#endregion

		#region Physics
		/// <summary>
		/// Applies force in the given angle. This is a recursive
		/// function that goes through and will apply force to all the
		/// modules that are capable of doing so.
		/// </summary>
		public virtual void ApplyForce(float angle, float strength)
		{
			// Figure out the angle
			float cos = (float)
				(Math.Cos(angle) * strength * EnginePower);
			float sin = (float)
				(Math.Sin(angle) * strength * EnginePower);

			// Apply the force to this body
			PhysicsBody.ApplyForce(new Vector2D(cos, sin));
		}

		/// <summary>
		/// Called when the physics body is created.
		/// </summary>
		protected override void CreatedPhysicsBody(Body body)
		{
			// Slow down angles
			body.LinearDamping = 0.95f;
		}
		#endregion
	}
}
