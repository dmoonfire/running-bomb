using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml;

namespace RunningBomb.Themes
{
	/// <summary>
	/// Consolidate processing of theme-based colors.
	/// </summary>
	public class ThemeColor
	{
		#region Constructors
		/// <summary>
		/// Creates the single and parses the results.
		/// </summary>
		public ThemeColor(string fmt)
		{
			// Try formatting it through normal channels
			color = ColorTranslator.FromHtml(fmt);
		}
		#endregion

		#region Properties
		private Color color = Color.White;
		#endregion

		#region Values
		/// <summary>
		/// Retrieves the color, which may be based on context.
		/// </summary>
		public Color GetColor(ThemeContext context)
		{
			return color;
		}
		#endregion
	}
}
