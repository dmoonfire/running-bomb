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
		private int digitToSpaceLength = 2;
		private int commaToSpaceLength = 1;

		/// <summary>
		/// Returns the alignment of this font.
		/// </summary>
		public ContentAlignment Alignment
		{
			get { return align; }
		}

		/// <summary>
		/// This is the number of spaces to convert commas into
		/// whitespace.
		/// </summary>
		public int CommaToSpaceLength
		{
			get { return commaToSpaceLength; }
		}

		/// <summary>
		/// The actual white space to use.
		/// </summary>
		public string CommaToSpaceString
		{
			get { return ToSpace(commaToSpaceLength); }
		}

		/// <summary>
		/// This is the number of spaces to convert a digit into
		/// whitespace.
		/// </summary>
		public int DigitToSpaceLength
		{
			get { return digitToSpaceLength; }
		}

		/// <summary>
		/// The spaces to replace a digit with.
		/// </summary>
		public string DigitToSpaceString
		{
			get { return ToSpace(digitToSpaceLength); }
		}

		/// <summary>
		/// Contains the loaded font for this theme.
		/// </summary>
		public BooGameFont Font
		{
			get { return font; }
		}
		#endregion

		#region Formatting
		/// <summary>
		/// Converts a length into a string and returns it.
		/// </summary>
		private string ToSpace(int length)
		{
			return "         ".Substring(0, length);
		}
		#endregion
	}
}
