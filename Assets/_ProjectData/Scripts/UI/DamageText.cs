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
    Rigidbody2D rb;

    private void Awake()
    {
        tm = GetComponent<TextMeshPro>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Pop(Transform source, string content, Color color, float duration)
    {
        tm.text = content;
        tm.color = color;

        Vector2 direction = Vector2.zero;

        if (source == null)
        {
            direction = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(1f, 1.5f));
        }
        else
        {
            bool left = source.position.x < transform.position.x;
            if (left) direction = new Vector2(1, UnityEngine.Random.Range(1f, 1.5f));
            else direction = new Vector2(-1, UnityEngine.Random.Range(1f, 1.5f));
        }

        rb.AddForce(direction * UnityEngine.Random.Range(rangePower.x, rangePower.y), ForceMode2D.Impulse);

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
