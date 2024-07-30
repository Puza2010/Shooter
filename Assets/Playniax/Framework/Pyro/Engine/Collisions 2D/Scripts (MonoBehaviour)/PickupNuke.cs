using UnityEngine;

namespace Playniax.Pyro
{
    public class PickupNuke : CollisionBase2D
    {
        public string emitter = "Nuke";

        public override void OnCollision(CollisionBase2D collision)
        {
            if (collision.id != id) return;

            Nukeable.Nuke();

            CameraFlash.Flash(Camera.main, 3, 1, 1, 1);

            Destroy(collision.gameObject);
        }
    }
}