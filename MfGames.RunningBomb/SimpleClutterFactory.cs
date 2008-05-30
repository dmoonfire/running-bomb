using MfGames.Utility;
using System.Drawing;

namespace MfGames.RunningBomb
{
	/// <summary>
	/// A general factory that creates clutter in a junction.
	/// </summary>
	public class SimpleClutterFactory
	:IClutterFactory
	{
		/// <summary>
		/// Populates a junction with clutter, bonuses, and other
		/// stuff.
		/// </summary>
		public void Create(Segment segment)
		{
			// Basically create a random number of modules and place
			// them in the stage.
			MersenneRandom random = segment.ParentJunction.Random;
			Junction junction = segment.ChildJunction;
			int moduleCount = junction.Random.Next(0, 20);

			for (int i = 0; i < moduleCount; i++)
			{
				// Find a random vertex and basically center it
				// somewhere along the line between the center and the
				// vertex.
				int pointCount = junction.InternalShape.PointCount;
				int pointIndex = random.Next(0, pointCount);
				float x = (float) junction.InternalShape.GetX(pointIndex);
				float y = (float) junction.InternalShape.GetY(pointIndex);
				
				// We get a place in the middle, but not too
				// close. Since the junction center is always at (0,
				// 0), this simplifies finding the point.
				float weight = random.NextSingle(0.1f, 0.9f);
				float cx = x * weight;
				float cy = y * weight;

				// Create the mobile
				Create(segment, random, cx, cy);
			}
		}

		/// <summary>
		/// Creates a random mobile object at the given location.
		/// </summary>
		private void Create(
			Segment segment, MersenneRandom random, float x, float y)
		{
			// Create a housing bubble
			PointF p = segment.ChildJunctionPoint;
			HousingBubble hb = new HousingBubble();		   
			hb.Point = new PointF(p.X + x, p.Y + y);
			hb.Radius = random.NextSingle(10, 50);

			// Add it
			segment.ParentJunction.Mobiles.Add(hb);
		}
	}
}
