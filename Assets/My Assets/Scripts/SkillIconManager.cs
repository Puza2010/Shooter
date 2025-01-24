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
    public Sprite engineFireIcon; // Assign in Inspector
    public Sprite gunsBlazingIcon; // Assign in Inspector
    public Sprite quadCannonsIcon; // Assign in Inspector
    public Sprite missileBarrageIcon; // Assign in Inspector
    public Sprite recurringShieldIcon; // Assign in Inspector
    public Sprite extraLifeIcon; // Assign in Inspector
    public Sprite shockWaveIcon; // Assign in Inspector
    public Sprite laserRingIcon; // Assign in Inspector
    public Sprite autoRepairIcon; // Assign in Inspector
    public Sprite damageZoneIcon; // Assign in Inspector
    public Sprite doubleWreckingBallIcon; // Assign in Inspector

    private Dictionary<string, Sprite> skillIcons = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> superSkillIcons = new Dictionary<string, Sprite>();

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
        skillIcons["Bouncing Shot"] = randomBouncingShotIcon;
        skillIcons["Engine Fire"] = engineFireIcon;

        superSkillIcons["Guns Blazing"] = gunsBlazingIcon;
        superSkillIcons["Quad Cannons"] = quadCannonsIcon;
        superSkillIcons["Missile Barrage"] = missileBarrageIcon;
        superSkillIcons["Recurring Shield"] = recurringShieldIcon;
        superSkillIcons["Extra Life"] = extraLifeIcon;
        superSkillIcons["Shock Wave"] = shockWaveIcon;
        superSkillIcons["Laser Ring"] = laserRingIcon;
        superSkillIcons["Auto Repair"] = autoRepairIcon;
        superSkillIcons["Damage Zone"] = damageZoneIcon;
        superSkillIcons["Double Wrecking Ball"] = doubleWreckingBallIcon;
    }

    public Sprite GetSkillIcon(string skillName)
    {
        if (skillIcons.ContainsKey(skillName))
        {
            return skillIcons[skillName];
        }
        else
        {
            return questionMarkIcon;
        }
    }

    public Sprite GetSuperSkillIcon(string superSkillName)
    {
        if (superSkillIcons.ContainsKey(superSkillName))
        {
            return superSkillIcons[superSkillName];
        }
        return questionMarkIcon;
    }
}
