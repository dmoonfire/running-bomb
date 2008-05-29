using System;
using System.IO;

namespace RunningBomb
{
	/// <summary>
	/// Maps out a standard file format using forward relative slashes
	/// into a system-specific version.
	/// </summary>
	public static class Assets
	{
		/// <summary>
		/// Returns a top-level directory for the assets.
		/// </summary>
		public static DirectoryInfo AssetsDirectory
		{
			get
			{
#if DEBUG
				// See if we have a debug assets
				string path = ".." + Path.DirectorySeparatorChar + "assets";
				
				if (Directory.Exists(path))
					return new DirectoryInfo(path);
#endif
					
				// See if we have a local one
				if (Directory.Exists("assets"))
					return new DirectoryInfo("assets");

				// Throw an exception
				throw new Exception("Cannot find assets directory");
			}
		}

		/// <summary>
		/// Returns a directory for the given asset path.
		/// </summary>
		public static DirectoryInfo GetAssetDirectory(string path)
		{
			path = path.Replace('/', Path.DirectorySeparatorChar);

			return new DirectoryInfo(
				AssetsDirectory.FullName
				+ Path.DirectorySeparatorChar
				+ path);
		}

		/// <summary>
		/// Returns a file for a given path.
		/// </summary>
		public static FileInfo GetAssetFile(string path)
		{
			path = path.Replace('/', Path.DirectorySeparatorChar);

			return new FileInfo(
				AssetsDirectory.FullName
				+ Path.DirectorySeparatorChar
				+ path);
		}
	}
}
