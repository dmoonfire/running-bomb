using MfGames.Sprite3;
using MfGames.Sprite3.BooWorks;
using MfGames.Utility;
using RunningBomb.Audio;
using System;
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
			bwm.Startup(ConfigFile);

			// Set up the clean up event
			BooGame.Core.Exiting += OnShutdown;

			// Set up our audio settings
			AudioManager.Startup();

			// Start the game
			bwm.Run(ConfigFile, new MainMenuGameMode());
		}

		/// <summary>
		/// Registers when the system attempts to shut down.
		/// </summary>
		private static void OnShutdown(object sender, EventArgs args)
		{
			// Stop music
			AudioManager.Stop();
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
