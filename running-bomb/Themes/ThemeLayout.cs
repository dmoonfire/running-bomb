using C5;
using MfGames.Utility;
using System.Drawing;
using System.Xml;

namespace RunningBomb.Themes
{
	/// <summary>
	/// Implements the commands for rendering out a layout to the
	/// screen, including text and arrangement.
	/// </summary>
	public class ThemeLayout
	{
		#region Properties
		private string id;

		/// <summary>
		/// Contains the layout ID.
		/// </summary>
		public string ID { get { return id; } }
		#endregion

		#region Rendering
		private ArrayList<IThemeCommand> commands =
			new ArrayList<IThemeCommand>();

		/// <summary>
		/// Renders out the screen, using the callback to fill in the
		/// gaps.
		/// </summary>
		public void Render(ThemeContext context)
		{
			// Loop through the commands
			foreach (IThemeCommand itc in commands)
				itc.Render(context);
		}
		#endregion

		#region IO
		/// <summary>
		/// Reads the layout from the given XML stream.
		/// </summary>
		public void Read(XmlReader xml)
		{
			// Get the id
			id = xml["id"];

			// Loop through the input
			while (xml.Read())
			{
				// Handle the ending elements
				if (xml.NodeType == XmlNodeType.EndElement)
				{
					if (xml.LocalName == "layout")
						return;
				}
				
				// Parse starting nodes
				if (xml.NodeType == XmlNodeType.Element)
				{
					switch (xml.LocalName)
					{
						case "string":
							commands.Add(new ThemeString(xml));
							break;

						case "viewport":
							commands.Add(new ThemeViewport(xml));
							break;
					}
				}
			}
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
