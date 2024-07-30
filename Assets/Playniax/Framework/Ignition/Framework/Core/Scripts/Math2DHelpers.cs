using UnityEngine;

namespace Playniax.Ignition
{
    // Collection of 2D math functions.
    public class Math2DHelpers
    {
		/*
				public static float GetAngle(GameObject a, GameObject b)
				{
					//if (a == null || b == null) return Random.Range(0, 359) * Mathf.Deg2Rad;

					return Mathf.Atan2(a.transform.position.y - b.transform.position.y, a.transform.position.x - b.transform.position.x);
				}
		*/

		// Returns the angle between vector a & b.
		public static float GetAngle(Vector3 a, Vector3 b)
		{
			return Mathf.Atan2(a.y - b.y, a.x - b.x);
		}

        // Returns the angle between GameObject a & b.
        public static float GetAngle(GameObject a, GameObject b)
		{
			return GetAngle(a.transform.position, b.transform.position);
		}

        // Returns the angle between vector a & b in degrees.
        public static float GetAngle360(Vector2 a, Vector2 b)
		{
			Vector2 direction = b - a;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			if (angle < 0f) angle += 360f;
			return angle;
		}

        // Returns the angle between GameObject a & b in degrees.
        public static float GetAngle360(GameObject a, GameObject b)
		{
			return GetAngle360(a.transform.position, b.transform.position);
		}

		// Returns whether the line intersects with circle or not.
		public static bool Intersect(float cx, float cy, float radius, float x1, float y1, float x2, float y2)
		{
			float d;
			float dx = x2 - x1;
			float dy = y2 - y1;
			float ld = Mathf.Sqrt((dx * dx) + (dy * dy));
			float lux = dx / ld;
			float luy = dy / ld;
			float dx1 = cx - (x1 - lux * radius);
			float dy1 = cy - (y1 - luy * radius);

			d = Mathf.Sqrt((dx1 * dx1) + (dy1 * dy1));
			dx1 = dx1 / d;
			dy1 = dy1 / d;

			float dx2 = cx - (x2 + lux * radius);
			float dy2 = cy - (y2 + luy * radius);

			d = Mathf.Sqrt((dx2 * dx2) + (dy2 * dy2));
			dx2 = dx2 / d;
			dy2 = dy2 / d;

			float dot1 = (dx1 * lux) + (dy1 * luy);
			float dot2 = (dx2 * lux) + (dy2 * luy);
			float px = x1 - cx;
			float py = y1 - cy;

			float distsq = Mathf.Abs((dx * py - px * dy) / ld);

			return ((dot1 >= 0 && dot2 <= 0) || (dot1 <= 0 && dot2 >= 0)) && (distsq <= radius);
		}
		
		// Returns whether point is inside the rectangle or not.
		public static bool PointInsideRect(float pointX, float pointY, float x, float y, float width, float height, float pivotX = .5f, float pivotY = .5f)
        {
            x -= width * pivotX;
            y -= height * pivotY;

            float leftX = x;
            float rightX = x + width;
            float topY = y;
            float bottomY = y + height;

            return leftX <= pointX && pointX <= rightX && topY <= pointY && pointY <= bottomY;
        }
	}
}