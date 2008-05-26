using System;

namespace RunningBomb
{
	/// <summary>
	/// Contains information about the view state.
	/// </summary>
	public static class ViewState
	{
		private static float scale = 0.1f;

		/// <summary>
		/// Contains the scale to render the various elements of the
		/// game window.
		/// </summary>
		public static float Scale
		{
			get { return scale; }
			set
			{
				// Just ignore if it would be zero
				if (value <= 0)
					return;

				// Set the value
				scale = value;
			}
		}
	}
}
