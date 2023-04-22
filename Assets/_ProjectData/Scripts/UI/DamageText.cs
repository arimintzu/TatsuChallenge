using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    public Vector2 rangePower;
    TextMeshPro tm;

    private void Awake()
    {
        tm = GetComponent<TextMeshPro>();
    }

    public void Pop(string content, Color color, float duration)
    {
        tm.text = content;
        tm.color = color;

        transform.DOLocalMoveY(transform.localPosition.y + 0.5f, duration / 2f);
        DOTween.To(() => tm.color.a, ChangeColorAlpha, 0f, duration).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void ChangeColorAlpha(float newAlpha)
    {
        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, newAlpha);
    }
}
