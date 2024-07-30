using UnityEngine;

namespace Playniax.Pyro
{
    public class Randomizer
    {
        public enum Position { None, RandomLeft, RandomRight, RandomTop, RandomBottom, RandomLeftOrRight, RandomTopOrBottom, Random };

        public static Vector2 GetPosition(Position setting, Vector2 size = default, Vector2 margin = default, Vector2 position = default)
        {
            if (setting == Position.RandomLeft)
            {
                return GetPosition(0);
            }
            else if (setting == Position.RandomRight)
            {
                return GetPosition(1);
            }
            else if (setting == Position.RandomTop)
            {
                return GetPosition(2);
            }
            else if (setting == Position.RandomBottom)
            {
                return GetPosition(3);
            }
            else if (setting == Position.RandomLeftOrRight)
            {
                return GetPosition(Random.Range(0, 2));
            }
            else if (setting == Position.RandomTopOrBottom)
            {
                return GetPosition(Random.Range(2, 4));
            }
            else if (setting == Position.Random)
            {
                return GetPosition(Random.Range(0, 4));
            }
            else
            {
                return position;
            }

            Vector2 GetPosition(int segment)
            {
                // 0 = left
                // 1 = right
                // 2 = top
                // 3 = bottom

                var min = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0 - Camera.main.transform.position.z));
                var max = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0 - Camera.main.transform.position.z));

                min.x -= size.x;
                max.x += size.x;

                min.y += size.y;
                max.y -= size.y;

                if (segment == 0)
                {
                    position.x = min.x;
                    position.y = Random.Range(min.y + size.y - margin.y, max.y - size.y + margin.y);
                }
                else if (segment == 1)
                {
                    position.x = max.x;
                    position.y = Random.Range(min.y + size.y - margin.y, max.y - size.y + margin.y);
                }
                else if (segment == 2)
                {
                    position.x = Random.Range(min.x - size.x + margin.x, max.x + size.x - margin.x);
                    position.y = min.y;
                }
                else if (segment == 3)
                {
                    position.x = Random.Range(min.x - size.x + margin.x, max.x + size.x - margin.x);
                    position.y = max.y;
                }

                return position;
            }
        }
    }

}