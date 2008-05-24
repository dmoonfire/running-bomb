using C5;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using System;
using System.Threading;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Encapsulates the code for preloading a junction and its
	/// related physics engine to reduce the amount of load it has on
	/// the user's interface.
	/// </summary>
	public class JunctionManagerPreload
	{
		#region Constructors
		/// <summary>
		/// Creates a new preloaded stub and prepares for the
		/// background processing.
		/// </summary>
		public JunctionManagerPreload(Junction junction)
		{
			this.junction = junction;
			this.physics = new Physics();
		}
		#endregion

		#region Preloading
		/// <summary>
		/// Performs the actual preloading process.
		/// </summary>
		public void Preload()
		{
			// Process the junction to make sure we have everything
			junction.BuildConnections();

			// Add all the physics shape to this engine
			foreach (IShape shape in junction.PhysicsShapes)
			{
				Body body = physics.AddImmobile(shape);
				junctionBodies.Add(body);
			}
		}

		/// <summary>
		/// Called from the worker thread, this starts the preload in
		/// a slightly different thread.
		/// </summary>
		private void OnPreload(object state)
		{
			// Set our thread priority to slightly below normal to let
			// the normal game play take priority.
			Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
		}

		/// <summary>
		/// Starts the preloading process for this junction.
		/// </summary>
		public void ThreadPreload()
		{
			// Fire the worker thread
			ThreadPool.QueueUserWorkItem(
				new WaitCallback(OnPreload), junction);
		}
		#endregion

		#region Junction
		private Junction junction;

		/// <summary>
		/// Contains the junction for this preload.
		/// </summary>
		public Junction Junction
		{
			get { return junction; }
		}
		#endregion

		#region Physics
		private Physics physics;
		private LinkedList<Body> junctionBodies = new LinkedList<Body>();

		/// <summary>
		/// Returns a read-only list of bodies for the junctions.
		/// </summary>
		public IList<Body> JunctionBodies
		{
			get { return junctionBodies; }
		}

		/// <summary>
		/// Contains the physics engine for this preload.
		/// </summary>
		public Physics Physics
		{
			get { return physics; }
		}
		#endregion
	}
}
