using C5;
using AdvanceMath;
using Gpc;
using MfGames.Sprite3;
using MfGames.Utility;
using Physics2DDotNet;
using Physics2DDotNet.Detectors;
using Physics2DDotNet.PhysicsLogics;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Solvers;
using System;
using System.Drawing;
using Vector2D = AdvanceMath.Vector2D;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Encapsulates all the functionality of the physics engine.
	/// </summary>
	public class Physics
	{
		#region Constructor
		/// <summary>
		/// Creates the physics object used in the game.
		/// </summary>
		public Physics()
		{
			// Set up the basic engine
			engine = new PhysicsEngine();

			// Set up the collision detector
			engine.BroadPhase = new SweepAndPruneDetector();
			//engine.BroadPhase = new SelectiveSweepDetector();
			//engine.BroadPhase = new BruteForceDetector();

			// Set up the solver
			SequentialImpulsesSolver solver = new SequentialImpulsesSolver();
			engine.Solver = solver;
			solver.Iterations = 12;
			solver.SplitImpulse = true;
			//solver.BiasFactor = 0.7f;
			//solver.AllowedPenetration = 0f;
		}
		#endregion

		#region Physics2D
		PhysicsEngine engine;

		/// <summary>
		/// Contains the currently allowed engine.
		/// </summary>
		public PhysicsEngine Engine
		{
			get { return engine; }
		}
		#endregion

		#region Update
		private float speed = 0.0125f;
		private bool enabled = true;

		/// <summary>
		/// Updates the physicsl engine.
		/// </summary>
		public void Update(UpdateArgs args)
		{
			// Don't bother if we aren't enabled
			if (!enabled)
				return;

			// Get the speed
			float s = (float) args.SecondsSinceLastUpdate;

			// Update the engine
			engine.Update(speed, s);
		}
		#endregion

		#region List Operations
		private LinkedList<Mobile> mobiles = new LinkedList<Mobile>();

		/// <summary>
		/// Contains a list of mobiles within the physics engine.
		/// </summary>
		public IList<Mobile> Mobiles
		{
			get { return mobiles; }
		}

		/// <summary>
		/// Adds a mobile object into the physics world.
		/// </summary>
		public void Add(Mobile mobile)
		{
			// See if we already have it
			if (mobile == null || mobiles.Contains(mobile))
				return;

			// Add this mobile to the engine
			Log.Debug("Adding mobile: {0}", mobile);
			engine.AddBody(mobile.PhysicsBody);
			mobiles.Add(mobile);
		}

		/// <summary>
		/// Adds a polygon as an immobile block.
		/// </summary>
		public Body AddImmobile(IShape shape)
		{
			// Create the object
			Body body = new Body(
				new PhysicsState(),
				shape,
				Single.PositiveInfinity,
				new Coefficients(0.75f, 0.5f),
				new Lifespan());
			body.IsCollidable = true;

			// Add it and return the results
			engine.AddBody(body);
			return body;
		}

		/// <summary>
		/// Retrieves all mobiles within a certain distance of the
		/// given point.
		/// </summary>
		public IList<Mobile> GetMobiles(PointF point, float distance)
		{
			// Go through the mobiles and find what's within range
			LinkedList<Mobile> list = new LinkedList<Mobile>();

			foreach (Mobile m in mobiles)
			{
				if (Geometry.CalculateDistance(point, m.Point) <= distance)
					list.Add(m);
			}

			// Return the results
			return list;
		}

		/// <summary>
		/// Removes the mobile from the internal management list.
		/// </summary>
		public void Remove(Mobile mobile)
		{
			mobiles.Remove(mobile);
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
