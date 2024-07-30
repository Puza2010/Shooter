using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Portals
{
    public class BlackHoleController : MonoBehaviour
    {
        public Timer timer;
        public BlackHole blackHole;

        void Awake()
        {
            if (blackHole == null) blackHole = GetComponent<BlackHole>();
        }
        void Update()
        {
            if (blackHole == null) return;

            if (blackHole.state == -1) return;

            if (blackHole.state == 0 && timer.Update())
            {
                blackHole.state = 1;
            }
            else if (blackHole.state == 0 && timer.counter == 0)
            {
                Destroy(blackHole.gameObject);
            }
        }
    }
}