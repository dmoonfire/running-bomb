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
		/// <summary>
		/// Adds a mobile object into the physics world.
		/// </summary>
		public void Add(Mobile mobile)
		{
			// Add this mobile to the engine
			Log.Debug("Adding mobile: {0}", mobile);
			engine.AddBody(mobile.PhysicsBody);
		}

		/// <summary>
		/// Adds a polygon as an immobile block.
		/// </summary>
		public Body AddImmobile(IPoly poly)
		{
			// Create a polygon shape as vectors
			LinkedList<Vector2D> vectors = new LinkedList<Vector2D>();

			for (int i = 0; i < poly.PointCount; i++)
			{
				// Get the coordinates
				float x = (float) poly.GetX(i);
				float y = (float) poly.GetY(i);

				// Create the vector
				vectors.Add(new Vector2D(x, y));
			}

			// Convert it into a physics2d polygon shape
			Vector2D [] array = vectors.ToArray();
			IShape shape = new PolygonShape(array, 1f);

			// Create the object
			Body body = new Body(
				new PhysicsState(),
				shape,
				10,
				new Coefficients(0.75f, 0.5f),
				new Lifespan());
			body.IsCollidable = true;

			// Add it and return the results
			engine.AddBody(body);
			return body;
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
