using Gtk;
using MfGames.RunningBomb;

namespace MfGames.RunningBomb.GtkTunneler
{
	/// <summary>
	/// Primary entry into the gtk drawing application.
	/// </summary>
	public class GtkTunnelerEntry
	{
		/// <summary>
		/// The primary entry into the entire system.
		/// </summary>
		public static void Main(string [] args)
		{
			// Start up Gtk
			Application.Init();
			Junction.GeneratePhysics = false;

			// This appliation only uses a single window interface
			GtkTunnelerFrame frame = new GtkTunnelerFrame();
			frame.ShowAll();

			// Start up Gtk and process through the GUI loop
			Application.Run();
		}
	}
}
