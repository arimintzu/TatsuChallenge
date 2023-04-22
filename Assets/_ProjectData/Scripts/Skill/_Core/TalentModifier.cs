using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TalentModifier 
{
}

public class EventModifier : TalentModifier
{
    [ListDrawerSettings(ShowIndexLabels = false), PropertyOrder(999)]
    public List<CustomEvent> AddedEvent = new List<CustomEvent>();
}

public class SpecialValueTalent : TalentModifier
{
    public List<AttributeValue> specialAttributes = new List<AttributeValue>();
}

public class StatsTalent : TalentModifier
{
    public List<StatAttributeValue> statsAdded = new List<StatAttributeValue>();
}

public class DefaultValueTalent : TalentModifier
{
    public List<DefaulAttributeValue> modifiedAttributes = new List<DefaulAttributeValue>();
}

public enum DefaultAttributeType
{
    Cooldown,
    ManaCost,
}

public enum Stats
{
    CDReduction,
    ManaCostReduction,
}