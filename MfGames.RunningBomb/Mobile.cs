using MfGames.Utility;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Represents some mobile, or physics-driven, object within the
	/// game. This can be the player ship, clutter, or anything else
	/// that can be reasonably shot, moved, or struck.
	/// </summary>
	public class Mobile
	{
		#region Properties
		/// <summary>
		/// Returns the angle of the mobile.
		/// </summary>
		public float Angle
		{
			get { return PhysicsBody.State.Position.Angular; }
		}

		/// <summary>
		/// Contains the relative point of this mobile to the root
		/// junction.
		/// </summary>
		public PointF Point
		{
			get
			{
				// Wrap the state in a point
				return new PointF(
					Convert.ToInt32(PhysicsBody.State.Position.Linear.X),
					Convert.ToInt32(PhysicsBody.State.Position.Linear.Y));
			}
			set
			{
				// Just set the straight positions
				PhysicsBody.State.Position.Linear.X = value.X;
				PhysicsBody.State.Position.Linear.Y = value.Y;
			}
		}
		#endregion

		#region Physics Information
		private Body physicsBody;

		public Body PhysicsBody
		{
			get
			{
				// If we have a body, return it
				if (physicsBody != null)
					return physicsBody;

				// Create the body
				physicsBody = new Body(
					new PhysicsState(),
					new PolygonShape(
						VertexHelper.CreateRectangle(40, 40), 2),
					3,
					new Coefficients(0.75f, 0.5f),
					new Lifespan());
				physicsBody.IsCollidable = true;
				return physicsBody;
			}
		}

		/// <summary>
		/// Clearws out the physics body.
		/// </summary>
		public void ClearPhysicsBody()
		{
			physicsBody = null;
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
