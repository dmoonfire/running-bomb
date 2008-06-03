using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Describes the common functionality of all segment factories.
	/// </summary>
	public interface ISegmentFactory
	{
		/// <summary>
		/// Creates a segment from a child junction to its parent.
		/// </summary>
		Segment Create(Junction childJunction, PointF childPoint, double distance);
	}
}
