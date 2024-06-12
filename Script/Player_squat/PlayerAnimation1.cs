using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation1 : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
    }
    private void Update()
    {
        SetAnimation();
    }
    private void SetAnimation()
    {
        anim.SetFloat("velocityX", Mathf.Abs(rb.velocity.x));
        //anim.SetFloat("velocityY", Mathf.Abs(rb.velocity.y));
        anim.SetBool("isGround", physicsCheck.isGround);
    }
}
