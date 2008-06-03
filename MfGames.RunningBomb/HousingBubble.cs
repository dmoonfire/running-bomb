using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// A housing bubble is where 0-5 people are living (based on the
	/// radius).
	/// </summary>
	public class HousingBubble
	: Mobile
	{
		/// <summary>
		/// Returns the color of this mobile.
		/// </summary>
		public override Color Color { get { return Color.White; } }
	}
}
