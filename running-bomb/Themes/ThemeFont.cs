using BooGame.Video;
using System.Drawing;
using System.IO;

namespace RunningBomb.Themes
{
	/// <summary>
	/// Encapsulates the functionality of a theme font, including
	/// color, alignment, and the actual font.
	/// </summary>
	public class ThemeFont
	{
		#region Constructors
		/// <summary>
		/// Constructs the font from the related elements.
		/// </summary>
		public ThemeFont(
			string id, float size, ContentAlignment align, FileInfo file)
		{
			this.font = new FreeTypeFont(file.FullName, size);
			this.align = align;
		}
		#endregion

		#region Properties
		private ContentAlignment align;
		private BooGameFont font;

		/// <summary>
		/// Returns the alignment of this font.
		/// </summary>
		public ContentAlignment Alignment
		{
			get { return align; }
		}

		/// <summary>
		/// Contains the loaded font for this theme.
		/// </summary>
		public BooGameFont Font
		{
			get { return font; }
		}
		#endregion
	}
}
