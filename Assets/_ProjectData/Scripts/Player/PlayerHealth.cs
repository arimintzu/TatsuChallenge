using MEC;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Title("Properties")]
    public float maxHealth = 100f;
    [ReadOnly] public float currentHealth;
    [HideInInspector] public bool die;

    [Title("Invulnerable")]
    public bool invulnerableAfterHit;
    [ShowIf(nameof(invulnerableAfterHit))] public float invulnerableDuration;

    [Title("Effects")]
    public Transform hitEffect;

    Animator _animator;
    private Collider2D _collider;
    private BaseProps _props;
    public UnityEvent OnDie;
    bool isInvulnerable;
    private void Awake()
    {
        _props = GetComponentInParent<BaseProps>();
        currentHealth = maxHealth;
        _collider = GetComponent<Collider2D>();
    }
    public void TakeDamage(Transform source, DamageRequest damageRequest, out DamageResult result)
    {
        result = new DamageResult();
        result.isMissed = true;

        if (isInvulnerable) return;
        if (die) return;

        currentHealth -= damageRequest.finalDamage;

        if (damageRequest.finalDamage > 0)
            HUDManager.Instance.PopDamage(transform, source, damageRequest.finalDamage, Color.red, 2f);

        if (currentHealth > 0)
        {
            if (damageRequest.knockback > 0)
                Knockback(source, damageRequest.knockback);

            if (invulnerableAfterHit)
            {
                if (_props)
                {
                    //_props.VisualGhost(invulnerableDuration);
                }
                Timing.RunCoroutine(_StartInvulnerable().CancelWith(gameObject));
            }

            if (_props)
            {
                _props.PlayHitSFX();
                _props.HitVFX();
            }

            if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        else
        {
            if (_props)
            {
                _props.PlayDieSFX();
                _props.PlayDieVFX(transform.position);
            }

            die = true;
            OnDie?.Invoke();

            GameManager.Instance.GameOver();
        }

        result.isMissed = false;
        result.isDead = die;
    }

    IEnumerator<float> _StartInvulnerable()
    {
        isInvulnerable = true;
        yield return Timing.WaitForSeconds(invulnerableDuration);
        isInvulnerable = false;

        var colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            if (collider.enabled)
            {
                collider.enabled = false;
                yield return Timing.WaitForOneFrame;
                collider.enabled = true;
            }
        }
    }
    public void Knockback(Transform source, float knockForce)
    {
        Vector2 direction = transform.position - source.position;
        direction = direction.normalized;
    }
}
