using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth;
    [ReadOnly, ShowInInspector] private float currentHealth;
    bool die;
    public void TakeDamage(Transform source, DamageRequest damageRequest, out DamageResult result)
    {
        result = new DamageResult();
        result.isMissed = true;

        if (die) return;

        currentHealth -= damageRequest.finalDamage;

        if (damageRequest.finalDamage > 0)
            HUDManager.Instance.PopDamage(transform, damageRequest.finalDamage, Color.red, 1f);

        if (currentHealth > 0)
        {
        }
        else
        {
        }

        result.isMissed = false;
        result.isDead = die;
    }
}
