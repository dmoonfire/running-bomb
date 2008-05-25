using MfGames.Sprite3;
using MfGames.Sprite3.BooWorks;
using MfGames.Utility;
using System.IO;

namespace RunningBomb
{
	/// <summary>
	/// Primary entry into the Running Bomb game itself.
	/// </summary>
	public class RunningBombEntry
	{
		/// <summary>
		/// Main entry into the application.
		/// </summary>
		public static void Main(string [] args)
		{
			// Set up the works infrastructure
			BooWorksManager bwm = new BooWorksManager();
			bwm.Run(ConfigFile, new TestThemeGameMode());
			//bwm.Run(ConfigFile, new NewGameMode());
		}

		#region Configuration
		/// <summary>
		/// Contains the full path to the configuration file.
		/// </summary>
		public static FileInfo ConfigFile
		{
			get
			{
				// Set up the configuration
				if (ConfigStorage.Singleton == null)
					ConfigStorage.Singleton = new ConfigStorage("MfGames");

				// Get the directory
				DirectoryInfo cDir =
					ConfigStorage.Singleton.GetDirectory("Running Bomb");
				return new FileInfo(cDir.FullName
					+ Path.DirectorySeparatorChar
					+ "config.xml");
			}
		}
		#endregion
	}
}
