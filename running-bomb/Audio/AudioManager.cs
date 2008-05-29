using C5;
using MfGames.RunningBomb;
using MfGames.Sprite3;
using MfGames.Utility;
using System;
using System.IO;
using Tao.Sdl;

namespace RunningBomb.Audio
{
	/// <summary>
	/// Manages the audio system for the entire game, both menus and
	/// playing.
	/// </summary>
	public static class AudioManager
	{
		#region Setup and Shutdown
        private static SdlMixer.MusicFinishedDelegate musicStopped;
        private static SdlMixer.ChannelFinishedDelegate channelStopped;
		private static bool isStarted = false;

		/// <summary>
		/// Sets up the SDL audio system for use by the application.
		/// </summary>
		public static void Startup()
		{
			// Don't reopen it
			if (isStarted)
				return;

            // Set up the SDL sound
            SdlMixer.Mix_OpenAudio(
                SdlMixer.MIX_DEFAULT_FREQUENCY,
                (short) SdlMixer.MIX_DEFAULT_FORMAT,
                2,
                1024);

            // Allocate channels
            SdlMixer.Mix_AllocateChannels(Constants.MaximumSoundChunks);

            // Default volumnes
			int vol = 75;
            SdlMixer.Mix_VolumeMusic(vol);

            // Hook up the events
            musicStopped = new SdlMixer.MusicFinishedDelegate(OnMusicEnded);
            SdlMixer.Mix_HookMusicFinished(musicStopped);

            channelStopped =
				new SdlMixer.ChannelFinishedDelegate(OnChannelEnded);
            SdlMixer.Mix_ChannelFinished(channelStopped);

			// Set up our basic sound
			isStarted = true;
		}

		/// <summary>
		/// Stops the music processing.
		/// </summary>
		public static void Stop()
		{
			// Kill all music
			//SdlMixer.Mix_HaltMusic();
			//SdlMixer.Mix_HaltChannel(-1);
			SdlMixer.Mix_CloseAudio();
		}
        #endregion

        #region Playing Music
        private static IntPtr musicChunk;
        private static IntPtr [] soundChunks =
			new IntPtr[Constants.MaximumSoundChunks];
        private static int soundIndex = 0;

        /// <summary>
        /// Triggers when a channel is finished.
        /// </summary>
        /// <param name="channel"></param>
        private static void OnChannelEnded(int channel)
        {
        }

        /// <summary>
        /// Triggered when the background music has stopped processing.
        /// </summary>
        private static void OnMusicEnded()
        {
            // Unallocate our music
            SdlMixer.Mix_FreeMusic(musicChunk);
        }
		#endregion

		#region Properties
		private static string themeName = "default";
		
		private static MersenneRandom random = new MersenneRandom();
		#endregion

		#region Background Samples
		private static int [] loopRhythms;
		private static ArrayList<FileInfo> loopFiles =
			new ArrayList<FileInfo>(Constants.MaximumAudioSamples);
		private static ArrayList<IntPtr> loopChunks =
			new ArrayList<IntPtr>(Constants.MaximumAudioSamples);

		/// <summary>
		/// Contains the number of beats per second this music is
		/// going to be using. If there is a score, it uses the
		/// Score.Stress, otherwise it defaults to 4.
		/// </summary>
		public static double BeatsPerLoop
		{
			get
			{
				// See if we have a score
				if (State.Score != null)
				{
					return Math.Max(4,
						State.Score.Stress * Constants.MaximumBeatsPerLoop);
				}

				// Default to four
				return 4;
			}
		}

		/// <summary>
		/// Contains the number of seconds for every beat.
		/// </summary>
		public static double SecondsPerBeat
		{
			get
			{
				return Constants.LoopSeconds / BeatsPerLoop;
			}
		}

		/// <summary>
		/// This routine sets up the background samples by selecting a
		/// number of them from the theme directory and seeding the
		/// initial sample loops.
		/// </summary>
		public static void ResetBackgroundSamples()
		{
			// Go through the loops and unallocate them
			SdlMixer.Mix_HaltChannel(-1);

			foreach (IntPtr chunk in loopChunks)
				SdlMixer.Mix_FreeChunk(chunk);

			// Clear the lists
			loopRhythms = new int [Constants.MaximumAudioSamples];
			loopFiles.Clear();
			loopChunks.Clear();

			// Get the list of files we are using
			DirectoryInfo di = Assets.GetAssetDirectory("samples/default");
			FileInfo [] fis = di.GetFiles("*.ogg");

			// Select up to the maximum samples allowed
			for (int i = 0; i < Constants.MaximumAudioSamples; i++)
			{
				// Get a random file and set up the rhythm
				loopFiles.Add(fis[random.Next(0, fis.Length)]);
				loopRhythms[i] = random.Next();

				// Allocate the chunk
				IntPtr ptr = SdlMixer.Mix_LoadWAV(loopFiles[i].FullName);

				if (ptr == IntPtr.Zero)
				{
					// We had a problem
					Log.Info("Cannot load chunk {0}: {1}",
						i, SdlMixer.Mix_GetError());
				}

				loopChunks.Add(ptr);

				// Make a bit of sound
				Log.Debug("Adding background sample: {0} - {1}",
					loopFiles[i].Name, ptr);
			}
			
		}
		#endregion

		#region Updating
		private static double lastBeat;
		private static int beatIndex;
		private static double lastAlter;

		/// <summary>
		/// Processes the sound generation for background music.
		/// </summary>
        public static void Update(UpdateArgs args)
        {
			// Don't bother if we don't have any
			if (loopChunks.Count == 0)
				return;

			// Add to the counters
			lastBeat += args.SecondsSinceLastUpdate;
			lastAlter += args.SecondsSinceLastUpdate;

			// If we exceeded the time for altering, update something
			if (lastAlter >= Constants.AudioUpdateSpeed)
			{
				// Decrement it so we only change on occasion
				lastAlter -= Constants.AudioUpdateSpeed;

				// Alter something
				int index = random.Next(0, Constants.MaximumAudioSamples);
				int bit = random.Next(0, Constants.MaximumBeatsPerLoop);

				// We basically XOR the random bit
				loopRhythms[index] = loopRhythms[index] ^ (1 << bit);
			}

			// See if we need to update our beat samples
			if (lastBeat >= SecondsPerBeat)
			{
				// Make a bit of noise
				//Log.Debug("--- BEAT ---");

				// Decrement it
				lastBeat -= SecondsPerBeat;

				// Go through and start playing the samples
				int bitIndex = 1 << beatIndex;

				for (int i = 0; i < Constants.MaximumAudioSamples; i++)
				{
					// See if we are set
					if ((loopRhythms[i] & bitIndex) != 0)
					{
						// Start playing it
						int results = 
							SdlMixer.Mix_PlayChannel(-1, loopChunks[i], 1);

						if (results == -1)
						{
							// We had a problem
							Log.Info("Cannot start sample {0}: {1}",
								i, SdlMixer.Mix_GetError());
						}
					}
				}

				// Increment the index
				int maxBeats = (int) BeatsPerLoop;
				beatIndex = (beatIndex + 1) % maxBeats;
			}
		}
		#endregion

		#region Logging
		private static Log log;

		/// <summary>
		/// Contains the logging interface which is lazily-loaded.
		/// </summary>
		public static Log Log
		{
			get
			{
				if (log == null)
					log = new Log(typeof(AudioManager));

				return log;
			}
		}
		#endregion
	}
}
