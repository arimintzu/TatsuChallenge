using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerManager : Singleton<DestroyerManager>
{
    public void DoDestroy(GameObject gameObject)
    {
        DoDestroy(gameObject, 0f);
    }
    public void DoDestroy(GameObject gameObject, float delay)
    {
        Timing.RunCoroutine(Utilities.DelayAndDo(delay, () =>
        {
            if(gameObject) Destroy(gameObject);
        }));
    }
}
