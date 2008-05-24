using C5;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// Specialized list for handling center points, including some
	/// path and distance optimization.
	/// </summary>
	public class CenterPointList
	: ArrayList<CenterPoint>
	{
		/// <summary>
		/// Contains the maximum distance of this list.
		/// </summary>
		public float MaximumRelativeDistance
		{
			get { return Last.RelativeDistance; }
		}

		/// <summary>
		/// Goes through the center point list and removes any points
		/// that are overlapping or staggered around. This creates a
		/// single, smooth line with no setbacks. This function also
		/// sets up the internal distances for the point.
		/// </summary>
		public void Optimize()
		{
			// Ignore if we are blank
			if (Count <= 2)
				return;

			// Create a new list
			CenterPointList cpl = new CenterPointList();
			int count = Count;

			// Always add the first point to the new list
			CenterPoint lastPoint = this[0];
			cpl.Add(lastPoint);

			// Go through the list of the rest of the points
			for (int i = 1; i < count; i++)
			{
				// Calculate the distance between points
				CenterPoint iPoint = this[i];
				float iDistance = lastPoint.GetDistance(iPoint);
				bool foundCloser = false;

				// We then go through the list a second time, going
				// forward and see if there is a closer point to the
				// last point than this one
				for (int j = i + 1; j < count; j++)
				{
					// Get this distance
					CenterPoint jPoint = this[j];
					float jDistance = lastPoint.GetDistance(jPoint);

					// See if this point is closer
					if (jDistance <= iDistance)
					{
						// We are closer, so set it back one point
						// because of the j++ above and start at that
						// line when we are done.
						iPoint = jPoint;
						iDistance = jDistance;
						i = j - 1;
						foundCloser = true;
					}
				}

				// See if we found a closer point
				if (foundCloser)
					continue;

				// We didn't find a closer one, so just add it
				cpl.Add(iPoint);
				lastPoint = iPoint;
			}

			// Go back through the list and build up the distances
			lastPoint = cpl[0];
			lastPoint.RelativeDistance = 0;
			count = cpl.Count;

			Clear();
			Add(lastPoint);

			for (int i = 1; i < count; i++)
			{
				// Calculate the distance
				CenterPoint cp = cpl[i];
				cp.RelativeDistance =
					lastPoint.GetDistance(cp) + lastPoint.RelativeDistance;
				lastPoint = cp;
				Add(cp);
			}
		}
	}
}
