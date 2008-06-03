using Physics2DDotNet;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Implements the player's ship in the universe.
	/// </summary>
	public class Player
	: Module
	{
		#region Properties
		private float enginePower = 10;
		private float containmentPower = 0;

		/// <summary>
		/// Returns the color of this mobile.
		/// </summary>
		public override Color Color
		{
			get
			{
				return Color.FromArgb(255, 0,
					(int) (ContainmentPower / Radius * 255),
					(int) (EnginePower / Radius * 255));
			}
		}

		/// <summary>
		/// Contains the containment power of this module.
		/// </summary>
		public float ContainmentPower
		{
			get { return containmentPower; }
			set { containmentPower = value; }
		}

		/// <summary>
		/// Contains the engine power of this module.
		/// </summary>
		public override float EnginePower
		{
			get { return enginePower; }
			set { enginePower = value; }
		}

		/// <summary>
		/// Contains the radius of the bubble.
		/// </summary>
		public override float Radius
		{
			get
			{
				return enginePower + containmentPower;
			}
		}
		#endregion

		#region Physics
		/// <summary>
		/// Clears out the physics body.
		/// </summary>
		public override void ClearPhysicsBody()
		{
			// Remove our eventing
			PhysicsBody.Collided -= OnBodyCollided;

			// Pass it on.
			base.ClearPhysicsBody();
		}

		/// <summary>
		/// Called when the physics body is created.
		/// </summary>
		protected override void CreatedPhysicsBody(Body body)
		{
			// Connect an event
			body.Collided += OnBodyCollided;

			// Slow down angles
			//body.LinearDamping = 0.95f;
			body.AngularDamping = 0.85f;
			body.Tag = null;
		}

		private void RebuildBody()
		{
			// Kill them in the current engine
			PhysicsBody.Lifetime.IsExpired = true;
			State.Physics.Remove(this);
			
			// Keep the old body and force the mobile to recreate
			// it by getting the new one and then setting the
			// internal state.
			Body oldBody = PhysicsBody;
			ClearPhysicsBody();
			
			Body newBody = PhysicsBody;
			newBody.State.Position.Linear.X =
				oldBody.State.Position.Linear.X;
			newBody.State.Position.Linear.Y =
				oldBody.State.Position.Linear.Y;
			newBody.State.Position.Angular =
				oldBody.State.Position.Angular;
			newBody.State.Velocity.Linear.X =
				oldBody.State.Velocity.Linear.X;
			newBody.State.Velocity.Linear.Y =
				oldBody.State.Velocity.Linear.Y;
			newBody.State.Velocity.Angular =
				oldBody.State.Velocity.Angular;
			
			// Add it in the new engine
			State.Physics.Add(this);
		}
		#endregion

		#region Events
		/// <summary>
		/// Triggered when an object hits the player.
		/// </summary>
		private void OnBodyCollided(object sender, CollisionEventArgs args)
		{
			// If we aren't connected, we don't care
			if (args.Other.Tag == null)
				return;

			// See if we are a module
			Module m = args.Other.Tag as Module;

			if (m == null || m == this)
				return;

			// Add a bit of the radius to ours
			if (m is EngineModule)
				EnginePower += (float) Math.Log(m.Radius);
			if (m is ContainmentModule)
				ContainmentPower += (float) Math.Log(m.Radius);

			// Kill the old one
			args.Other.Tag = null;
			State.Physics.Remove(m);

			// Rebuild the body
			RebuildBody();
		}
		#endregion
	}
}
