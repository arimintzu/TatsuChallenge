using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : Singleton<HUDManager>
{
    public Transform damageNumber;

    public void PopDamage(Transform target, float damage, Color color, float duration)
    {
        PopDamage(target, damage.ToString(), color, duration);
    }

    public void PopDamage(Transform target, string damage, Color color, float duration)
    {
        var instantiated = Instantiate(damageNumber.gameObject, target.position + Vector3.up * 1.5f, Quaternion.identity);

        var damageScript = instantiated.GetComponent<DamageText>();
        if (damageScript)
        {
            damageScript.Pop(damage.ToString(), color, duration);
        }
    }
}
