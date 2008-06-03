using AdvanceMath;
using Physics2DDotNet;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Implements a basic engine module that can attach to the
	/// player's ship.
	/// </summary>
	public class EngineModule
	: Module
	{
		#region Properties
		/// <summary>
		/// Returns the color of this mobile.
		/// </summary>
		public override Color Color { get { return Color.DarkBlue; } }
		#endregion

		#region Physics
		/// <summary>
		/// Contains the engine power of this module.
		/// </summary>
		public override float EnginePower
		{
			get { return Radius; }
		}
		#endregion
	}
}
