using System.Collections.Generic;
using UnityEngine;

public class SkillIconManager : MonoBehaviour
{
    public static SkillIconManager Instance { get; private set; }

    public Sprite questionMarkIcon; // Assign in Inspector

    public Sprite mainGunIcon; // Assign in Inspector
    public Sprite cannonsIcon; // Assign in Inspector
    public Sprite speedUpIcon; // Assign in Inspector
    public Sprite weaponSpeedIcon; // Assign in Inspector
    public Sprite homingMissileIcon; // Assign in Inspector
    public Sprite homingGunIcon; // Assign in Inspector
    public Sprite redLaserIcon; // Assign in Inspector
    public Sprite angledShotsIcon; // Assign in Inspector
    public Sprite slowEnemiesIcon; // Assign in Inspector
    public Sprite threeWayShooterIcon; // Assign in Inspector
    public Sprite blueLaserIcon; // Assign in Inspector
    public Sprite healthUpgradeIcon; // Assign in Inspector
    public Sprite wreckingBallIcon; // Assign in Inspector
    public Sprite shieldIcon; // Assign in Inspector
    public Sprite greenLaserIcon; // Assign in Inspector
    public Sprite droneIcon; // Assign in Inspector
    public Sprite slowEnemyBulletsIcon; // Assign in Inspector
    public Sprite purpleLaserIcon; // Assign in Inspector
    public Sprite randomBouncingShotIcon; // Assign in Inspector
    public Sprite extraScoreIcon; // Assign in Inspector

    private Dictionary<string, Sprite> skillIcons = new Dictionary<string, Sprite>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSkillIcons();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeSkillIcons()
    {
        skillIcons["Main Gun"] = mainGunIcon;
        skillIcons["Cannons"] = cannonsIcon;
        skillIcons["Speed Up"] = speedUpIcon;
        skillIcons["Weapon Speed"] = weaponSpeedIcon;
        skillIcons["Homing Missile"] = homingMissileIcon;
        skillIcons["Homing Gun"] = homingGunIcon;
        skillIcons["Red Laser"] = redLaserIcon;
        skillIcons["Angled Shots"] = angledShotsIcon;
        skillIcons["Slow Enemies"] = slowEnemiesIcon;
        skillIcons["3 Way Shooter"] = threeWayShooterIcon;
        skillIcons["Blue Laser"] = blueLaserIcon;
        skillIcons["Health Upgrade"] = healthUpgradeIcon;
        skillIcons["Wrecking Ball"] = wreckingBallIcon;
        skillIcons["Shield"] = shieldIcon;
        skillIcons["Green Laser"] = greenLaserIcon;
        skillIcons["Drone"] = droneIcon;
        skillIcons["Slow Enemy Bullets"] = slowEnemyBulletsIcon;
        skillIcons["Purple Laser"] = purpleLaserIcon;
        skillIcons["Random Bouncing Shot"] = randomBouncingShotIcon;
        skillIcons["Extra Score"] = extraScoreIcon;
    }

    public Sprite GetSkillIcon(string skillName)
    {
        if (skillIcons.ContainsKey(skillName))
        {
            Debug.Log($"Skill icon found for {skillName}");
            return skillIcons[skillName];
        }
        else
        {
            Debug.Log($"No skill icon found for {skillName}, returning question mark icon");
            return questionMarkIcon;
        }
    }
}
