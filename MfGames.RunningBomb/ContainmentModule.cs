using AdvanceMath;
using Physics2DDotNet;
using System;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Implements a basic containment module that can attach to the
	/// player's ship.
	/// </summary>
	public class ContainmentModule
	: Module
	{
		#region Properties
		/// <summary>
		/// Returns the color of this mobile.
		/// </summary>
		public override Color Color { get { return Color.DarkGreen; } }
		#endregion

		#region Physics
		/// <summary>
		/// Contains the containment power of this module.
		/// </summary>
		public float ContainmentPower
		{
			get { return Radius; }
		}
		#endregion
	}
}
