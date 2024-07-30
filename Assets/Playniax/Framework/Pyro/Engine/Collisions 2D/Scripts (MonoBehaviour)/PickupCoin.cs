using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class PickupCoin : CollisionBase2D
    {
        public int playerIndex;
        public AudioProperties[] audioSettings = new AudioProperties[1] { new AudioProperties() };

        public override void OnCollision(CollisionBase2D collision)
        {
            if (collision.id != id) return;

            PlayerData.Get(playerIndex).coins += 1;

            AudioProperties.Play(audioSettings);

            Destroy(collision.gameObject);
        }
    }
}