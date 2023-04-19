using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : Singleton<HUDManager>
{
    public Transform damageNumber;

    public void PopDamage(Transform target, Transform source, float damage, Color color, float duration)
    {
        var instantiated = Instantiate(damageNumber.gameObject, target.position + Vector3.up * 1.5f, Quaternion.identity);

        var damageScript = instantiated.GetComponent<DamageText>();
        if (damageScript)
        {
            damageScript.Pop(source, damage.ToString(), color, duration);
        }
    }
}
