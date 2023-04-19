using MEC;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseProps : MonoBehaviour
{
    [Title("Visual")]
    public Material visualMaterial;
    public List<Renderer> visuals;
    public float hitEffectDuration = 0.1f;
    public float ghostDuration = 0.75f;
    public List<Transform> dieEffects;

    [Title("Source")]
    public AudioSource sfxSource;
    public AudioSource voiceSource;

    [Title("Clips")]
    public List<AudioClip> hitClips;
    public List<AudioClip> dieClips;

    protected Material material;
    protected void Awake()
    {
        material = new Material(visualMaterial);
        foreach (var item in visuals)
        {
            if (!item) continue;

            item.material = material;
        }

        if (material)
        {
            material.DisableKeyword("HITEFFECT_ON");
            material.DisableKeyword("GHOST_ON");
            material.DisableKeyword("FADE_ON");
        }
    }

    public void PlayDieSFX()
    {
        if(Utilities.TryGetRandomFromList<AudioClip>(dieClips, out var result))
        {
            voiceSource.PlayOneShot(result);
        }
    }

    public void PlayDieVFX(Vector3 position)
    {
        if (Utilities.TryGetRandomFromList<Transform>(dieEffects, out var result))
        {
            Instantiate(result.gameObject, position, result.rotation);
        }
    }

    public void PlayHitSFX()
    {
        if (Utilities.TryGetRandomFromList<AudioClip>(hitClips, out var result))
        {
            voiceSource.PlayOneShot(result);
        }
    }

    public void HitVFX()
    {
        if(material)
        {
            material.EnableKeyword("HITEFFECT_ON");
            Timing.RunCoroutine(Utilities.DelayAndDo(hitEffectDuration, () =>
            {
                material.DisableKeyword("HITEFFECT_ON");
            }).CancelWith(gameObject));
        }
    }

    public void VisualGhost(float duration)
    {
        if (material)
        {
            material.EnableKeyword("GHOST_ON");
            float currentValue = 0.1f;
            var tweener = DOTween.To(() => currentValue, (val) =>
            {
                currentValue = val;
                material.SetFloat("_GhostTransparency", val);
            }, 1f, ghostDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

            Timing.RunCoroutine(Utilities.DelayAndDo(duration, () =>
            {
                if(tweener != null)
                {
                    if (tweener.IsActive()) tweener.Kill();
                }

                material.DisableKeyword("GHOST_ON");
            }).CancelWith(gameObject));
        }
    }

    public void Fade(float duration, System.Action OnEndedFading)
    {
        if (material)
        {
            material.EnableKeyword("FADE_ON");
            float currentValue = 0f;
            var tweener = DOTween.To(() => currentValue, (val) =>
            {
                currentValue = val;
                material.SetFloat("_FadeAmount", val);
            }, 1f, duration).SetEase(Ease.OutSine).OnComplete(() =>
            {
                OnEndedFading?.Invoke();
            });

        }
    }
}
