using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(Transform source, DamageRequest damageRequest, out DamageResult result);
}

[System.Serializable, FoldoutGroup("Damage Request"), HideLabel]
public class DamageRequest
{
    public float finalDamage;
    public float knockback;
    public float stunDuration;
    
    public StatusEffect statusEffectType = StatusEffect.None;
}

[System.Serializable, FoldoutGroup("Juicy Request"), HideLabel]
public class JuicyRequest
{
    [Title("Timestop")]
    public bool timeStopOnHit;
    [ShowIf(nameof(timeStopOnHit))] public float timeStopDuration;

    [Title("Camera Shake")]
    public bool shakeOnHit;
    [ShowIf(nameof(shakeOnHit))] public bool useCustomIntensity;
    [ShowIf(nameof(ShowShakeLevel))] public ShakeLevel shakeLevel;
    [ShowIf(nameof(ShowCustomIntensity))] public float customIntensity;
    [ShowIf(nameof(shakeOnHit))] public float shakeDuration;
    bool ShowShakeLevel => shakeOnHit && !useCustomIntensity;
    bool ShowCustomIntensity => shakeOnHit && useCustomIntensity;

    [Title("Screen Flash")]
    public bool screenFlashOnHit;
    [ShowIf(nameof(screenFlashOnHit))] public ScreenFlash screenFlashType;
}

[System.Serializable]
public class DamageResult
{
    public bool isDead;
    public bool isMissed;
    public bool isStunned;
}

public enum ScreenFlash
{
    None, 
    White,
    Red
}