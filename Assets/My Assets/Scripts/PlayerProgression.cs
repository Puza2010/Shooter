using System.Collections.Generic;
using UnityEngine;
using Playniax.Pyro;
using System;
using Playniax.Ignition;
using System.Linq;

public class PlayerProgression : MonoBehaviour
{
    public static PlayerProgression Instance { get; private set; }

    public int playerLevel = 1;
    public int totalXP = 0;
    
    // New flag to track if the player has played the game
    private bool hasPlayedGame = false;
    private bool justPlayedFirstGame = false;
    
    public List<string> initialSkills = new List<string>();

    // Experience required for each level
    public Dictionary<int, int> levelXPRequirements = new Dictionary<int, int>();

    // Skills unlocked at each level
    public Dictionary<int, List<string>> skillsUnlockedAtLevel = new Dictionary<int, List<string>>();

    // List of unlocked skills
    public List<string> unlockedSkills = new List<string>();

    // Add these fields to the PlayerProgression class
    public List<string> unlockedSuperSkills = new List<string>();
    public Dictionary<string, SuperSkill> availableSuperSkills = new Dictionary<string, SuperSkill>();

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
        initialSkills = new List<string> { "Main Gun", "Cannons", "Speed Up", "Weapon Speed", "Homing Missile" };

        // Level 1 unlocked skills (excluding Main Gun since it's always unlocked)
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
        skillsUnlockedAtLevel[18] = new List<string> { "Bouncing Shot" };
        skillsUnlockedAtLevel[20] = new List<string> { "Engine Fire" };

        // Add this to InitializeSkillsUnlockedAtLevel()
        InitializeSuperSkills();
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

        // Load hasPlayedGame flag
        hasPlayedGame = PlayerPrefs.GetInt("HasPlayedGame", 0) == 1;

        // If the player has played the game, unlock skills corresponding to their level
        if (hasPlayedGame)
        {
            for (int lvl = 1; lvl <= playerLevel; lvl++)
            {
                UnlockSkillsAtLevel(lvl);
            }
        }
    }

    // Save player progress to persistent storage
    public void SavePlayerProgress()
    {
        PlayerPrefs.SetInt("PlayerLevel", playerLevel);
        PlayerPrefs.SetInt("TotalXP", totalXP);
        PlayerPrefs.SetString("UnlockedSkills", string.Join(",", unlockedSkills));
        PlayerPrefs.SetInt("HasPlayedGame", hasPlayedGame ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Add experience points and handle leveling up
    public void AddExperience(int xpToAdd)
    {
        totalXP += xpToAdd;

        if (!hasPlayedGame)
        {
            hasPlayedGame = true;
            justPlayedFirstGame = true; // Indicates that the player just played their first game
            UnlockSkillsAtLevel(1);      // Unlock level 1 skills
        }
        else
        {
            justPlayedFirstGame = false;
        }

        CheckForLevelUp();
        SavePlayerProgress();
    }
    
    public bool HasJustPlayedFirstGame()
    {
        return justPlayedFirstGame;
    }
    
    public void ResetJustPlayedFirstGameFlag()
    {
        justPlayedFirstGame = false;
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
    
    public bool HasPlayedGame()
    {
        return hasPlayedGame;
    }
    
    public void SetHasPlayedGame(bool value)
    {
        hasPlayedGame = value;
        SavePlayerProgress();
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

    // Add this to InitializeSkillsUnlockedAtLevel()
    void InitializeSuperSkills()
    {
        // Initialize Guns Blazing super skill
        List<SuperSkillRequirement> gunsBlazingReqs = new List<SuperSkillRequirement>
        {
            new SuperSkillRequirement { skillName = "Angled Shots", requiredLevel = 5 },
            new SuperSkillRequirement { skillName = "3 Way Shooter", requiredLevel = 4 }
        };
        
        List<string> gunsBlazingDisables = new List<string>
        {
            "Angled Shots",
            "3 Way Shooter"
        };

        SuperSkill gunsBlazingSkill = new SuperSkill(
            "Guns Blazing",
            "Unleash a devastating barrage of bullets in 7 directions!",
            gunsBlazingReqs,
            gunsBlazingDisables
        );

        availableSuperSkills.Add("Guns Blazing", gunsBlazingSkill);
    }

    // Add this method to check for newly unlocked super skills
    public List<string> CheckNewlyUnlockedSuperSkills()
    {
        List<string> newlyUnlocked = new List<string>();

        foreach (var superSkill in availableSuperSkills)
        {
            if (!unlockedSuperSkills.Contains(superSkill.Key))
            {
                bool allRequirementsMet = true;
                
                // First check if we have both required skills unlocked at all
                foreach (var req in superSkill.Value.requirements)
                {
                    bool hasSkill = unlockedSkills.Any(s => s.StartsWith($"{req.skillName} Level "));
                    if (!hasSkill)
                    {
                        allRequirementsMet = false;
                        break;
                    }
                }

                // Only check levels if we have all required skills
                if (allRequirementsMet)
                {
                    foreach (var req in superSkill.Value.requirements)
                    {
                        int currentLevel = GetSkillLevel(req.skillName);
                        if (currentLevel < req.requiredLevel)
                        {
                            allRequirementsMet = false;
                            break;
                        }
                    }
                }

                if (allRequirementsMet)
                {
                    newlyUnlocked.Add(superSkill.Key);
                }
            }
        }

        return newlyUnlocked;
    }

    // Add this method to handle super skill activation
    public void ActivateSuperSkill(string superSkillName)
    {
        if (!availableSuperSkills.ContainsKey(superSkillName)) return;

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Enable Guns Blazing
            var singleSpawners = player.GetComponentsInChildren<BulletSpawner>();
            foreach (var spawner in singleSpawners)
            {
                if (spawner.id == superSkillName)
                {
                    spawner.timer.counter = -1;
                    Debug.Log($"Enabled {superSkillName}");
                }
            }

            // Disable 3 Way Shooter
            var multiSpawners = player.GetComponentsInChildren<BulletSpawners>();
            foreach (var spawner in multiSpawners)
            {
                if (spawner.id == "3 Way Shooter")
                {
                    Debug.Log(spawner.timer.counter);
                    spawner.timer.counter = 0;
                    Debug.Log($"Disabled 3 Way Shooter");
                    Debug.Log(spawner.timer.counter);
                }
            }
        }

        unlockedSuperSkills.Add(superSkillName);
        SavePlayerProgress();
    }

    public void DisableSkill(string skillName)
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var bulletSpawner = player.GetComponentInChildren<BulletSpawner>();
            if (bulletSpawner != null)
            {
                if (skillName == "Angled Shots" || skillName == "3 Way Shooter")
                {
                    bulletSpawner.timer.counter = 0;
                }
            }
        }
    }

    // Add this method to PlayerProgression class
    private int GetSkillLevel(string skillName)
    {
        // Get the current level from EasyGameUI's acquired skills
        var currentSkill = EasyGameUI.instance.acquiredSkills
            .Where(s => s.StartsWith(skillName))
            .Select(s => EasyGameUI.instance.skills[s])
            .OrderByDescending(s => s.level)
            .FirstOrDefault();

        int level = currentSkill?.level ?? 0;
        return level;
    }
}
