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
	public abstract class Mobile
	{
		#region Properties
		private float radius = 10;

		/// <summary>
		/// Returns the angle of the mobile.
		/// </summary>
		public float Angle
		{
			get { return PhysicsBody.State.Position.Angular; }
		}

		/// <summary>
		/// Gets the area of a circle.
		/// </summary>
		public float Area
		{
			get
			{
				// Returns the area of the a circle or PI * r**2.
				return Constants.PI * radius * radius;
			}
		}

		/// <summary>
		/// Returns the color of this mobile.
		/// </summary>
		public abstract Color Color { get; }

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

		/// <summary>
		/// Contains the radius of the bubble.
		/// </summary>
		public virtual float Radius
		{
			get { return radius; }
			set
			{
				if (value <= 0)
					throw new Exception("Cannot set an empty radius");

				radius = value;
			}
		}
		#endregion

		#region Physics Information
		private Body physicsBody;

		/// <summary>
		/// Gets the physics body for this mobile.
		/// </summary>
		public virtual Body PhysicsBody
		{
			get
			{
				// If we have a body, return it
				if (physicsBody != null)
					return physicsBody;

				// Create the body
				physicsBody = new Body(
					new PhysicsState(),
					PhysicsShape,
					0.0001f,
					new Coefficients(0.95f, 0.75f),
					new Lifespan());
				physicsBody.IsCollidable = true;
				physicsBody.Tag = this;
				CreatedPhysicsBody(physicsBody);
				return physicsBody;
			}
		}

		/// <summary>
		/// Creates the shape of the bubble module.
		/// </summary>
		protected virtual IShape PhysicsShape
		{
			get
			{
				return new PolygonShape(VertexHelper
					.CreateCircle(Radius, (int) Radius + 6), Radius);
			}
		}

		/// <summary>
		/// Clearws out the physics body.
		/// </summary>
		public virtual void ClearPhysicsBody()
		{
			physicsBody = null;
		}

		/// <summary>
		/// Called when the physics body is created.
		/// </summary>
		protected virtual void CreatedPhysicsBody(Body body)
		{
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
