using MfGames.RunningBomb;
using MfGames.Sprite3.BooWorks;

namespace RunningBomb
{
	/// <summary>
	/// Sets up a new game and starts everything before passing it
	/// over to RunningGameMode.
	/// </summary>
	public class NewGameMode
	: NullGameMode
	{
		/// <summary>
		/// Called when this mode is brought to the focus, which
		/// immediately sets up a new game and starts playing.
		/// </summary>
		public override void OnFocused()
		{
			// Set up a new score object, junction and everything
			State.Initialize();
			State.JunctionManager.Junction = new Junction();

			// Set up the view state
			ViewState.Scale = 1;

			// Switch to the running mode
			GameModeManager.Set(new RunningGameMode());
		}
	}
}
