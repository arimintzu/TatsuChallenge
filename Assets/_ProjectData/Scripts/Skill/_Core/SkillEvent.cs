using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillEvent
{
    public List<CustomBehaviour> events = new List<CustomBehaviour>();

    public virtual void Do() { }
}

public class OnStartSkill : SkillEvent { }
public class OnUseSkill : SkillEvent { }
public class OnSkillEnded : SkillEvent { }
public class OnPlayerTakeDamage : SkillEvent { }
public class OnPlayerDead : SkillEvent { }
public class OnPlayerAttack : SkillEvent { }
public class OnProjectileHit : SkillEvent { }
public class OnCustomEvent<T> : SkillEvent{ public T param; }

