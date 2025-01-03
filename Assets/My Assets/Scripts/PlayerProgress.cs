using Playniax.Ignition;
using UnityEngine;
using Playniax.Ignition.UI;

public class PlayerProgress : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentCoins = 0;
    public int coinsRequiredForNextLevel = 5; // Set this to the correct initial value for the first level
    public GameProgressBarUI progressBarUI; // Reference to the Progress Bar UI component

    void Start()
    {
        // Initialize the correct coins required for the next level
        coinsRequiredForNextLevel = coinsRequiredForNextLevel + currentLevel; // This will correctly set it to 5 for level 1

        // Initialize the progress bar at the start
        if (progressBarUI != null)
        {
            progressBarUI.SetProgress(currentCoins, coinsRequiredForNextLevel);
        }
    }

    public void AddCoin()
    {
        currentCoins++;
        CheckLevelUp();
        UpdateProgressBar();
    }

    private void CheckLevelUp()
    {
        // Only level up when currentCoins is equal to or greater than the required amount
        if (currentCoins >= coinsRequiredForNextLevel)
        {
            // Subtract the required coins for leveling up
            currentCoins -= coinsRequiredForNextLevel;

            // Increase the level
            currentLevel++;

            // Calculate the next level's coin requirement
            coinsRequiredForNextLevel = 1 + currentLevel;
            
            LevelUp(); // Trigger the level-up process
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBarUI != null)
        {
            progressBarUI.SetProgress(currentCoins, coinsRequiredForNextLevel);
        }
    }

    private void LevelUp()
    {
        // Show the skill selection panel (or any other level-up logic)
        EasyGameUI.instance.ShowSkillSelectionPanel();
    }
}
