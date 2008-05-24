using C5;
using Gpc;
using MfGames.Utility;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Encapsulates the code for managing junctions, including
	/// loading and switching elements within the physics engine,
	/// adding junction elements to the physics, and swapping out
	/// junctions.
	/// </summary>
	public class JunctionManager
	{
		#region Junction
		private Junction junction;

		/// <summary>
		/// Setting this junction sets the various elements of the
		/// game to manage the junction, including adding it to the
		/// physics and swapping out any junction already loaded.
		/// </summary>
		public Junction Junction
		{
			get { return junction; }
			set
			{
				// See if we have an old junction
				if (junction != null)
				{
					// Swap out the old junction
					RemoveJunctionPhysics();
				}

				// Sets the new junction
				junction = value;

				// We are done if we are null
				if (junction == null)
					return;

				// Add ourselves to the physics
				AddJunctionPhysics();
			}
		}
		#endregion

		#region Physics
		private LinkedList<Body> junctionBodies = new LinkedList<Body>();

		/// <summary>
		/// Returns a read-only list of bodies for the junctions.
		/// </summary>
		public IList<Body> JunctionBodies
		{
			get { return junctionBodies; }
		}

		/// <summary>
		/// Adds the junctions physics to this object.
		/// </summary>
		private void AddJunctionPhysics()
		{
			foreach (IShape shape in junction.PhysicsShapes)
			{
				Body body = State.Physics.AddImmobile(shape);
				junctionBodies.Add(body);
			}
		}

		/// <summary>
		/// Removes the junction data from the physics layer.
		/// </summary>
		private void RemoveJunctionPhysics()
		{
			// Go through the list
			foreach (Body body in junctionBodies)
				body.Lifetime.IsExpired = true;

			// Clear it since it will remove itself
			junctionBodies.Clear();
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
					log = new Log(typeof(JunctionManager));

				return log;
			}
		}
		#endregion
	}
}
