using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlocksMenuUI : MonoBehaviour
{
    public GameObject unlocksPanel; // Assign the UnlocksPanel in the Inspector
    public GameObject skillSlotPrefab; // Assign the SkillSlot prefab
    public Transform skillGrid; // Assign the SkillGrid transform
    public Transform superSkillGrid; // Assign in Inspector
    public GameObject superSkillSlotPrefab; // Assign in Inspector

    private List<GameObject> skillSlots = new List<GameObject>();
    private List<GameObject> superSkillSlots = new List<GameObject>();

    void Start()
    {
        unlocksPanel.SetActive(false);
        PopulateSkillGrid();
        PopulateSuperSkillGrid();
    }

    // Open the UnlocksPanel
    public void OpenUnlocksPanel()
    {
        UpdateSkillGridDisplay();
        unlocksPanel.SetActive(true);
    }

    // Close the UnlocksPanel
    public void CloseUnlocksPanel()
    {
        unlocksPanel.SetActive(false);
    }

    // Populate the grid with skill slots
    void PopulateSkillGrid()
    {
        int totalSkills = 20; // 4 rows × 5 columns
        for (int i = 0; i < totalSkills; i++)
        {
            GameObject slot = Instantiate(skillSlotPrefab, skillGrid);
            skillSlots.Add(slot);
        }
    }

    // Populate the super skill grid with slots
    void PopulateSuperSkillGrid()
    {
        int totalSuperSkills = 9; // Total number of super skills
        for (int i = 0; i < totalSuperSkills; i++)
        {
            GameObject slot = Instantiate(superSkillSlotPrefab, superSkillGrid);
            superSkillSlots.Add(slot);
        }
    }

    // Update the skill slots display based on player progress
    void UpdateSkillGridDisplay()
    {
        var playerProgression = PlayerProgression.Instance;
        var unlockedSkills = playerProgression.unlockedSkills;
        int playerLevel = playerProgression.playerLevel;
        bool hasPlayedGame = playerProgression.HasPlayedGame(); // We'll create this method

        // Define the skills and their unlock levels in the order they appear in the grid
        List<SkillUnlockInfo> skillUnlockInfos = new List<SkillUnlockInfo>
        {
            new SkillUnlockInfo("Main Gun", 0),
            new SkillUnlockInfo("Cannons", 1),
            new SkillUnlockInfo("Speed Up", 1),
            new SkillUnlockInfo("Weapon Speed", 1),
            new SkillUnlockInfo("Homing Missile", 1),
            new SkillUnlockInfo("Homing Gun", 2),
            new SkillUnlockInfo("Red Laser", 2),
            new SkillUnlockInfo("Angled Shots", 3),
            new SkillUnlockInfo("Slow Enemies", 4),
            new SkillUnlockInfo("3 Way Shooter", 5),
            new SkillUnlockInfo("Blue Laser", 6),
            new SkillUnlockInfo("Health Upgrade", 7),
            new SkillUnlockInfo("Wrecking Ball", 8),
            new SkillUnlockInfo("Shield", 9),
            new SkillUnlockInfo("Green Laser", 10),
            new SkillUnlockInfo("Drone", 12),
            new SkillUnlockInfo("Slow Enemy Bullets", 14),
            new SkillUnlockInfo("Purple Laser", 16),
            new SkillUnlockInfo("Bouncing Shot", 18),
            new SkillUnlockInfo("Engine Fire", 20),
        };

        for (int i = 0; i < skillSlots.Count; i++)
        {
            GameObject slot = skillSlots[i];
            Image skillIcon = slot.transform.Find("SkillIcon").GetComponent<Image>();
            TMP_Text skillText = slot.transform.Find("SkillText").GetComponent<TMP_Text>();

            if (i < skillUnlockInfos.Count)
            {
                SkillUnlockInfo skillInfo = skillUnlockInfos[i];
                bool isUnlocked = unlockedSkills.Contains(skillInfo.skillName);

                if (isUnlocked)
                {
                    // Set the skill icon and name
                    skillIcon.sprite = GetSkillIcon(skillInfo.skillName);
                    skillIcon.color = Color.white;
                    skillText.text = skillInfo.skillName;
                }
                else
                {
                    // Display level requirement
                    skillIcon.sprite = GetQuestionMarkIcon();
                    skillIcon.color = Color.gray;
                    if (skillInfo.skillName == "Main Gun")
                    {
                        skillIcon.sprite = GetSkillIcon("Main Gun");
                        skillIcon.color = Color.white;
                        skillText.text = "Main Gun";
                    }
                    else
                    {
                        skillText.text = "Lvl " + skillInfo.unlockLevel;
                    }
                }
            }
            else
            {
                // Hide extra slots if any
                slot.SetActive(false);
            }
        }

        UpdateSuperSkillsDisplay();
    }

    // Update super skills display
    void UpdateSuperSkillsDisplay()
    {
        var playerProgression = PlayerProgression.Instance;
        var unlockedSuperSkills = playerProgression.unlockedSuperSkills;
        var availableSuperSkills = playerProgression.availableSuperSkills;

        int index = 0;
        foreach (var superSkill in availableSuperSkills)
        {
            if (index < superSkillSlots.Count)
            {
                GameObject slot = superSkillSlots[index];
                Image skillIcon = slot.transform.Find("SkillIcon").GetComponent<Image>();
                TMP_Text skillNameText = slot.transform.Find("SkillName").GetComponent<TMP_Text>();
                TMP_Text requirementsText = slot.transform.Find("Requirements").GetComponent<TMP_Text>();

                bool isUnlocked = unlockedSuperSkills.Contains(superSkill.Key);

                if (isUnlocked)
                {
                    // Show the super skill icon and details
                    skillIcon.sprite = GetSuperSkillIcon(superSkill.Key);
                    skillIcon.color = Color.white;
                    skillNameText.text = superSkill.Key;
                    
                    // Create requirements text
                    string reqText = "Requires:\n";
                    foreach (var req in superSkill.Value.requirements)
                    {
                        reqText += $"{req.skillName} Lvl {req.requiredLevel}\n";
                    }
                    requirementsText.text = reqText;
                }
                else
                {
                    // Show question mark
                    skillIcon.sprite = GetQuestionMarkIcon();
                    skillIcon.color = Color.gray;
                    skillNameText.text = "";
                    requirementsText.text = "";
                }
            }
            index++;
        }
    }

    // Helper method to get the skill icon sprite
    Sprite GetSkillIcon(string skillName)
    {
        // Implement a method to get the sprite based on the skill name
        // You might have a dictionary mapping skill names to sprites
        return SkillIconManager.Instance.GetSkillIcon(skillName);
    }

    // Helper method to get the question mark icon
    Sprite GetQuestionMarkIcon()
    {
        // Return a sprite representing a question mark
        return SkillIconManager.Instance.questionMarkIcon;
    }

    // Helper method to get the super skill icon sprite
    Sprite GetSuperSkillIcon(string superSkillName)
    {
        return SkillIconManager.Instance.GetSuperSkillIcon(superSkillName);
    }
}

public class SkillUnlockInfo
{
    public string skillName;
    public int unlockLevel;

    public SkillUnlockInfo(string skillName, int unlockLevel)
    {
        this.skillName = skillName;
        this.unlockLevel = unlockLevel;
    }
}
