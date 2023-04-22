using MEC;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomEvent
{
    [HideInInspector] public EventRequest eventRequest;
    [PropertyOrder(999)] public List<CustomAction> actions = new List<CustomAction>();

    public void Do(EventRequest request)
    {
        Timing.RunCoroutine(DoRoutine(request));
    }
    public IEnumerator<float> DoRoutine(EventRequest request)
    {
        foreach (var act in actions)
        {
            if (act != null)
            {
                act.Do(request);
                if (act is Wait)
                {
                    yield return Timing.WaitForSeconds(((Wait)act).waitTime);
                }
                else if (act is WaitForFrame)
                {
                    yield return Timing.WaitForOneFrame * ((WaitForFrame)act).frameCount;
                }
            }
        }
    }
}

public class OnStartSkill : CustomEvent { }
public class OnUseSkill : CustomEvent { }
public class OnSkillEnded : CustomEvent { }
public class OnPlayerTakeDamage : CustomEvent { }
public class OnPlayerDead : CustomEvent { }
public class OnPlayerAttack : CustomEvent { }
public class OnUpgrade : CustomEvent { }
public class OnHit : CustomEvent { }
public class OnCustomEvent<T> : CustomEvent{ public T param; }

[System.Serializable]
public class EventRequest
{
    public Transform caster;
    public Transform target;
    public List<Transform> targets = new List<Transform>();
    public List<CustomEvent> events = new List<CustomEvent>();
    public Dictionary<string, float> valueCollection = new Dictionary<string, float>(); 
}

[System.Serializable]
public class EventResult
{
    public bool isDead;
}
