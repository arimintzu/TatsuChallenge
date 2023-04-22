using MEC;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public abstract class CustomAction
{
    [HorizontalGroup(width: 0.5f)] public string key;
    [HorizontalGroup(width: 0.25f), HideLabel] public int priority = 0;
    [HorizontalGroup(width: 0.25f), HideLabel] public bool active = true;
    [ShowIf(nameof(NeedTarget))] public Target target;

    public virtual void Do(EventRequest evt) { }

    public CustomAction()
    {

    }

    public virtual Transform GetTarget(EventRequest evt)
    {
        if (target == Target.Caster) return evt.caster;
        else return evt.target;
    }

    public virtual bool NeedTarget()
    {
        return true;
    }
}

public class IncreaseHealth : CustomAction
{
    public ReferencedAttribute healAmount = new ReferencedAttribute();
    public bool isPercentage;

    public override void Do(EventRequest evt)
    {
        base.Do(evt);

        var target = GetTarget(evt);
        var iHealable = target.GetComponent<IHealable>();
        if(iHealable != null)
        {
            iHealable.Heal(healAmount.GetValue(evt.valueCollection), isPercentage);
        }
    }
}

public class SpawnVFX : CustomAction
{
    public GameObject spawnedEffect;
    public bool rescale;
    [ShowIf(nameof(rescale))] public Vector3 scale = Vector3.one;

    public override void Do(EventRequest evt)
    {
        base.Do(evt);

        var target = GetTarget(evt);
        var spawned = EZ_Pooling.EZ_PoolManager.Spawn(spawnedEffect.transform, target.position, spawnedEffect.transform.rotation);
        if(rescale) spawned.transform.localScale = scale;
    }
}

public class ActivateTrail : CustomAction
{
    public GameObject trail;
    public bool rescale;
    public float duration;
    [ShowIf(nameof(rescale))] public Vector3 scale = Vector3.one;

    public override void Do(EventRequest evt)
    {
        base.Do(evt);

        var target = GetTarget(evt);
        var spawned = EZ_Pooling.EZ_PoolManager.Spawn(trail.transform, target.position, trail.transform.rotation);
        if (rescale) spawned.transform.localScale = scale;

        spawned.parent = target;

        Timing.RunCoroutine(Utilities.DelayAndDo(duration, () =>
        {
            Utilities.Destroy(spawned.gameObject);
        }));
    }
}
public class Wait : CustomAction
{
    [SuffixLabel("seconds", overlay: true)] public float waitTime;
}

public class WaitForFrame : CustomAction
{
    [SuffixLabel("frame", overlay: true)] public float frameCount;
}

public enum SoundChannel
{
    Master,
    BGM,
    SFX,
    Voice
}
public class PlaySound : CustomAction
{
    public SoundChannel channel;
    public AudioClip clip;
    public bool modifyDb;
    [Wrap(0f, 2f), ShowIf(nameof(modifyDb))] public float desiredDb = 1f;

    public override void Do(EventRequest evt)
    {
        base.Do(evt);

        var target = GetTarget(evt);
        if(!modifyDb)
            AudioSource.PlayClipAtPoint(clip, target.position);
        else
            AudioSource.PlayClipAtPoint(clip, target.position, desiredDb);
    }
}

public class AOEDamage : CustomAction
{
    public ReferencedAttribute damage = new ReferencedAttribute();
    public ReferencedAttribute radius = new ReferencedAttribute();
    public LayerMask layer;
    public bool debugArea;
    public List<string> collidedTags = new List<string>() { "Enemy" };

    public OnHit HitEvent = new OnHit(); 
    public override void Do(EventRequest evt)
    {
        base.Do(evt);
        var center = GetTarget(evt);

        EventRequest eventRequest = new EventRequest();

        eventRequest.caster = evt.caster;
        eventRequest.valueCollection = evt.valueCollection;

        if (debugArea) Utilities.DrawCircle(center.position, radius.GetValue(evt.valueCollection), Color.red, 2f);
        var victims = Physics2D.OverlapCircleAll(center.position, radius.GetValue(evt.valueCollection), layer);
        foreach (var victim in victims)
        {
            if (!collidedTags.Contains(victim.tag)) continue;
            var iDamageable = victim.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                iDamageable.TakeDamage(evt.caster, new DamageRequest() { finalDamage = damage.GetValue(evt.valueCollection) }, out _);
            }

            eventRequest.target = victim.transform;
            HitEvent.Do(eventRequest);
        }
    }
}
public class Damage : CustomAction
{
    public ReferencedAttribute damage = new ReferencedAttribute();

    public OnHit HitEvent = new OnHit();
    public override void Do(EventRequest evt)
    {
        base.Do(evt);
        var target = GetTarget(evt);

        var iDamageable = target.GetComponent<IDamageable>();
        if(iDamageable != null)
        {
            iDamageable.TakeDamage(evt.caster, new DamageRequest() { finalDamage = damage.GetValue(evt.valueCollection) }, out _);
            HitEvent.Do(evt);
        }
    }
}
public class AOESingleDamage : CustomAction
{
    public ReferencedAttribute damage = new ReferencedAttribute();
    public ReferencedAttribute targetCount = new ReferencedAttribute();
    public float delayStrike = 0.1f;

    public OnHit HitEvent = new OnHit();

    public override bool NeedTarget()
    {
        return false;
    }

    public override void Do(EventRequest evt)
    {
        base.Do(evt);

        Timing.RunCoroutine(_Do(evt));
    }

    IEnumerator<float> _Do(EventRequest evt)
    {
        EventRequest temp = evt;
        for (int i = 0; i < evt.targets.Count; i++)
        {
            if (i > targetCount.GetValue(evt.valueCollection) - 1) continue;
            var target = evt.targets[i];
            yield return Timing.WaitForSeconds(delayStrike);

            var iDamageable = target.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                iDamageable.TakeDamage(evt.caster, new DamageRequest() { finalDamage = damage.GetValue(evt.valueCollection) }, out _);
                temp.target = target;
                HitEvent.Do(temp);
            }
        }
    }
}

public class ChainDamage : CustomAction
{
    public ReferencedAttribute damage = new ReferencedAttribute();
    public ReferencedAttribute targetCount = new ReferencedAttribute();
    public float delayStrike = 0.1f;
    public GameObject trail;
    public bool rescale;
    [ShowIf(nameof(rescale))] public Vector3 scale = Vector3.one;
    public OnHit HitEvent = new OnHit();

    public override bool NeedTarget()
    {
        return false;
    }

    public override void Do(EventRequest evt)
    {
        base.Do(evt);

        Timing.RunCoroutine(_Do(evt));
    }

    IEnumerator<float> _Do(EventRequest evt)
    {
        //SpawnTrail VFX
        EventRequest temp = evt;
        Transform spawnedVFX = null;
        for (int i = 0; i < evt.targets.Count; i++)
        {
            if (i > targetCount.GetValue(evt.valueCollection) - 1) continue;
            var target = evt.targets[i];

            if(spawnedVFX == null)
            {
                if (trail)
                {
                    spawnedVFX = EZ_Pooling.EZ_PoolManager.Spawn(trail.transform, target.position, trail.transform.rotation);
                    if (rescale) spawnedVFX.transform.localScale = scale;
                }
            }

            spawnedVFX.DOMove(target.position, delayStrike);
            var iDamageable = target.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                iDamageable.TakeDamage(evt.caster, new DamageRequest() { finalDamage = damage.GetValue(evt.valueCollection) }, out _);
                temp.target = target;
                HitEvent.Do(temp);
            }

            yield return Timing.WaitForSeconds(delayStrike);
        }

        if (spawnedVFX)
        {
            yield return Timing.WaitForSeconds(1f);
            Utilities.Destroy(spawnedVFX.gameObject);
        }
    }
}
public class BlinkForward : CustomAction
{
    public ReferencedAttribute range = new ReferencedAttribute();

    public override void Do(EventRequest evt)
    {
        base.Do(evt);
        var target = GetTarget(evt);

        var rb = target.GetComponent<Rigidbody2D>();
        if (rb)
        {
            var dir = rb.velocity.normalized;
            var newPos = rb.position + dir * range.GetValue(evt.valueCollection);
            rb.MovePosition(newPos);
        }
    }
}