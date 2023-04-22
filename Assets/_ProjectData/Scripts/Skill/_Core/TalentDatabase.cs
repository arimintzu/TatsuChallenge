using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Talent Database", menuName = "TatsuChallenge/Talent Database", order = 11)]
public class TalentDatabase : ScriptableObject
{
    public List<TalentData> db;
}