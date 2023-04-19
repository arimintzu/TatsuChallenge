using MEC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class SnackbarManager : Singleton<SnackbarManager>
{
    [ReadOnly] public bool isOpen;
    public float defaultDuration = 2f;
    public CanvasGroup parent;
    public TextMeshProUGUI content;

    private void Start()
    {
        parent.alpha = 0;
    }

    public void Pop(string content)
    {
        Pop(content, defaultDuration);
    }

    CoroutineHandle handler;
    public void Pop(string content, float duration)
    {
        if (isOpen)
        {
            Timing.KillCoroutines(handler);
        }

        isOpen = true;
        gameObject.SetActive(true);

        string text = content;
        this.content.text = text;

        Utilities.PlayForward(parent, 0.3f);
        handler = Timing.RunCoroutine(DelayUnpop(duration));
    }

    public void ForceUnpop()
    {
        if (isOpen)
        {
            Timing.KillCoroutines(handler);
        }

        handler = Timing.RunCoroutine(DelayUnpop(0.1f));
    }

    IEnumerator<float> DelayUnpop(float delay)
    {
        yield return Timing.WaitForSeconds(delay);
        Utilities.PlayReverse(parent, 0.3f);
        isOpen = false;
    }

}
