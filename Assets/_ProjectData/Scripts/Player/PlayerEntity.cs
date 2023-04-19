using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : MonoBehaviour
{
    public PlayerHealth health;
    public BaseProps props;

    private void Start()
    {
        GetComponent();
    }

    [Button]
    public void GetComponent()
    {
        health = GetComponentInParent<PlayerHealth>();
        props = GetComponentInParent<BaseProps>();
    }
}
