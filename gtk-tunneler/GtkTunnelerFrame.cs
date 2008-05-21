using Gtk;
using MfGames.RunningBomb.Draw;
using MfGames.Utility;
using System;
using System.IO;

namespace MfGames.RunningBomb.GtkTunneler
{
	/// <summary>
	/// The primary window for the application, this contains the menu
	/// bars, status windows, and the Cairo display.
	/// </summary>
	public class GtkTunnelerFrame
	: Window
	{
		#region Constructors
		/// <summary>
		/// Constructs the primary window, builds the UI, and connects the
		/// standard eventing.
		/// </summary>
		public GtkTunnelerFrame()
			: base("Moonfire Games' Running Bomb (Tunneler)")
		{
			// Create the basic UI elements
			VBox innerBox = new VBox(false, 0);
			Add(innerBox);

			// Add the UI
			SetupUI();
			innerBox.PackStart(uim.GetWidget("/MenuBar"), false, false, 0);

			// Add the canvas
			canvas = new GtkTunnelerCanvas();
			innerBox.PackStart(canvas, true, true, 0);

			// Connect the basic events
			DeleteEvent += OnDelete;

			// Set our default size
			SetDefaultSize(640, 480);
		}
		#endregion

		#region UI Creation
		private GtkTunnelerCanvas canvas;
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

				new ActionEntry("Reload", null, "Re_load", "<control>L",
					"Reload everything", new EventHandler(OnReload)),
				new ActionEntry("Rebuild", null, "Re_build", "<control>B",
					"Rebuild the entire map", new EventHandler(OnRebuild)),
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
		/// <summary>
		/// Triggered when the Quit item is selected or the close button
		/// is clicked on the window.
		/// </summary>
		private void OnDelete(object o, EventArgs e)
		{
			Application.Quit();
		}

		/// <summary>
		/// Resets the entire junction.
		/// </summary>
		private void OnRebuild(object o, EventArgs e)
		{
			canvas.Rebuild();
		}

		/// <summary>
		/// Reloads the entire junction without changing it.
		/// </summary>
		private void OnReload(object o, EventArgs e)
		{
			canvas.Reload();
		}

		/// <summary>
		/// Changes the zoom of the window.
		/// </summary>
		private void OnZoomIn(object o, EventArgs e)
		{
			canvas.Scale *= 2;
			canvas.QueueDraw();
		}

		/// <summary>
		/// Changes the zoom of the window.
		/// </summary>
		private void OnZoomOut(object o, EventArgs e)
		{
			canvas.Scale /= 2;
			canvas.QueueDraw();
		}

		/// <summary>
		/// Changes the zoom of the window.
		/// </summary>
		private void OnZoomReset(object o, EventArgs e)
		{
			canvas.Scale = 0.125f;
			canvas.QueueDraw();
		}
		#endregion
	}
}
