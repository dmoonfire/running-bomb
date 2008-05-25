using C5;
using MfGames.Sprite3;
using System.Drawing;

namespace RunningBomb.Themes
{
	/// <summary>
	/// The context used throughout the entire theme engine.
	/// </summary>
	public class ThemeContext
	: HashDictionary<string,object>
	{
		#region Simple Properties
		public Theme Theme;
		public IThemeCallback Callback;
		public DrawingArgs DrawingArgs;
		public SizeF ScreenSize;
		#endregion
	}
}
