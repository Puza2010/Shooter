using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Image levelProgressBar; // Assign in Inspector
    public TMP_Text levelText; // Assign in Inspector
    public Transform unlockedSkillsContainer; // Assign in Inspector
    public GameObject skillIconPrefab; // Assign in Inspector

    private PlayerProgression playerProgression;
    private int xpGainedThisGame = 0;
    private int startingXP;
    private int targetXP;
    private int startingLevel;
    private int targetLevel;
    private List<string> newUnlockedSkills = new List<string>();
    private HashSet<string> displayedSkills = new HashSet<string>();

    void Awake()
    {
        playerProgression = PlayerProgression.Instance;
    }
    // Call this method when the game ends, passing the score from the game
    public void ShowGameOver(int score)
    {
        displayedSkills.Clear();
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

        // Include Level 1 skills if the player just played their first game
        if (playerProgression.HasJustPlayedFirstGame())
        {
            if (playerProgression.skillsUnlockedAtLevel.ContainsKey(1))
            {
                newUnlockedSkills.AddRange(playerProgression.skillsUnlockedAtLevel[1]);
            }
            // Reset the flag
            playerProgression.ResetJustPlayedFirstGameFlag();
        }

        // Clear previous skill icons
        foreach (Transform child in unlockedSkillsContainer)
        {
            Destroy(child.gameObject);
        }

        // Display newly unlocked skills
        foreach (string skillName in newUnlockedSkills)
        {
            if (!displayedSkills.Contains(skillName))
            {
                displayedSkills.Add(skillName);
                Debug.Log($"Displaying unlocked skill: {skillName}");
                GameObject iconGO = Instantiate(skillIconPrefab, unlockedSkillsContainer);
                Image iconImage = iconGO.GetComponent<Image>();
                iconImage.sprite = SkillIconManager.Instance.GetSkillIcon(skillName);
            }
        }

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
                currentLevel++;
                levelText.text = "Level " + currentLevel;

                // Display unlocked skills for this level
                ShowUnlockedSkillsAtLevel(currentLevel);

                // Wait briefly before continuing the animation
                yield return new WaitForSecondsRealtime(0.5f);

                // Update XP thresholds for next level
                xpNeededForCurrentLevel = targetXPForNextLevel;
                targetXPForNextLevel = GetXPForLevel(currentLevel + 1);

                // Reset fill amount for next level
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

            // Instantiate skill icons
            foreach (string skillName in unlockedSkillsThisLevel)
            {
                if (!displayedSkills.Contains(skillName))
                {
                    displayedSkills.Add(skillName);
                    GameObject iconGO = Instantiate(skillIconPrefab, unlockedSkillsContainer);
                    Image iconImage = iconGO.GetComponent<Image>();
                    iconImage.sprite = SkillIconManager.Instance.GetSkillIcon(skillName);
                }
            }
        }
    }

    List<string> GetNewlyUnlockedSkills(int startingLevel, int targetLevel)
    {
        List<string> newlyUnlocked = new List<string>();

        // Include Level 1 skills if the player just played their first game
        if (playerProgression.HasJustPlayedFirstGame())
        {
            if (playerProgression.skillsUnlockedAtLevel.ContainsKey(1))
            {
                newlyUnlocked.AddRange(playerProgression.skillsUnlockedAtLevel[1]);
            }
        }

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
