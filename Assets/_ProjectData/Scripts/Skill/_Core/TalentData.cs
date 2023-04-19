using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Talent", menuName = "TatsuChallenge/Talent", order = 1)]
public class TalentData : SerializedScriptableObject
{
    #region EDITOR DISPLAY
    [BoxGroup("Info"), ShowInInspector, PropertyOrder(-1), DisplayAsString, HideLabel, GUIColor(0, 1, 0)]
    private string Editor_Skill_Name
    {
        get { return "[" + I2.Loc.LocalizationManager.GetTranslation("talent/" + id + "/name") + "]"; }
        set { }
    }
    [BoxGroup("Info"), ShowInInspector, PropertyOrder(-1), DisplayAsString(false), HideLabel]
    private string Editor_Skill_Desc
    {
        get { return I2.Loc.LocalizationManager.GetTranslation("talent/" + id + "/desc"); }
        set { }
    }
    #endregion

    [LabelText("Talent ID")] public string id;
    public Sprite icon;

    public List<TalentModifier> talents = new List<TalentModifier>();
}
