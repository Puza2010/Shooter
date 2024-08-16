using Playniax.Ignition;
using UnityEngine;
using Playniax.Ignition.UI;

public class PlayerProgress : MonoBehaviour
{
    public int currentLevel = 1;  // Starting level
    public int currentCoins = 0;  // Coins collected in current level
    public int coinsRequiredForNextLevel = 1;  // Coins needed to reach the next level
    public GameProgressBarUI progressBarUI;  // Reference to progress bar

    private EasyGameUI easyGameUI;

    void Start()
    {
        easyGameUI = FindObjectOfType<EasyGameUI>();
        UpdateProgressBar();
    }

    public void AddCoin()
    {
        currentCoins++;
        if (currentCoins >= coinsRequiredForNextLevel)
        {
            LevelUp();
        }
        UpdateProgressBar();
    }

    void LevelUp()
    {
        currentLevel++;
        currentCoins = 0;  // Reset coins for the new level
        coinsRequiredForNextLevel = currentLevel;  // Increase the requirement based on current level

        // Trigger the skill selection panel from EasyGameUI
        easyGameUI.ShowSkillSelectionPanel();

        UpdateProgressBar();
    }

    void UpdateProgressBar()
    {
        // Update the progress bar to reflect the current level progress
        if (progressBarUI != null)
        {
            progressBarUI.SetProgress(currentCoins, coinsRequiredForNextLevel);
        }
    }
}