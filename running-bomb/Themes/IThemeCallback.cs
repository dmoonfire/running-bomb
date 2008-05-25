using MfGames.Sprite3;
using System.Drawing;

namespace RunningBomb.Themes
{
	/// <summary>
	/// Defines the callback signature for theme rendering.
	/// </summary>
	public interface IThemeCallback
	{
		/// <summary>
		/// Draws the viewport in the given bounds.
		/// </summary>
		void DrawViewport(DrawingArgs args, RectangleF bounds);
	}
}
