using System.Collections.Generic;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    public static PlayerProgression Instance { get; private set; }

    public int playerLevel = 1;
    public int totalXP = 0;

    // Experience required for each level
    public Dictionary<int, int> levelXPRequirements = new Dictionary<int, int>();

    // Skills unlocked at each level
    public Dictionary<int, List<string>> skillsUnlockedAtLevel = new Dictionary<int, List<string>>();

    // List of unlocked skills
    public List<string> unlockedSkills = new List<string>();

    void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeLevelXPRequirements();
            InitializeSkillsUnlockedAtLevel();
            LoadPlayerProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initialize the XP requirements for each level
    void InitializeLevelXPRequirements()
    {
        levelXPRequirements[1] = 0;
        levelXPRequirements[2] = 1000;
        levelXPRequirements[3] = 2000;
        levelXPRequirements[4] = 3000;
        levelXPRequirements[5] = 4000;
        levelXPRequirements[6] = 5000;
        levelXPRequirements[7] = 6000;
        levelXPRequirements[8] = 8000;
        levelXPRequirements[9] = 10000;
        levelXPRequirements[10] = 12000;
        levelXPRequirements[11] = 14000;
        levelXPRequirements[12] = 16000;
        levelXPRequirements[13] = 18000;
        levelXPRequirements[14] = 20000;
        levelXPRequirements[15] = 24000;
        levelXPRequirements[16] = 28000;
        levelXPRequirements[17] = 32000;
        levelXPRequirements[18] = 36000;
        levelXPRequirements[19] = 40000;
        levelXPRequirements[20] = 50000;
    }

    // Initialize which skills are unlocked at each level
    void InitializeSkillsUnlockedAtLevel()
    {
        skillsUnlockedAtLevel[1] = new List<string> { "Cannons", "Speed Up", "Weapon Speed", "Homing Missile" };
        skillsUnlockedAtLevel[2] = new List<string> { "Homing Gun", "Red Laser" };
        skillsUnlockedAtLevel[3] = new List<string> { "Angled Shots" };
        skillsUnlockedAtLevel[4] = new List<string> { "Slow Enemies" };
        skillsUnlockedAtLevel[5] = new List<string> { "3 Way Shooter" };
        skillsUnlockedAtLevel[6] = new List<string> { "Blue Laser" };
        skillsUnlockedAtLevel[7] = new List<string> { "Health Upgrade" };
        skillsUnlockedAtLevel[8] = new List<string> { "Wrecking Ball" };
        skillsUnlockedAtLevel[9] = new List<string> { "Shield" };
        skillsUnlockedAtLevel[10] = new List<string> { "Green Laser" };
        skillsUnlockedAtLevel[12] = new List<string> { "Drone" };
        skillsUnlockedAtLevel[14] = new List<string> { "Slow Enemy Bullets" };
        skillsUnlockedAtLevel[16] = new List<string> { "Purple Laser" };
        skillsUnlockedAtLevel[18] = new List<string> { "Random Bouncing Shot" };
        skillsUnlockedAtLevel[20] = new List<string> { "Extra Score" };
    }

    // Load player progress from persistent storage
    void LoadPlayerProgress()
    {
        playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        totalXP = PlayerPrefs.GetInt("TotalXP", 0);

        // Load unlocked skills
        string unlockedSkillsString = PlayerPrefs.GetString("UnlockedSkills", "");
        if (!string.IsNullOrEmpty(unlockedSkillsString))
        {
            unlockedSkills = new List<string>(unlockedSkillsString.Split(','));
        }
        else
        {
            unlockedSkills = new List<string> { "Main Gun" }; // Main Gun is unlocked by default
        }

        // Ensure skills corresponding to current level are unlocked
        for (int lvl = 1; lvl <= playerLevel; lvl++)
        {
            UnlockSkillsAtLevel(lvl);
        }
    }

    // Save player progress to persistent storage
    public void SavePlayerProgress()
    {
        PlayerPrefs.SetInt("PlayerLevel", playerLevel);
        PlayerPrefs.SetInt("TotalXP", totalXP);
        PlayerPrefs.SetString("UnlockedSkills", string.Join(",", unlockedSkills));
        PlayerPrefs.Save();
    }

    // Add experience points and handle leveling up
    public void AddExperience(int xpToAdd)
    {
        totalXP += xpToAdd;
        CheckForLevelUp();
    }

    // Check if the player has enough XP to level up
    void CheckForLevelUp()
    {
        int newLevel = playerLevel;
        foreach (var level in levelXPRequirements.Keys)
        {
            if (totalXP >= levelXPRequirements[level])
            {
                newLevel = level;
            }
            else
            {
                break;
            }
        }

        if (newLevel > playerLevel)
        {
            // Player has leveled up
            for (int lvl = playerLevel + 1; lvl <= newLevel; lvl++)
            {
                UnlockSkillsAtLevel(lvl);
            }
            playerLevel = newLevel;
            SavePlayerProgress();
        }
    }

    // Unlock skills at a specific level
    void UnlockSkillsAtLevel(int level)
    {
        if (skillsUnlockedAtLevel.ContainsKey(level))
        {
            foreach (var skill in skillsUnlockedAtLevel[level])
            {
                if (!unlockedSkills.Contains(skill))
                {
                    unlockedSkills.Add(skill);
                }
            }
        }
    }
    
    public void ResetProgress()
    {
        playerLevel = 1;
        totalXP = 0;
        unlockedSkills = new List<string> { "Main Gun" }; // Reset to default unlocked skills
        SavePlayerProgress();
    }
    
    void OnApplicationQuit()
    {
        SavePlayerProgress();
    }
}
