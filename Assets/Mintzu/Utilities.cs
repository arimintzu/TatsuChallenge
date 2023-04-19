using DG.Tweening;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utilities
{
    public static IEnumerator<float> DelayAndDo(float delay, System.Action Do)
    {
        yield return Timing.WaitForSeconds(delay);
        Do?.Invoke();
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
    }
    public static IEnumerator<float> DelayFrameAndDo(int frameCount, System.Action Do)
    {
        yield return Timing.WaitForOneFrame * frameCount;
        Do?.Invoke();
    }

    public static string GenerateStringFromGuid(int length)
    {
        var randomGuid = Guid.NewGuid().ToString();
        return randomGuid.Substring(0, length);
    }

    public static void Destroy(GameObject target)
    {
        var iDestroyable = target.GetComponent<IDestroyable>();
        if (iDestroyable != null)
        {
            iDestroyable.Destroy();
        }

        else
        {
            Destroy(target);
        }
    }
    public static string RankPlayer(int rank)
    {
        if (rank % 10 == 1 && rank != 11)
        {
            return rank + "st";
        }
        else if (rank % 10 == 2 && rank != 12)
        {
            return rank + "nd";
        }
        else if (rank % 10 == 3 && rank != 13)
        {
            return rank + "rd";
        }
        else
        {
            return rank + "th";
        }
    }

    public static int GetLayerNumber(this LayerMask mask)
    {
        int value = mask.value;
        if (value == 0) return 0;  // Early out
        for (int l = 1; l < 32; l++)
            if ((value & (1 << l)) != 0) return l;  // Bitwise
        return -1;  // This line won't ever be reached but the compiler needs it
    }
    public static bool OutOfIndex(int count, int index)
    {
        if (index >= count) return true;
        else return false;
    }

    public static bool TryGetRandomFromList<T>(List<T> list, out T result)
    {
        result = default(T);
        List<T> nonNullElements = new List<T>();
        foreach (T element in list)
        {
            if (element != null)
            {
                nonNullElements.Add(element);
            }
        }

        if (nonNullElements.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, nonNullElements.Count);
            result = nonNullElements[randomIndex];

            if (result != null)
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    public static bool Randomize(float applyChance, float from = 0f, float to = 100f) => applyChance >= UnityEngine.Random.Range(from, to);

    public static void PlayReverse(CanvasGroup parent, float duration)
    {
        DOTween.To(() => parent.alpha, (val) => parent.alpha = val, 0, duration);
    }

    public static void PlayForward(CanvasGroup parent, float duration)
    {
        DOTween.To(() => parent.alpha, (val) => parent.alpha = val, 1, duration);
    }
}
