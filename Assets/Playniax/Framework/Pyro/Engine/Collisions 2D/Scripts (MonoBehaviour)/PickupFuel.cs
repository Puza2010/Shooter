using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class PickupFuel : CollisionBase2D
    {
        public int playerIndex;

        public override void OnCollision(CollisionBase2D collision)
        {
            if (collision.id != id) return;

            //PlayerData.Get(playerIndex).fuel += 100;

            //if (PlayerData.Get(playerIndex).fuel > PlayerData.Get(playerIndex).fuelScale) PlayerData.Get(playerIndex).fuel = PlayerData.Get(playerIndex).fuelScale;

            Destroy(collision.gameObject);
        }
    }
}