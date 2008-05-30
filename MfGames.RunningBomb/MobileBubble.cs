using System;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Encapsulates the common functionality that all bubbles have,
	/// be living quarters or modules.
	/// </summary>
	public class MobileBubble
	: Mobile
	{
		#region Properties
		private float radius = 1;
		
		/// <summary>
		/// Contains the radius of the bubble.
		/// </summary>
		public float Radius
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
	}
}
