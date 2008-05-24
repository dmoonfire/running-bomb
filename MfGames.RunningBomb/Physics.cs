using AdvanceMath;
using MfGames.Sprite3;
using MfGames.Utility;
using Physics2DDotNet;
using Physics2DDotNet.Detectors;
using Physics2DDotNet.PhysicsLogics;
using Physics2DDotNet.Solvers;

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

			// Set up the gravity
			AdvanceMath.Vector2D gravityVector =
				new AdvanceMath.Vector2D(0f, 0.05f);
			PhysicsLogic gravity =
				new GravityField(gravityVector, new Lifespan());
			engine.AddLogic(gravity);
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
