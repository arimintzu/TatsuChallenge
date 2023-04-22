using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Database", menuName = "TatsuChallenge/Skill Database", order = 11)]
public class SkillDatabase : ScriptableObject
{
    public List<SkillData> db;
}
