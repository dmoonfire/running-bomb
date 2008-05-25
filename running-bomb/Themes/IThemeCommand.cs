namespace RunningBomb.Themes
{
	/// <summary>
	/// Defines the common functionality of theme rendering commands.
	/// </summary>
	public interface IThemeCommand
	{
		/// <summary>
		/// Renders the theme command out.
		/// </summary>
		void Render(ThemeContext context);
	}
}
