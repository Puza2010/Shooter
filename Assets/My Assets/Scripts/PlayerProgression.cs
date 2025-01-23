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

    [Header("Super Skill Prefabs")]
    [SerializeField] private GameObject energyBeamPrefab; // Assign in inspector

    [Header("Laser Ring Settings")]
    [SerializeField] private GameObject laserRingPrefab; // Assign in inspector

    [Header("Damage Zone Settings")]
    [SerializeField] private GameObject glowPrefab; // Assign Glow prefab in inspector
    [SerializeField] private Sprite yellowGlowSprite; // Assign yellow glow sprite in inspector

    [Header("Super Skill Icons")]
    public Sprite gunsBlazingIcon;
    public Sprite quadCannonsIcon;
    public Sprite missileBarrageIcon;
    public Sprite recurringShieldIcon;
    public Sprite extraLifeIcon;
    public Sprite autoRepairIcon;
    public Sprite damageZoneIcon;
    public Sprite shockWaveIcon;
    public Sprite laserRingIcon;

    // Add this field to track which super skills have been unlocked for display
    private const string UNLOCKED_SUPER_SKILLS_DISPLAY_KEY = "UnlockedSuperSkillsDisplay";

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

        // Load unlocked super skills
        string unlockedSuperSkillsString = PlayerPrefs.GetString("UnlockedSuperSkills", "");
        if (!string.IsNullOrEmpty(unlockedSuperSkillsString))
        {
            unlockedSuperSkills = new List<string>(unlockedSuperSkillsString.Split(','));
        }
        else
        {
            unlockedSuperSkills = new List<string>();
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
        PlayerPrefs.SetString("UnlockedSuperSkills", string.Join(",", unlockedSuperSkills));
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
        unlockedSuperSkills = new List<string>(); // Reset super skills
        ResetDisplayedSuperSkills(); // Also reset the displayed super skills
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
            gunsBlazingDisables,
            gunsBlazingIcon
        );

        availableSuperSkills.Add("Guns Blazing", gunsBlazingSkill);

        // Initialize Quad Cannons super skill
        List<SuperSkillRequirement> quadCannonsReqs = new List<SuperSkillRequirement>
        {
            new SuperSkillRequirement { skillName = "Cannons", requiredLevel = 5 },
            new SuperSkillRequirement { skillName = "Drone", requiredLevel = 5 }
        };
        
        List<string> quadCannonsDisables = new List<string>
        {
            "Cannons"
        };

        SuperSkill quadCannonsSkill = new SuperSkill(
            "Quad Cannons",
            "Double your side firepower with four devastating cannons!",
            quadCannonsReqs,
            quadCannonsDisables,
            quadCannonsIcon
        );

        availableSuperSkills.Add("Quad Cannons", quadCannonsSkill);

        // Initialize Missile Barrage super skill
        List<SuperSkillRequirement> missileBarrageReqs = new List<SuperSkillRequirement>
        {
            new SuperSkillRequirement { skillName = "Homing Missile", requiredLevel = 5 },
            new SuperSkillRequirement { skillName = "Homing Gun", requiredLevel = 5 }
        };

        List<string> missileBarrageDisables = new List<string>
        {
            "Homing Missile"
        };

        SuperSkill missileBarrageSkill = new SuperSkill(
            "Missile Barrage",
            "Launch three powerful homing missiles at once!",
            missileBarrageReqs,
            missileBarrageDisables,
            missileBarrageIcon
        );

        availableSuperSkills.Add("Missile Barrage", missileBarrageSkill);

        // Add Recurring Shield super skill
        List<SuperSkillRequirement> recurringShieldReqs = new List<SuperSkillRequirement>
        {
            new SuperSkillRequirement { skillName = "Health Upgrade", requiredLevel = 5 },
            new SuperSkillRequirement { skillName = "Shield", requiredLevel = 5 }
        };
        
        List<string> recurringShieldDisables = new List<string>
        {
            "Shield"
        };

        SuperSkill recurringShieldSkill = new SuperSkill(
            "Recurring Shield",
            "Automatically activates a shield every minute for 20 seconds!",
            recurringShieldReqs,
            recurringShieldDisables,
            recurringShieldIcon
        );

        availableSuperSkills.Add("Recurring Shield", recurringShieldSkill);

        // Initialize Extra Life super skill
        List<SuperSkillRequirement> extraLifeReqs = new List<SuperSkillRequirement>
        {
            new SuperSkillRequirement { skillName = "Health Upgrade", requiredLevel = 5 },
            new SuperSkillRequirement { skillName = "Engine Fire", requiredLevel = 5 }
        };

        List<string> extraLifeDisables = new List<string>();  // No skills need to be disabled

        SuperSkill extraLifeSkill = new SuperSkill(
            "Extra Life",
            "Get a second chance! When fatal damage is taken, revive with temporary invulnerability!",
            extraLifeReqs,
            extraLifeDisables,
            extraLifeIcon
        );

        availableSuperSkills.Add("Extra Life", extraLifeSkill);

        // Initialize Auto Repair super skill
        List<SuperSkillRequirement> autoRepairReqs = new List<SuperSkillRequirement>
        {
            new SuperSkillRequirement { skillName = "Health Upgrade", requiredLevel = 5 },
            new SuperSkillRequirement { skillName = "Slow Enemies", requiredLevel = 5 }
        };

        List<string> autoRepairDisables = new List<string>();  // No skills need to be disabled

        SuperSkill autoRepairSkill = new SuperSkill(
            "Auto Repair",
            "Your ship automatically repairs itself, healing 10% of max health every 20 seconds!",
            autoRepairReqs,
            autoRepairDisables,
            autoRepairIcon
        );

        availableSuperSkills.Add("Auto Repair", autoRepairSkill);

        // Initialize Damage Zone super skill
        List<SuperSkillRequirement> damageZoneReqs = new List<SuperSkillRequirement>
        {
            new SuperSkillRequirement { skillName = "Shield", requiredLevel = 5 },
            new SuperSkillRequirement { skillName = "Engine Fire", requiredLevel = 5 }
        };

        List<string> damageZoneDisables = new List<string>();  // No skills need to be disabled

        SuperSkill damageZoneSkill = new SuperSkill(
            "Damage Zone",
            "Create a permanent damaging aura around your ship that harms enemies!",
            damageZoneReqs,
            damageZoneDisables,
            damageZoneIcon
        );

        availableSuperSkills.Add("Damage Zone", damageZoneSkill);

        // Initialize Shock Wave super skill
        List<SuperSkillRequirement> shockWaveReqs = new List<SuperSkillRequirement>
        {
            new SuperSkillRequirement { skillName = "Wrecking Ball", requiredLevel = 5 },
            new SuperSkillRequirement { skillName = "Green Laser", requiredLevel = 4 },
            new SuperSkillRequirement { skillName = "Blue Laser", requiredLevel = 4 }
        };

        List<string> shockWaveDisables = new List<string>
        {
            "Wrecking Ball"
        };

        SuperSkill shockWaveSkill = new SuperSkill(
            "Shock Wave",
            "Release a devastating energy wave that damages all enemies in its path!",
            shockWaveReqs,
            shockWaveDisables,
            shockWaveIcon
        );

        availableSuperSkills.Add("Shock Wave", shockWaveSkill);

        // Initialize Laser Ring super skill
        List<SuperSkillRequirement> laserRingReqs = new List<SuperSkillRequirement>
        {
            new SuperSkillRequirement { skillName = "Red Laser", requiredLevel = 4 },
            new SuperSkillRequirement { skillName = "Blue Laser", requiredLevel = 4 },
            new SuperSkillRequirement { skillName = "Green Laser", requiredLevel = 4 },
            new SuperSkillRequirement { skillName = "Purple Laser", requiredLevel = 4 }
        };

        List<string> laserRingDisables = new List<string>();  // No skills need to be disabled

        SuperSkill laserRingSkill = new SuperSkill(
            "Laser Ring",
            "Create a rotating ring of pure energy that damages enemies!",
            laserRingReqs,
            laserRingDisables,
            laserRingIcon
        );

        availableSuperSkills.Add("Laser Ring", laserRingSkill);
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
        // Add to display list when activated
        AddUnlockedSuperSkillDisplay(superSkillName);

        if (!availableSuperSkills.ContainsKey(superSkillName)) return;

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Only disable Angled Shots and 3 Way Shooter for Guns Blazing
            if (superSkillName == "Guns Blazing")
            {
                // Disable Angled Shots
                var mainSpawner = player.GetComponent<BulletSpawner>();
                if (mainSpawner != null)
                {
                    mainSpawner.angledShotsLevel = 0;
                    Debug.Log("Disabled Angled Shots");
                }

                // Disable 3 Way Shooter
                var multiSpawners = player.GetComponentsInChildren<BulletSpawners>();
                foreach (var spawner in multiSpawners)
                {
                    if (spawner.id == "3 Way Shooter")
                    {
                        spawner.timer.counter = 0;
                        Debug.Log($"Disabled 3 Way Shooter");
                    }
                }
            }
            else if (superSkillName == "Quad Cannons")
            {
                // Disable regular Cannons
                var multiSpawners = player.GetComponentsInChildren<BulletSpawners>();
                foreach (var spawner in multiSpawners)
                {
                    if (spawner.id == "Cannon")
                    {
                        spawner.timer.counter = 0;
                        Debug.Log("Disabled regular Cannons");
                    }
                    else if (spawner.id == "Quad Cannons")
                    {
                        spawner.timer.counter = -1;
                        Debug.Log("Enabled Quad Cannons");
                    }
                }
            }
            else if (superSkillName == "Missile Barrage")
            {
                // Disable regular Homing Missiles
                var missileSpawner = player.GetComponent<SimpleBulletSpawner>();
                if (missileSpawner != null && missileSpawner.id == "Missiles")
                {
                    missileSpawner.timer.counter = 0;
                    Debug.Log("Disabled regular Homing Missiles");
                }

                // Enable Missile Barrage
                var missileBarrageSpawner = player.AddComponent<SimpleBulletSpawner>();
                missileBarrageSpawner.id = "Missile Barrage";
                missileBarrageSpawner.prefab = missileSpawner.prefab; // Use same prefab as regular missiles
                missileBarrageSpawner.timer = new Timer();
                missileBarrageSpawner.timer.interval = 0.5f;
                missileBarrageSpawner.timer.counter = -1;
                Debug.Log("Enabled Missile Barrage");
            }
            else if (superSkillName == "Recurring Shield")
            {
                var collisionState = player.GetComponent<CollisionState>();
                if (collisionState != null)
                {
                    // Activate recurring shield
                    collisionState.ActivateRecurringShield();
                }
            }
            else if (superSkillName == "Extra Life")
            {
                var collisionState = player.GetComponent<CollisionState>();
                if (collisionState != null)
                {
                    // The actual revival logic is handled in CollisionState.Kill()
                    Debug.Log("Extra Life super skill activated");
                }
            }
            else if (superSkillName == "Shock Wave")
            {
                if (player != null && energyBeamPrefab != null)
                {
                    var beam = Instantiate(energyBeamPrefab, player.transform.position, player.transform.rotation, player.transform);
                    var energyBeam = beam.GetComponent<EnergyBeam>();
                    if (energyBeam != null)
                    {
                        energyBeam.ActivateAsSuperSkill(30f); // 30 seconds duration
                        energyBeam.gameObject.SetActive(true);
                    }
                }
            }
            else if (superSkillName == "Laser Ring")
            {
                if (player != null && laserRingPrefab != null)
                {
                    // Create the laser ring as a child of the player
                    var ring = Instantiate(laserRingPrefab, player.transform.position, Quaternion.identity, player.transform);
                    var ringController = ring.GetComponent<RotatingRingController>();
                    if (ringController != null)
                    {
                        // Base damage 50 per second
                        ringController.SetDamage(50f);
                    }
                }
            }
            else if (superSkillName == "Auto Repair")
            {
                var collisionState = player.GetComponent<CollisionState>();
                if (collisionState != null)
                {
                    collisionState.StartAutoRepair();
                }
            }
            else if (superSkillName == "Damage Zone")
            {
                // Find or create the glow effect
                var existingGlow = player.transform.Find("DamageZoneGlow");
                if (existingGlow == null && glowPrefab != null)
                {
                    // Instantiate the glow as a child of the player
                    var glow = Instantiate(glowPrefab, player.transform.position, Quaternion.identity, player.transform);
                    glow.name = "DamageZoneGlow";
                    
                    // Set up the sprite renderer with yellow glow
                    var spriteRenderer = glow.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null && yellowGlowSprite != null)
                    {
                        spriteRenderer.sprite = yellowGlowSprite;
                        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent
                    }
                    
                    // Add the damage zone component
                    var damageZone = glow.AddComponent<DamageZone>();
                    
                    // Scale the glow appropriately
                    glow.transform.localScale = new Vector3(3f, 3f, 3f);
                    
                    // Add circle collider for damage zone if it doesn't exist
                    var collider = glow.GetComponent<CircleCollider2D>();
                    if (collider == null)
                    {
                        collider = glow.AddComponent<CircleCollider2D>();
                        collider.isTrigger = true;
                        collider.radius = 2f;
                    }
                }
            }

            // Enable the super skill
            var singleSpawners = player.GetComponentsInChildren<BulletSpawner>();
            foreach (var spawner in singleSpawners)
            {
                if (spawner.id == superSkillName)
                {
                    spawner.timer.counter = -1;
                    Debug.Log($"Enabled {superSkillName}");
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

    // Add this new method to PlayerProgression class
    public void ResetSuperSkills()
    {
        unlockedSuperSkills.Clear();
        SavePlayerProgress();
    }

    // Add this method to track when a super skill is unlocked
    public void AddUnlockedSuperSkillDisplay(string superSkillName)
    {
        string displayList = PlayerPrefs.GetString(UNLOCKED_SUPER_SKILLS_DISPLAY_KEY, "");
        List<string> displayedSkills = !string.IsNullOrEmpty(displayList) 
            ? new List<string>(displayList.Split(',')) 
            : new List<string>();

        if (!displayedSkills.Contains(superSkillName))
        {
            displayedSkills.Add(superSkillName);
            PlayerPrefs.SetString(UNLOCKED_SUPER_SKILLS_DISPLAY_KEY, string.Join(",", displayedSkills));
            PlayerPrefs.Save();
        }
    }

    // Add this method to get the list of all unlocked super skills for display
    public List<string> GetUnlockedSuperSkillsDisplay()
    {
        string displayList = PlayerPrefs.GetString(UNLOCKED_SUPER_SKILLS_DISPLAY_KEY, "");
        return !string.IsNullOrEmpty(displayList) 
            ? new List<string>(displayList.Split(',')) 
            : new List<string>();
    }

    // Add this method to reset displayed super skills
    public void ResetDisplayedSuperSkills()
    {
        PlayerPrefs.DeleteKey(UNLOCKED_SUPER_SKILLS_DISPLAY_KEY);
        PlayerPrefs.Save();
    }
}
