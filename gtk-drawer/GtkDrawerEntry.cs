using Gtk;

namespace MfGames.RunningBomb.GtkDrawer
{
	/// <summary>
	/// Primary entry into the gtk drawing application.
	/// </summary>
	public class GtkDrawerEntry
	{
		/// <summary>
		/// The primary entry into the entire system.
		/// </summary>
		public static void Main(string [] args)
		{
			// Start up Gtk
			Application.Init();

			// This appliation only uses a single window interface
			GtkDrawerFrame frame = new GtkDrawerFrame();
			frame.ShowAll();

			// Start up Gtk and process through the GUI loop
			Application.Run();
		}
	}
}
