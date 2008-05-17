namespace MfGames.RunningBomb
{
	/// <summary>
	/// Contains a bunch of common functions used in the system.
	/// </summary>
	public static class Functions
	{
		/// <summary>
		/// Returns a value of two that is a power of two greater than
		/// the count.
		/// </summary>
        public static int NextPowerOfTwo(int n)
        {
            double power = 0;

            while (n > System.Math.Pow(2.0, power))
                power++;

            return (int)System.Math.Pow(2.0, power);
        }
	}
}

