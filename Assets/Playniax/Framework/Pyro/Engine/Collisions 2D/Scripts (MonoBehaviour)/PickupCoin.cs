using Playniax.Ignition;
using UnityEngine;

namespace Playniax.Pyro
{
    public class PickupCoin : CollisionBase2D
    {
        public int playerIndex;
        public AudioProperties[] audioSettings = new AudioProperties[1] { new AudioProperties() };

        private PlayerProgress playerProgress;

        private void Start()
        {
            playerProgress = FindObjectOfType<PlayerProgress>();

            if (playerProgress == null)
            {
                Debug.LogError("PlayerProgress component not found in the scene. Please ensure that it is added to a GameObject.");
            }
        }

        public override void OnCollision(CollisionBase2D collision)
        {
            if (collision.id != id) return;

            if (playerProgress == null)
            {
                playerProgress = FindObjectOfType<PlayerProgress>();

                if (playerProgress == null)
                {
                    Debug.LogError("PlayerProgress component not found in the scene.");
                    return; // Exit if PlayerProgress is not found
                }
            }

            playerProgress.AddCoin(); // Call the AddCoin method to update the player's progress

            PlayerData.Get(playerIndex).coins += 1; // Update player data with the coin count

            AudioProperties.Play(audioSettings); // Play coin pickup sound

            Destroy(collision.gameObject); // Destroy the coin game object
        }
    }
}