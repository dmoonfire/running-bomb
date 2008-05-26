using C5;
using MfGames.RunningBomb;
using MfGames.Sprite3;
using MfGames.Sprite3.BooWorks;
using MfGames.Utility;
using System;
using System.Drawing;
using System.IO;
using System.Xml;

namespace RunningBomb.Themes
{
	/// <summary>
	/// Contains information about a single theme and also performs
	/// the actual rendering of theme data on the screen, with
	/// callbacks.
	/// </summary>
	public class Theme
	{
		#region Constants
		public const string DisplayLayout = "display";
		#endregion

		#region Properties
		private string name;

		/// <summary>
		/// Contains the human display name for the theme.
		/// </summary>
		public string Name
		{
			get { return name; }
		}
		#endregion

		#region Rendering
		private HashDictionary<string,ThemeLayout> layouts =
			new HashDictionary<string,ThemeLayout>();

		/// <summary>
		/// Renders out the screen, using the callback to fill in the
		/// gaps.
		/// </summary>
		public void Render(
			string layout, IThemeCallback callback, DrawingArgs args)
		{
			// Get the layout
			if (!layouts.Contains(layout))
				throw new Exception("Theme does not have the " + layout
					+ " layout");

			// Set up the context
			ThemeContext context = new ThemeContext();
			context.DrawingArgs = args;
			context.Callback = callback;
			context.ScreenSize = VideoManager.ScreenSize;
			context.Theme = this;

			context["width"] = context.ScreenSize.Width;
			context["height"] = context.ScreenSize.Height;
			context["distance"] = State.Score.Distance;
			context["countdown"] = State.Score.CountdownString;
			context["popsaved"] = State.Score.PopulationSaved;
			context["popkilled"] = State.Score.PopulationKilled;
			context["speed"] = State.Score.Speed;
			context["maxspeed"] = State.Score.MaximumSpeed;
			context["speedfmt"] = State.Score.SpeedString;
			context["maxspeedfmt"] = State.Score.MaximumSpeedString;

			// Get it
			ThemeLayout tl = layouts[layout];
			tl.Render(context);
		}
		#endregion

		#region Reading
		/// <summary>
		/// Reads theme data from the given file.
		/// </summary>
		public void Read(FileInfo file)
		{
			using (FileStream fs = file.OpenRead())
			{
				XmlReader xml = XmlReader.Create(fs);
				Read(xml);
				xml.Close();
				fs.Close();
			}
		}

		/// <summary>
		/// Reads theme information from a given XML stream.
		/// </summary>
		private void Read(XmlReader xml)
		{
			// Loop through the input
			while (xml.Read())
			{
				// Handle the ending elements
				if (xml.NodeType == XmlNodeType.EndElement)
				{
					if (xml.LocalName == "theme")
						return;
				}
				
				// Parse starting nodes
				if (xml.NodeType == XmlNodeType.Element)
				{
					switch (xml.LocalName)
					{
						case "name":
							name = xml.ReadString();
							break;

						case "font":
							ReadFont(xml);
							break;

						case "layout":
							// Do a bit of sanity checking
							if (String.IsNullOrEmpty(xml["id"]))
								throw new Exception(
									"Cannot handle id'less layout");

							// Load the layout into memory
							ThemeLayout tl = new ThemeLayout();
							tl.Read(xml);
							layouts[tl.ID] = tl;
							break;
					}
				}
			}
		}
		#endregion

		#region Rendering - Fonts and Text
		private HashDictionary<string,ThemeFont> fonts =
			new HashDictionary<string,ThemeFont>();

		/// <summary.
		/// Contains a dictionary of fonts for this theme.
		/// </summary>
		public IDictionary<string,ThemeFont> Fonts
		{
			get { return fonts; }
		}

		/// <summary>
		/// Reads font information into the system and prepares its
		/// use.
		/// </summary>
		private void ReadFont(XmlReader xml)
		{
			// Sanity checking
			if (String.IsNullOrEmpty(xml["id"]))
				throw new Exception("Cannot read font: id");
			if (String.IsNullOrEmpty(xml["size"]))
				throw new Exception("Cannot read font: size");
			if (String.IsNullOrEmpty(xml["align"]))
				throw new Exception("Cannot read font: align");
			if (String.IsNullOrEmpty(xml["file"]))
				throw new Exception("Cannot read font: file");

			// Pull out the parts
			string id = xml["id"];
			float size = Single.Parse(xml["size"]);
			ContentAlignment alignment = (ContentAlignment)
				Enum.Parse(typeof(ContentAlignment), xml["align"]);
			FileInfo file = new FileInfo(xml["file"]);

			// Create the font
			ThemeFont tf = new ThemeFont(id, size, alignment, file);
			fonts[id] = tf;
			Log.Info("Loaded theme font: {0}", id);
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
