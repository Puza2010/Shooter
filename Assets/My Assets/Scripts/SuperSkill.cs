using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SuperSkillRequirement
{
    public string skillName;
    public int requiredLevel;
}

[System.Serializable]
public class SuperSkill
{
    public string name;
    public string description;
    public List<SuperSkillRequirement> requirements;
    public List<string> skillsToDisable;
    public Sprite icon;

    public SuperSkill(string name, string description, List<SuperSkillRequirement> requirements, List<string> skillsToDisable)
    {
        this.name = name;
        this.description = description;
        this.requirements = requirements;
        this.skillsToDisable = skillsToDisable;
    }
} 