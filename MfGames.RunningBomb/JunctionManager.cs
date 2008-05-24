using C5;
using Gpc;
using MfGames.Sprite3;
using MfGames.Utility;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using System;
using System.Drawing;
using System.Threading;

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
		#region Constructors
		/// <summary>
		/// Prepares the threads used by the junction manager.
		/// </summary>
		static JunctionManager()
		{
			// Set up the thread pool
			ThreadPool.SetMaxThreads(30, 45);
		}
		#endregion

		#region Junction
		private Junction junction;
		private JunctionManagerPreload preload;
		private HashDictionary<Junction,JunctionManagerPreload>
			preloadedJunctions =
			new HashDictionary<Junction,JunctionManagerPreload>();

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
				// If we have a junction, then preload the data
				preload = null;

				if (value != null && !preloadedJunctions.Contains(value))
				{
					// Preload the junction
					preload = new JunctionManagerPreload(value);
					preloadedJunctions[value] = preload;
				}
				else if (value != null)
				{
					preload = preloadedJunctions[value];
				}

				// For the junction to load
				if (preload != null)
					preload.Preload();

				// If we have two junctions, then we need to move
				// stuff over between the two.
				if (junction != null && value != null)
					SwitchJunctionContents(junction, value);

				// Clean up the old junction loading
				if (junction != null)
				{
					// Remove the physics engine
					State.Physics = null;
				}

				// Sets the new junction
				junction = value;

				// We are done if we are null
				if (junction == null)
					return;

				// Set up our new physics engine in the state
				State.Physics = preload.Physics;

				// Fire up the thread pool to process the child
				// junctions
				//UpdateChildJunctions();
			}
		}

		/// <summary>
		/// Returns a read-only list of bodies for the junctions or
		/// null if there is none loaded.
		/// </summary>
		public IList<Body> JunctionBodies
		{
			get
			{
				if (preload == null)
					return null;
				
				return preload.JunctionBodies;
			}
		}

		/// <summary>
		/// Swaps the contents of one junction with another, moving as
		/// appropriate.
		/// </summary>
		private void SwitchJunctionContents(
			Junction oldJunction, Junction newJunction)
		{
			// Get our segment
			Segment s = oldJunction.GetSegment(newJunction);

			if (s == null)
			{
				throw new Exception(
					"Cannot handle switching to non-continous segments");
			}

			PointF translate = s.ChildJunctionPoint;

			// Get a list of mobiles, including the player, around the
			// player.
			JunctionManagerPreload oldPreload = preloadedJunctions[oldJunction];
			JunctionManagerPreload newPreload = preloadedJunctions[newJunction];
			IList<Mobile> list = oldPreload.Physics.GetMobiles(
				State.Player.Point, Constants.OverlapConnectionDistance);

			foreach (Mobile m in list)
			{
				// Kill them in the current engine
				m.PhysicsBody.Lifetime.IsExpired = true;
				oldPreload.Physics.Remove(m);

				// Keep the old body and force the mobile to recreate
				// it by getting the new one and then setting the
				// internal state.
				Body oldBody = m.PhysicsBody;
				m.ClearPhysicsBody();

				Body newBody = m.PhysicsBody;
				newBody.State.Position.Linear.X =
					oldBody.State.Position.Linear.X - translate.X;
				newBody.State.Position.Linear.Y =
					oldBody.State.Position.Linear.Y - translate.Y;
				newBody.State.Position.Angular =
					oldBody.State.Position.Angular;
				newBody.State.Velocity.Linear.X =
					oldBody.State.Velocity.Linear.X;
				newBody.State.Velocity.Linear.Y =
					oldBody.State.Velocity.Linear.Y;
				newBody.State.Velocity.Angular =
					oldBody.State.Velocity.Angular;

				// Add it in the new engine
				newPreload.Physics.Add(m);
			}
		}

		/// <summary>
		/// This method is called to update the player's position and
		/// to potentially switch junctions.
		/// </summary>
		public void Update(UpdateArgs args)
		{
			// Ignore if we don't have a player or a junctin
			if (State.Player == null || junction == null)
				return;

			// Go through the segments and find the distance
			PointF playerPoint = State.Player.Point;

			LinkedList<Junction> preload = new LinkedList<Junction>();
			LinkedList<Junction> build = new LinkedList<Junction>();

			foreach (Segment s in junction.Segments)
			{
				// Get our distance from the player's position
				float distance = Geometry
					.CalculateDistance(playerPoint, s.ChildJunctionPoint);

				// See if we are in switch range
				if (distance <= Constants.JunctionSwitchDistance)
				{
					// Switching junctions!
					Junction = s.ChildJunction;
					return;
				}

				// See if we are in preload distance and we haven't
				// already started the preload.
				if (distance <= Constants.OverlapConnectionDistance &&
					!preloadedJunctions.Contains(s.ChildJunction))
				{
					preload.Add(s.ChildJunction);
					continue;
				}

				// See if we are close enough to start building out
				// the children.
				if (distance <= Constants.MinimumConnectionDistance)
					build.Add(s.ChildJunction);
			}

			// If we got this far, we aren't in the switching distance
			// but we have some junctions that might need preloading.
			if (preload.Count == 0 && build.Count == 0)
				return;

			// Go through the junctions and start the preloading
			foreach (Junction j in preload)
			{
				JunctionManagerPreload jmp = new JunctionManagerPreload(j);
				preloadedJunctions[j] = jmp;
				jmp.ThreadPreload();
			}

			// Also trigger the normal updates
			foreach (Junction j in build)
			{
				// Fire off a background thread
				ThreadPool.QueueUserWorkItem(
					new WaitCallback(UpdateJunction), j);
			}
		}
		#endregion

		#region Updater Threads
		/// <summary>
		/// Processes a single junction in a low-priority thread.
		/// </summary>
		private void UpdateJunction(Object stateInfo)
		{
			// Get our junction
			Junction junction = stateInfo as Junction;
			
			if (junction == null)
				return;

			// Set our thread priority to slightly below normal to let
			// the normal game play take priority.
			Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

			// Process the junction
			junction.BuildConnections();
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
