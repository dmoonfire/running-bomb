using Gtk;
using MfGames.RunningBomb.Draw;
using MfGames.Utility;
using System;
using System.IO;

namespace MfGames.RunningBomb.GtkDrawer
{
	/// <summary>
	/// The primary window for the application, this contains the menu
	/// bars, status windows, and the Cairo display.
	/// </summary>
	public class GtkDrawerFrame
	: Window
	{
		#region Constructors
		/// <summary>
		/// Constructs the primary window, builds the UI, and connects the
		/// standard eventing.
		/// </summary>
		public GtkDrawerFrame()
			: base("Moonfire Games' Running Bomb (Drawer)")
		{
			// Create the basic UI elements
			VBox innerBox = new VBox(false, 0);
			Add(innerBox);

			// Add the UI
			SetupUI();
			innerBox.PackStart(uim.GetWidget("/MenuBar"), false, false, 0);

			// Add the canvas
			canvas = new GtkDrawerCanvas();
			innerBox.PackStart(canvas, true, true, 0);

			// Connect the basic events
			DeleteEvent += OnDelete;

			// Create a new form
			file = new FileInfo("../assets/themes/default/engine.xml");
			DrawingCommandsReader dcr = new DrawingCommandsReader();
			canvas.DrawingCommands = dcr.Read(file);

			// Set our default size
			SetDefaultSize(640, 480);
		}
		#endregion

		#region UI Creation
		private GtkDrawerCanvas canvas;
		private UIManager uim;

		/// <summary>
		/// Sets up the UI elements using the UI manager and connecting
		/// the events.
		/// </summary>
		private UIManager SetupUI()
		{
			// Set up our action entries
			ActionEntry[] entries = new ActionEntry[] {
				new ActionEntry("FileMenu", null, "_File", null,
					null, null),

				new ActionEntry("Reload", null, "Re_load", "<control>R",
					"Reload", new EventHandler(OnReload)),
				new ActionEntry("ZoomIn", null, "Zoom _In", "<control>I",
					"Zoom In", new EventHandler(OnZoomIn)),
				new ActionEntry("ZoomOut", null, "Zoom _Out", "<control>O",
					"Zoom Out", new EventHandler(OnZoomOut)),
				new ActionEntry("ZoomReset", null, "Zoom _Reset", "<control>0",
					"Zoom Reset", new EventHandler(OnZoomReset)),
			
				new ActionEntry("Quit", Stock.Quit, "_Quit", "<control>Q",
					"Quit", new EventHandler(OnDelete)),
			};

			ActionGroup actions = new ActionGroup("group");
			actions.Add(entries);
		
			// Create the UI to populate data
			uim = new UIManager();
			uim.InsertActionGroup(actions, 0);
			AddAccelGroup(uim.AccelGroup);
			uim.AddUiFromResource("ui.xml");

			// Return the results
			return uim;
		}
		#endregion

		#region UI Events
		private FileInfo file;

		/// <summary>
		/// Triggered when the Quit item is selected or the close button
		/// is clicked on the window.
		/// </summary>
		private void OnDelete(object o, EventArgs e)
		{
			Application.Quit();
		}

		/// <summary>
		/// Reloads the file.
		/// </summary>
		private void OnReload(object o, EventArgs e)
		{
			DrawingCommandsReader dcr = new DrawingCommandsReader();
			canvas.DrawingCommands = dcr.Read(file);
			canvas.QueueDraw();
		}

		/// <summary>
		/// Changes the zoom of the window.
		/// </summary>
		private void OnZoomIn(object o, EventArgs e)
		{
			canvas.Scale += 0.5f;
			canvas.QueueDraw();
		}

		/// <summary>
		/// Changes the zoom of the window.
		/// </summary>
		private void OnZoomOut(object o, EventArgs e)
		{
			canvas.Scale -= 0.5f;
			canvas.QueueDraw();
		}

		/// <summary>
		/// Changes the zoom of the window.
		/// </summary>
		private void OnZoomReset(object o, EventArgs e)
		{
			canvas.Scale = 1;
			canvas.QueueDraw();
		}
		#endregion
	}
}
