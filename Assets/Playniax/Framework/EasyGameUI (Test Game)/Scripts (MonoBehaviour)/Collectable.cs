using UnityEngine;

namespace Playniax.Ignition.TestGame
{
    public class Collectable : MonoBehaviour
    {
        public int points = 10;
        void Start()
        {
            PlayerData.Get(0).collectables += 1;
        }
    }
}