namespace MfGames.RunningBomb
{
	/// <summary>
	/// The primary static state for the entire game. This contains
	/// the score, junction, and player information needed to play the
	/// game.
	/// </summary>
	public static class State
	{
		#region Setup
		public static void Initialize()
		{
			// Set up the game
			player = new Player();
			score = new Score();

			// Set up the physics
			physics = new Physics();
			physics.Add(player);
		}
		#endregion

		#region Properties
		private static Score score;
		private static Player player;
		private static Physics physics;

		/// <summary>
		/// Contains the physics engine used.
		/// </summary>
		public static Physics Physics
		{
			get { return physics; }
		}

		/// <summary>
		/// Contains the information about the player.
		/// </summary>
		public static Player Player
		{
			get { return player; }
		}

		/// <summary>
		/// Contains the score object.
		/// </summary>
		public static Score Score
		{
			get { return score; }
		}
		#endregion
	}
}
