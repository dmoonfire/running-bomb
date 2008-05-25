using MfGames.Sprite3;
using System;
using System.Drawing;
using System.Xml;

namespace RunningBomb.Themes
{
	/// <summary>
	/// Handles rendering of strings to the screen.
	/// </summary>
	public class ThemeString
	: IThemeCommand
	{
		#region Constructors
		/// <summary>
		/// Creates the string command from the XML stream.
		/// </summary>
		public ThemeString(XmlReader xml)
		{
			// Get the coordinates
			x = new ThemeSingle(xml["x"]);
			y = new ThemeSingle(xml["y"]);
			fontName = xml["font"];
			text = xml["text"];
			format = xml["format"];

			// See if we have fields
			if (!String.IsNullOrEmpty(xml["variables"]))
			{
				// Parse it for the format string
				string vars = xml["variables"].Replace(" ", "");
				variables = vars.Split(',');
			}
		}
		#endregion

		#region Properties
		private string fontName;
		private ThemeSingle x, y;
		private string text, format;
		private string [] variables;
		#endregion

		#region Rendering
		/// <summary>
		/// Renders the theme command out.
		/// </summary>
		public void Render(ThemeContext context)
		{
			// Get the values
			float rx = x.GetValue(context);
			float ry = y.GetValue(context);

			// Get the font
			ThemeFont font = context.Theme.Fonts[fontName];
			
			// Figure out the string
			string output = text;

			if (!String.IsNullOrEmpty(format) && variables != null)
			{
				// Create a format string instead
				string [] parms = new string [variables.Length];

				for (int i = 0; i < variables.Length; i++)
					parms[i] = context[variables[i]].ToString();

				// Format the string
				output = String.Format(format, parms);
			}

			// Render out the string
		    font.Font.Print(output,
				Color.White,
				rx, ry,
				font.Alignment);
		}
		#endregion
	}
}
