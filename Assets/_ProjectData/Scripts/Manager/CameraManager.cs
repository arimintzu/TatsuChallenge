using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraManager : Singleton<CameraManager>
{
    public CinemachineVirtualCamera CurrentActive
    {
        get
        {
            return CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.
                VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        }
    }

    public void Shake(ShakeLevel shakeLevel, float duration)
    {
        Shake(GetIntensity(shakeLevel), duration);
    }

    float GetIntensity(ShakeLevel shakeLevel)
    {
        switch(shakeLevel)
        {
            case ShakeLevel.None: default: return 0;
            case ShakeLevel.Light: return 1f;
            case ShakeLevel.Medium: return 2f;
            case ShakeLevel.Heavy: return 3f;
            case ShakeLevel.Earthquake: return 4f;
        }
    }

    DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> shakeTween;
    public void Shake(float intensity, float duration)
    {
        var perlin = CurrentActive.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = intensity;

        if(shakeTween != null)
        {
            if(shakeTween.IsActive())
            {
                shakeTween.Kill();
            }
        } 

        shakeTween = DOTween.To(() => perlin.m_AmplitudeGain, (newValue) =>
        {
            perlin.m_AmplitudeGain = newValue;
        }, 0f, duration);
    }
}

public enum ShakeLevel
{
    None, 
    Light, 
    Medium, 
    Heavy, 
    Earthquake
}
