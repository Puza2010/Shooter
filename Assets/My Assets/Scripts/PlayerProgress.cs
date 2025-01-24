using Playniax.Ignition;
using UnityEngine;
using Playniax.Ignition.UI;

public class PlayerProgress : MonoBehaviour
{

    public int currentLevel = 1;
    public int currentCoins = 0;
    // public int coinsRequiredForNextLevel = 5; // Set this to the correct initial value for the first level
    public int coinsRequiredForNextLevel = 1; // Set this to the correct initial value for the first level
    public GameProgressBarUI progressBarUI; // Reference to the Progress Bar UI component

    void Start()
    {
        EasyGameUI.instance.OnSkillSelected("Main Gun Level 1");
        // EasyGameUI.instance.OnSkillSelected("Red Laser Level 4");
        EasyGameUI.instance.OnSkillSelected("Blue Laser Level 4");
        EasyGameUI.instance.OnSkillSelected("Green Laser Level 4");
        // EasyGameUI.instance.OnSkillSelected("Purple Laser Level 4");
        // EasyGameUI.instance.OnSkillSelected("Angled Shots Level 5");
        // EasyGameUI.instance.OnSkillSelected("Cannons Level 5");
        EasyGameUI.instance.OnSkillSelected("3 Way Shooter Level 5");
        // EasyGameUI.instance.OnSkillSelected("Speed Up Level 5");
        // EasyGameUI.instance.OnSkillSelected("Health Upgrade Level 5");
        // EasyGameUI.instance.OnSkillSelected("Homing Missile Level 5");
        // EasyGameUI.instance.OnSkillSelected("Homing Gun Level 5");
        EasyGameUI.instance.OnSkillSelected("Wrecking Ball Level 5");
        // EasyGameUI.instance.OnSkillSelected("Shield Level 5");
        // EasyGameUI.instance.OnSkillSelected("Slow Enemies Level 5");
        // EasyGameUI.instance.OnSkillSelected("Slow Enemy Bullets Level 5");
        // EasyGameUI.instance.OnSkillSelected("Weapon Speed Level 5");
        // EasyGameUI.instance.OnSkillSelected("Drone Level 5");
        // EasyGameUI.instance.OnSkillSelected("Bouncing Shot Level 5");
        // EasyGameUI.instance.OnSkillSelected("Engine Fire Level 5");

        InitializeSuperSkillsForTesting();

        // Initialize the correct coins required for the next level
        coinsRequiredForNextLevel = coinsRequiredForNextLevel + currentLevel; // This will correctly set it to 5 for level 1

        // Initialize the progress bar at the start
        if (progressBarUI != null)
        {
            progressBarUI.SetProgress(currentCoins, coinsRequiredForNextLevel);
        }
    }

    private void InitializeSuperSkillsForTesting()
    {
        // First, make sure PlayerProgression exists
        if (PlayerProgression.Instance != null)
        {
            // Activate all super skills
            // PlayerProgression.Instance.ActivateSuperSkill("Guns Blazing");
            // PlayerProgression.Instance.ActivateSuperSkill("Quad Cannons");
            // PlayerProgression.Instance.ActivateSuperSkill("Missile Barrage");
            // PlayerProgression.Instance.ActivateSuperSkill("Recurring Shield");
            // PlayerProgression.Instance.ActivateSuperSkill("Extra Life");
            // PlayerProgression.Instance.ActivateSuperSkill("Shock Wave");
            // PlayerProgression.Instance.ActivateSuperSkill("Laser Ring");
            // PlayerProgression.Instance.ActivateSuperSkill("Auto Repair");
            // PlayerProgression.Instance.ActivateSuperSkill("Damage Zone");
            // PlayerProgression.Instance.ActivateSuperSkill("Double Wrecking Ball");
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
            // coinsRequiredForNextLevel = 1 + currentLevel;
            coinsRequiredForNextLevel = 1;
            
            // Show skill selection panel directly
            EasyGameUI.instance.ShowSkillSelectionPanel();
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBarUI != null)
        {
            progressBarUI.SetProgress(currentCoins, coinsRequiredForNextLevel);
        }
    }
}
