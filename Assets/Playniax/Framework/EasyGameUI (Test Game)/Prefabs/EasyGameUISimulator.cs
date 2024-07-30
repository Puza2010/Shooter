using UnityEngine;

namespace Playniax.Ignition
{
    /*

        To see if the game is paused use:

        Timing.Paused

        ( returns True or False )

    */

    public class EasyGameUISimulator : MonoBehaviour
    {
        public int playerIndex;

        private void Awake()
        {
            if (EasyGameUI.instance && EasyGameUI.instance.isLastLevel) print("Player just entered last level");
        }

        public void LevelUp()
        {
            if (EasyGameUI.instance) EasyGameUI.instance.LevelUp();         // Call this when the player successfully finishes a level
        }

        public void LifeLoss()
        {
            PlayerData.Get(playerIndex).lives -= 1;                         // Use the built-in counter to keep track of how many lifes the player has left

            if (PlayerData.Get(playerIndex).lives <= 0) GameOver();         // No lifes left? Game Over
        }

        public void GameOver()
        {
            if (EasyGameUI.instance) EasyGameUI.instance.GameOver();        // Call this for Game Over
        }

        public void Score()
        {
            PlayerData.Get(playerIndex).scoreboard += 10;                   // Use the built-in counter to keep track of how much points the player scores
        }
    }
}