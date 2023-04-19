using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomBehaviour
{
    public virtual void Do() { }
}

public class IncreaseHealth : CustomBehaviour
{
    public float healAmount;

    public override void Do()
    {
        base.Do();
        Debug.Log("Heal");
    }
}

public class SpawnVFX : CustomBehaviour
{
    public GameObject spawnedEffect;

    public override void Do()
    {
        base.Do();
        Debug.Log("VFX");
    }
}

public class PlaySound : CustomBehaviour
{
    public AudioClip clip;

    public override void Do()
    {
        base.Do();
        Debug.Log("SFX");
    }
}

public class AOEDamage : CustomBehaviour
{
    public float damage;
    public float radius;

    public override void Do()
    {
        base.Do();
        Debug.Log("AOE");
    }
}

public class ApplyStatusEffect : CustomBehaviour
{
    public StatusEffect statusEffect;
    public float duration;

    public override void Do()
    {
        base.Do();
        Debug.Log("Status Effect");
    }
}

public class FireProjectile : CustomBehaviour
{
    public GameObject projectilePrefab;
    public int projectileCount = 1;
    [SuffixLabel("seconds", overlay: true)] public float delayEachProjectile;
    [SuffixLabel("seconds", overlay: true)] public float duration;
    public float speed;
    public int piercingCount;
    [HideIf(nameof(piercingCount), -1)] public bool explodeOnDestroy;

    public OnProjectileHit OnProjectileHit = new OnProjectileHit();
    public override void Do()
    {
        base.Do();
        Debug.Log("Fire");
    }
}

public class InstantStrike : CustomBehaviour
{
    public int targetCount = 1;
    public float delayEachProjectile;

    public override void Do()
    {
        base.Do();
        Debug.Log("InstantStrike");
    }
}

public class Dash : CustomBehaviour
{
    public int targetCount = 1;
    public float delayEachProjectile;

    public override void Do()
    {
        base.Do();
        Debug.Log("Dash");
    }
}

public class Teleport : CustomBehaviour
{
    public int targetCount = 1;
    public float delayEachProjectile;

    public override void Do()
    {
        base.Do();
        Debug.Log("Tele");
    }
}

public class ActivateShield : CustomBehaviour
{
    public float absorbValue;
    public float absorbPercentage;
    public DamageType absorbedType;

    public float radius;

    public override void Do()
    {
        base.Do();
        Debug.Log("InstantStrike");
    }
}

