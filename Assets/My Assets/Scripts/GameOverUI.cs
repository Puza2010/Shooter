using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Slider levelProgressBar; // Assign in Inspector
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
        if (playerProgression == null)
        {
            Debug.LogError("PlayerProgression instance is null during GameOverUI Start!");
        }
        else
        {
            Debug.Log("PlayerProgression started!");
        }
    }
    // Call this method when the game ends, passing the score from the game
    public void ShowGameOver(int score)
    {
        xpGainedThisGame = score; // Assuming score equals XP gained
        if (playerProgression == null)
        {
            Debug.LogError("PlayerProgression instance is null during GameOverUI Start!");
        }
        else
        {
            Debug.Log("PlayerProgression started!");
        }
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

        // Start the animation coroutine
        StartCoroutine(AnimateLevelProgress());
    }

    IEnumerator AnimateLevelProgress()
    {
        int currentXP = startingXP;
        int xpForNextLevel = playerProgression.levelXPRequirements[startingLevel + 1];
        int xpForCurrentLevel = playerProgression.levelXPRequirements[startingLevel];
        levelText.text = "Level " + startingLevel;

        while (currentXP < targetXP)
        {
            currentXP += Mathf.CeilToInt(xpGainedThisGame * Time.deltaTime); // Adjust speed as needed
            if (currentXP > targetXP)
            {
                currentXP = targetXP;
            }

            // Check for level up
            if (currentXP >= xpForNextLevel && startingLevel < targetLevel)
            {
                startingLevel++;
                levelText.text = "Level " + startingLevel;
                xpForCurrentLevel = xpForNextLevel;
                xpForNextLevel = playerProgression.levelXPRequirements.ContainsKey(startingLevel + 1) ? playerProgression.levelXPRequirements[startingLevel + 1] : xpForNextLevel + 1000;
                ShowUnlockedSkillsAtLevel(startingLevel);
            }

            float progress = (float)(currentXP - xpForCurrentLevel) / (xpForNextLevel - xpForCurrentLevel);
            levelProgressBar.value = progress;

            yield return null;
        }

        // Ensure the progress bar is accurate at the end
        levelProgressBar.value = (float)(currentXP - xpForCurrentLevel) / (xpForNextLevel - xpForCurrentLevel);
    }

    void ShowUnlockedSkillsAtLevel(int level)
    {
        if (playerProgression.skillsUnlockedAtLevel.ContainsKey(level))
        {
            List<string> unlockedSkills = playerProgression.skillsUnlockedAtLevel[level];
            string skillsText = "Unlocked:\n" + string.Join(", ", unlockedSkills);
            unlockedSkillsText.text = skillsText;
            // Optionally, display this text in the UI or trigger animations
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
