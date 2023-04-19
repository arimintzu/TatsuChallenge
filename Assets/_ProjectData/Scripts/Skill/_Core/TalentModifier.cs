using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TalentModifier 
{
    [PropertyOrder(998)] public bool addEvent;
    [ListDrawerSettings(ShowIndexLabels = false),
        PropertyOrder(999), ShowIf(nameof(addEvent))] public List<SkillEvent> AddedEvent = new List<SkillEvent>();
}

public class DefaultModifier : TalentModifier { }

public class SpecialValueTalent : TalentModifier
{
    public List<AttributeValue> specialAttributes = new List<AttributeValue>();
}

public class StatsTalent : TalentModifier
{
    public List<AttributeValue> specialAttributes;
}

public class DefaultValueTalent : TalentModifier
{
    public List<AttributeValue> specialAttributes;
}