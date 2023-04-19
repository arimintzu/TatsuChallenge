using MEC;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "TatsuChallenge/Skill", order = 0)]
public class SkillData : SerializedScriptableObject
{
    #region EDITOR DISPLAY
    [BoxGroup("Info"), ShowInInspector, PropertyOrder(-1), DisplayAsString, HideLabel, GUIColor(0, 1, 0)]
    private string Editor_Skill_Name
    {
        get { return "[" + I2.Loc.LocalizationManager.GetTranslation("skill/" + id + "/name") + "]"; }
        set { }
    }
    [BoxGroup("Info"), ShowInInspector, PropertyOrder(-1), DisplayAsString(false), HideLabel]
    private string Editor_Skill_Desc
    {
        get { return I2.Loc.LocalizationManager.GetTranslation("skill/" + id + "/desc"); }
        set { }
    }
    #endregion

    [LabelText("Skill ID")] public string id;
    public Sprite icon;

    [SuffixLabel("seconds", overlay: true), TabGroup("Default")] public float cooldown;
    [TabGroup("Default")] public float manacost;
    [SuffixLabel("seconds", overlay: true), TabGroup("Default")] public float castTime; 
    [HideIf(nameof(targetting), Targetting.NoTarget), TabGroup("Default")] public float castRange;
    [HideIf(nameof(targetting), Targetting.NoTarget), TabGroup("Default")] public bool walkToPoint;
    [TabGroup("Default")] public DamageType damageType;
    [TabGroup("Default")] public Targetting targetting;
    [TabGroup("Default")] public List<AffectedTarget> affectedTargets;
    [TabGroup("Default")] public List<AttributeValue> specialAttributes;
    [TabGroup("Event"), ListDrawerSettings(ShowIndexLabels = false)] public List<SkillEvent> Event = new List<SkillEvent>();
    [TabGroup("Talent"), ListDrawerSettings(ShowIndexLabels = false)] public List<TalentData> TalentData = new List<TalentData>();

    public bool IsTalentUnlocked()
    {
        return false;
    }

    [Button]
    public  void UseSkill()
    {
        Timing.RunCoroutine(_UseSkillRoutine());
    }

    IEnumerator<float> _UseSkillRoutine()
    {
        var startingSkillEvent = Event.FindAll(x => x is OnStartSkill);
        foreach (var eachEvent in startingSkillEvent)
        {
            if (eachEvent != null)
            {
                eachEvent.Do();
            }
        }
        yield return Timing.WaitForSeconds(castTime);

        var useSkillEvent = Event.FindAll(x => x is OnUseSkill);
        foreach (var eachEvent in useSkillEvent)
        {
            if (eachEvent != null)
            {
                eachEvent.Do();
            }
        }
    }

    public void ApplyCooldown()
    {

    }

    public virtual string GetAttributeDescription()
    {
        return string.Empty;
    }
}

[System.Serializable]
public class AttributeValue
{
    public string key;
    public float value;
    public AttributeType type;
    public bool isHidden;
}

public enum AffectedTarget
{
    Self,
    Allies,
    Enemies
}

public enum Targetting
{
    NoTarget, //Instant Cast
    UnitTarget, //Need to click unit
    GroundTarget //Ground Target
}

public enum AttributeType
{
    ExactValue,
    Percentage,
    Second
}

public enum DamageType
{
    Physical,
    Magical,
    Pure
}


public enum StatusEffect
{
    None,
    Freeze,
    Burn,
    Poison,
    Slow
}

public enum DefaultSkillValue
{
    Cooldown,
    CastTime,
    CastPoint,
    CastRange
}
