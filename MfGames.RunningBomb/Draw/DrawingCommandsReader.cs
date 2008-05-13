using MfGames.Utility;
using System;
using System.IO;
using System.Xml;

namespace MfGames.RunningBomb.Draw
{
	/// <summary>
	/// A reader object that takes in a file or XML stream and
	/// produces a DrawingCommands object with the given commands
	/// within the document.
	/// </summary>
	public class DrawingCommandsReader
	{
		#region Public Reading
		/// <summary>
		/// Reads drawing commands from the given file and returns the
		/// results.
		/// </summary>
		public DrawingCommands Read(FileInfo file)
		{
			using (FileStream fs = file.OpenRead())
			{
				// Make a bit of noise
				Log.Info("Reading commands from: {0}", file.Name);

				// Create the XML stream
				XmlReader xml = XmlReader.Create(fs);
				return Read(xml);
			}
		}

		/// <summary>
		/// Creates a DrawingCommands object and parses the given XML
		/// stream to populate it with the data.
		/// </summary>
		public DrawingCommands Read(XmlReader xml)
		{
			// Create the object
			DrawingCommands dc = new DrawingCommands();

			// Parse through it
			while (xml.Read())
			{
				// See if we are closing
				if (xml.NodeType == XmlNodeType.EndElement &&
					xml.LocalName == "draw")
				{
					// We are done reading
					break;
				}

				// Parse the elements
				if (xml.NodeType == XmlNodeType.Element)
				{
					switch (xml.LocalName)
					{
						case "circle":
							dc.Add(new DrawCircle(xml));
							break;
					}
				}
			}

			// Return the results
			return dc;
		}
		#endregion

		#region Properties
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
