using RunningBomb.Themes;

namespace RunningBomb
{
	/// <summary>
	/// Represents a single line in a text scrolling mode.
	/// </summary>
	public class TextScrollLine
	{
		public string Line;
		public LineType LineType;
		public float Height;
		public float Position;
		public ThemeFont Font;
	}
}
