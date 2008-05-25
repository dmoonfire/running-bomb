using MfGames.Sprite3;
using MfGames.Utility;
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
			color = new ThemeColor(xml["color"]);

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
		private ThemeFont font;
		private ThemeColor color;
		private string text, format;
		private string [] variables;
		#endregion

		#region Rendering
		/// <summary>
		/// Formats a zero-fill rules. This takes a width of the line,
		/// which is formatted as if it was all zeros. For examples,
		/// z7 would produce "0,000,000" then remove the given value
		private void FormatZeroFill(int index, ref string format, object val)
		{
			// Convert the value to an integer
			long value = Convert.ToInt64(val);

			// Build up the string
			string search = "{" + index + ":z";
			int ndx = format.IndexOf(search);
			
			while (ndx >= 0)
			{
				// Get the string
				string specifier = format.Substring(ndx + 1);
				int lastIndex = specifier.IndexOf("}");

				if (lastIndex < 0)
					throw new Exception(
						"Cannot find ending brace in specifier: " + specifier);

				specifier = specifier.Substring(0, lastIndex);
				int number =
					Int32.Parse(specifier.Substring(search.Length - 1));

				// Figure out the zero-padded fill. We do this by
				// adding a number of 1's to the right amount,
				// printing this via "N0" to get the commas in the
				// right place, then replacing the 1's with 0'.
				long zero = 0;

				for (int i = 0; i < number; i++)
					zero = zero * 10 + 1;

				string zeroString = zero.ToString("N0").Replace("1", "0");

				// Now format the value in the same manner to get what
				// we need to remove. Then, remove the length of the
				// value string from the right-side of the zero string
				// and replace it with an appropriate amount of spaces.
				string valueString = value.ToString("N0");

				// If they are not equal in length, we do something special
				string replace = valueString;

				if (valueString.Length < zeroString.Length)
				{
					// Remove the different
					int diff = zeroString.Length - valueString.Length;
					replace = zeroString.Substring(0, diff) +
						valueString
						.Replace(",", font.CommaToSpaceString)
						.Replace("0", font.DigitToSpaceString)
						.Replace("1", font.DigitToSpaceString)
						.Replace("2", font.DigitToSpaceString)
						.Replace("3", font.DigitToSpaceString)
						.Replace("4", font.DigitToSpaceString)
						.Replace("5", font.DigitToSpaceString)
						.Replace("6", font.DigitToSpaceString)
						.Replace("7", font.DigitToSpaceString)
						.Replace("8", font.DigitToSpaceString)
						.Replace("9", font.DigitToSpaceString);
				}

				// Remove the specifier from the format
				format = format.Replace("{" + specifier + "}", replace);

				// Look for the next one
				ndx = format.IndexOf(search);
			}
		}

		/// <summary>
		/// Renders the theme command out.
		/// </summary>
		public void Render(ThemeContext context)
		{
			// Get the values
			float rx = x.GetValue(context);
			float ry = y.GetValue(context);

			// Get the font
			font = context.Theme.Fonts[fontName];
			
			// Figure out the string
			string output = text;
			string fmt = format;

			if (!String.IsNullOrEmpty(format) && variables != null)
			{
				// Create a format string instead
				object [] parms = new object [variables.Length];

				for (int i = 0; i < variables.Length; i++)
				{
					// We need to do a bit of formatting to make these
					// numbers as appropriate.
					if (fmt.IndexOf("{" + i + ":N") >= 0)
					{
						// Convert to a double to make it a number
						parms[i] = Convert.ToDouble(context[variables[i]]);
					}
					else if (fmt.IndexOf("{" + i + ":z") >= 0)
					{
						// This uses the zero fill rules
						FormatZeroFill(i, ref fmt, context[variables[i]]);
						parms[i] = 0;
					}
					else
					{
						// Just use it as a string
						parms[i] = context[variables[i]].ToString();
					}
				}

				// Format the string
				output = String.Format(fmt, parms);
			}

			// Render out the string
		    font.Font.Print(output,
				color.GetColor(context),
				rx, ry,
				font.Alignment);
		}
		#endregion

		#region Logging
		private Log log;

		/// <summary>
		/// Contains the logging interface which is lazily-loaded.
		/// </summary>
		public Log Log
		{
			get
			{
				if (log == null)
					log = new Log(GetType());

				return log;
			}
		}
		#endregion
	}
}
