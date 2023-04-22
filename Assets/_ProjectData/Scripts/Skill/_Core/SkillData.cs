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
        get { return "[" + Name() +"]"; }
        set { }
    }
    [BoxGroup("Info"), ShowInInspector, PropertyOrder(-1), DisplayAsString(false), HideLabel]
    private string Editor_Skill_Desc
    {
        get { return Description(); }
        set { }
    }
    public IEnumerable<System.Type> GetFilteredTypeList()
    {
        var q = typeof(CustomEvent).Assembly.GetTypes()
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsGenericTypeDefinition)
            .Where(x => typeof(CustomEvent).IsAssignableFrom(x));

        q = q.AppendWith(typeof(OnCustomEvent<>).MakeGenericType(typeof(GameObject)));

        return q;
    }

    #endregion
    [LabelText("Skill ID")] public string id;
    public Sprite icon;
    [TabGroup("Combat")] public DamageType damageType;
    [TabGroup("Combat")] public Targetting targetting;
    [TabGroup("Combat")] public List<AffectedTarget> affectedTargets;
    [TabGroup("Variables"), MinValue(1)] public int maxLevel;
    [TabGroup("Variables"), SuffixLabel("seconds", overlay: true)] public List<float> cooldown;
    [TabGroup("Variables")] public List<float> manaCost;
    [TabGroup("Variables"), SuffixLabel("seconds", overlay: true)] public float castTime; 
    [TabGroup("Variables"), HideIf(nameof(targetting), Targetting.NoTarget)] public List<float> castRange;
    [TabGroup("Variables"), HideIf(nameof(targetting), Targetting.NoTarget)] public bool walkToPoint;
    [TabGroup("Variables")] public List<AttributeValue> attributeValues = new List<AttributeValue>();
    [TabGroup("Event"), ListDrawerSettings(ShowIndexLabels = false), TypeFilter(nameof(GetFilteredTypeList))] public List<CustomEvent> Event = new List<CustomEvent>();

    public float GetCooldown(int level) => GetValueFromList(cooldown, level);
    public float GetManaCost(int level) => GetValueFromList(manaCost, level);
    public float GetCastRange(int level) => GetValueFromList(castRange, level);

    public float GetValueFromList(List<float> list, int level)
    {
        if (list == null) return 0f;
        if (list.Count == 0) return 0f;
        if (Utilities.OutOfIndex(list.Count, level))
            level = list.Count - 1;

        return list[level];
    }

    public string Name()
    {
        return I2.Loc.LocalizationManager.GetTranslation("skill/" + id + "/name");
    }

    public string Description()
    {
        return I2.Loc.LocalizationManager.GetTranslation("skill/" + id + "/desc");
    }

    public void UseSkill(EventRequest request, int currentLevel)
    {
        Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();
        foreach (var val in attributeValues)
            keyValuePairs.Add(val.key, val.GetValue(currentLevel));

        foreach (var attr in request.valueCollection.Keys.ToList())
        {
            if(request.valueCollection.ContainsKey(attr))
            {
                if (keyValuePairs.ContainsKey(attr)) keyValuePairs[attr] += request.valueCollection[attr];
                else keyValuePairs.Add(attr, request.valueCollection[attr]);
            }
        }
        request.valueCollection = keyValuePairs;
        Timing.RunCoroutine(_UseSkillRoutine(request));
    }
    IEnumerator<float> _UseSkillRoutine(EventRequest request)
    {
        var temporaryEvent = new List<CustomEvent>(Event.ToArray());

        for (int i = 0; i < temporaryEvent.Count; i++)
        {
            var evt = temporaryEvent[i];
            for (int j = 0; j < request.events.Count; j++)
            {
                var requestedEvent = request.events[j];

                for (int k = 0; k < requestedEvent.actions.Count; k++)
                {
                    var action = requestedEvent.actions[k];
                    evt.actions.Add(action);
                    //var findAct = evt.actions.Find(x => x.key == action.key);
                    //if(findAct != null)
                    //{
                    //    findAct = action;
                    //}
                }
            }
        }

        var startingSkillEvent = temporaryEvent.FindAll(x => x is OnStartSkill);
        foreach (var eachEvent in startingSkillEvent)
        {
            if (eachEvent != null)
            {
                var handler = Timing.RunCoroutine(eachEvent.DoRoutine(request));
                yield return Timing.WaitUntilDone(handler);
            }
        }
        
        yield return Timing.WaitForSeconds(castTime);

        var useSkillEvent = temporaryEvent.FindAll(x => x is OnUseSkill);
        foreach (var eachEvent in useSkillEvent)
        {
            if (eachEvent != null)
            {
                var handler = Timing.RunCoroutine(eachEvent.DoRoutine(request));
                yield return Timing.WaitUntilDone(handler);
            }
        }
    }
}

[System.Serializable]
public class AttributeValue
{
    [PropertyOrder(0), ShowIf(nameof(ShowHidden))] public string key;
    public ValueType type;
    [ShowIf(nameof(ShowHidden))] public bool isHidden;
    [PropertyOrder(99)] public List<float> value = new List<float>();

    public float GetValue(int level)
    {
        if (value == null) return 0f;
        if (value.Count == 0) return 0f;
        if(Utilities.OutOfIndex(value.Count, level))
            level = value.Count - 1;

        return value[level];
    }

    public virtual bool ShowHidden()
    {
        return true;
    }
}

[System.Serializable]
public class StatAttributeValue : AttributeValue
{
    public Stats stats;
    public override bool ShowHidden()
    {
        return false;
    }
}
[System.Serializable]
public class DefaulAttributeValue : AttributeValue
{
    public DefaultAttributeType attributeType;
    public override bool ShowHidden()
    {
        return false;
    }
}

public enum AffectedTarget
{
    Self,
    Allies,
    Enemies
}

public enum Target
{
    Caster,
    Target,
}

public enum Targetting
{
    NoTarget, //Instant Cast
    NearestUnitTarget, //Find closest 
    RandomUnitInArea, //Find random in area
    AroundCaster,
    UnitTarget,//Need to click unit
    GroundTarget //Ground Target
}

public enum ValueType
{
    ExactValue,
    Percentage,
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

[System.Serializable]
public class ReferencedAttribute
{
    public bool inputValue;
    [ShowIf(nameof(inputValue))] public float value;
    [HideIf(nameof(inputValue))] public string key;

    public float GetValue(Dictionary<string, float> collection)
    {
        if (inputValue) return value;
        if (collection == null) return 0f;
        if (collection.Count == 0) return 0f;
        if (!collection.ContainsKey(key)) return 0f; 
        return collection[key];
    }
}