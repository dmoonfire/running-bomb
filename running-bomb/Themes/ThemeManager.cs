using System;
using System.Drawing;
using System.IO;

namespace RunningBomb.Themes
{
	/// <summary>
	/// Manages the theme and also the list of possible themes in the
	/// game.
	/// </summary>
	public static class ThemeManager
	{
		#region Constructors
		static ThemeManager()
		{
			// Sets up the default theme
			theme = new Theme();
			theme.Read(new FileInfo("../assets/themes/default/theme.xml"));

			// Set up the events
		}
		#endregion

		#region Theme Selection
		private static Theme theme;

		/// <summary>
		/// Sets or gets the currently selected theme. This defaults
		/// to "default" if none are selected and it is saved through
		/// the settings object.
		/// </summary>
		public static Theme Theme
		{
			get { return theme; }
			set
			{
				if (value == null)
					throw new Exception("Cannot set a null theme");

				theme = value;
			}
		}
		#endregion
	}
}
