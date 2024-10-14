using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Image levelProgressBar; // Assign in Inspector
    public TMP_Text levelText; // Assign in Inspector
    public TMP_Text unlockedSkillsText; // Assign in Inspector (Optional)

    private PlayerProgression playerProgression;
    private int xpGainedThisGame = 0;
    private int startingXP;
    private int targetXP;
    private int startingLevel;
    private int targetLevel;
    private List<string> newUnlockedSkills = new List<string>();

    void Awake()
    {
        playerProgression = PlayerProgression.Instance;
    }
    // Call this method when the game ends, passing the score from the game
    public void ShowGameOver(int score)
    {
        xpGainedThisGame = score; // Assuming score equals XP gained

        // Calculate starting and target XP
        startingXP = playerProgression.totalXP;
        targetXP = startingXP + xpGainedThisGame;

        // Save the starting level
        startingLevel = playerProgression.playerLevel;

        // Add the XP (this will update the player level and unlock skills)
        playerProgression.AddExperience(xpGainedThisGame);

        // Save the target level
        targetLevel = playerProgression.playerLevel;

        // Determine which skills were newly unlocked
        newUnlockedSkills = GetNewlyUnlockedSkills(startingLevel, targetLevel);

        // Clear unlocked skills text
        unlockedSkillsText.text = "";

        // Start the animation coroutine
        StartCoroutine(AnimateLevelProgress());
    }

    IEnumerator AnimateLevelProgress()
    {
        int currentXP = startingXP;
        int currentLevel = startingLevel;
        int targetXPForNextLevel = GetXPForLevel(currentLevel + 1);
        int xpNeededForCurrentLevel = GetXPForLevel(currentLevel);

        levelText.text = "Level " + currentLevel;
        
        while (currentXP < targetXP)
        {
            // Calculate XP to add per frame
            float animationDurationPerLevel = 2f; // Duration to fill the bar for each level
            int xpForThisLevel = targetXPForNextLevel - xpNeededForCurrentLevel;

            if (xpForThisLevel <= 0)
            {
                xpForThisLevel = 1; // Avoid division by zero
            }

            float xpPerSecond = xpForThisLevel / animationDurationPerLevel;
            float xpToAdd = xpPerSecond * Time.unscaledDeltaTime; // Use unscaledDeltaTime

            currentXP += Mathf.CeilToInt(xpToAdd);

            if (currentXP >= targetXPForNextLevel)
            {
                currentXP = targetXPForNextLevel;
            }
            if (currentXP > targetXP)
            {
                currentXP = targetXP;
            }

            // Update progress bar
            float progress = (float)(currentXP - xpNeededForCurrentLevel) / (xpForThisLevel);
            levelProgressBar.fillAmount = progress;

            yield return null;

            // Check for level up
            if (currentXP >= targetXPForNextLevel && currentLevel < targetLevel)
            {
                // Display unlocked skills for this level
                ShowUnlockedSkillsAtLevel(currentLevel + 1);

                // Wait briefly before starting next level animation
                yield return new WaitForSecondsRealtime(0.5f); // Use WaitForSecondsRealtime

                // Level up
                currentLevel++;
                levelText.text = "Level " + currentLevel;

                // Reset progress bar for next level
                xpNeededForCurrentLevel = targetXPForNextLevel;
                targetXPForNextLevel = GetXPForLevel(currentLevel + 1);

                // Reset fill amount
                levelProgressBar.fillAmount = 0f;
            }
        }

        // Ensure the progress bar is accurate at the end
        int finalXPForThisLevel = targetXPForNextLevel - xpNeededForCurrentLevel;
        if (finalXPForThisLevel <= 0)
        {
            finalXPForThisLevel = 1;
        }
        float finalProgress = (float)(currentXP - xpNeededForCurrentLevel) / finalXPForThisLevel;
        levelProgressBar.fillAmount = finalProgress;
    }

    
    int GetXPForLevel(int level)
    {
        if (playerProgression.levelXPRequirements.ContainsKey(level))
        {
            return playerProgression.levelXPRequirements[level];
        }
        else
        {
            // If level exceeds defined levels, assume XP increases by 1000 per level
            return playerProgression.levelXPRequirements[playerProgression.levelXPRequirements.Count] + (level - playerProgression.levelXPRequirements.Count) * 1000;
        }
    }

    void ShowUnlockedSkillsAtLevel(int level)
    {
        if (playerProgression.skillsUnlockedAtLevel.ContainsKey(level))
        {
            List<string> unlockedSkillsThisLevel = playerProgression.skillsUnlockedAtLevel[level];
            string skillsText = "Unlocked:\n" + string.Join(", ", unlockedSkillsThisLevel);
            unlockedSkillsText.text = skillsText;

            // Optionally, add an animation or effect when new skills are unlocked
        }
    }

    List<string> GetNewlyUnlockedSkills(int startingLevel, int targetLevel)
    {
        List<string> newlyUnlocked = new List<string>();
        for (int lvl = startingLevel + 1; lvl <= targetLevel; lvl++)
        {
            if (playerProgression.skillsUnlockedAtLevel.ContainsKey(lvl))
            {
                newlyUnlocked.AddRange(playerProgression.skillsUnlockedAtLevel[lvl]);
            }
        }
        return newlyUnlocked;
    }
}
