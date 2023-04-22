using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 1f;
    PlayerInput input;
    Rigidbody2D rb;
    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    public void ApplyMovement()
    {
        if (!input) return;
        var Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //rb.MovePosition(rb.position + input.FrameInput.Move.normalized * moveSpeed * Time.deltaTime);
        rb.velocity = input.FrameInput.Move.normalized * moveSpeed * Time.fixedDeltaTime;
    }
}
