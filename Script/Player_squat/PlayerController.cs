using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputcontrol;
    private Rigidbody2D rb;
    public PhysicsCheck physicsCheck;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private Vector2 OriginBox;
    private bool isJump;
    public bool canChange;
    public float dirction;

    public Vector2 inputDirection;
    [Header("基本参数")]
    public float speed;
    public float jumpSpeed;
    public float fallMultiplier = 2.5f;//下降修正
    public float lowMultiplier = 2f;//上升修正


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        physicsCheck = GetComponent<PhysicsCheck>();//地面碰撞检测

        inputcontrol = new PlayerInputControl();//调用键盘输入

        spriteRenderer = GetComponent<SpriteRenderer>();

        boxCollider = GetComponent<BoxCollider2D>();
        OriginBox = boxCollider.size;
    }

    //private void OnEnable()
    //{
    //    inputcontrol.Enable();// 人物的显示
    //}
    //private void OnDisable()
    //{
    //    inputcontrol.Disable();//人物的隐藏
    //}

    void Update()//人物的移动
    {
        if (Input.GetKeyDown(KeyCode.Space) && physicsCheck.isGround)
        {
            isJump = true;
        }
        BoxAdjust();
    }
    private void FixedUpdate()
    {
        Move();
        Jump();
    }
    public void Move()//人物移动翻转
    {
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime, rb.velocity.y);
        //if (Input.GetAxisRaw("Horizontal") != 0)
        //{
        //    dirction = Input.GetAxisRaw("Horizontal");
        //}
        if (dirction < 0)
        {
            spriteRenderer.flipX = false;
        }
        if (dirction > 0)
        {
            spriteRenderer.flipX = true;
        }
    }
    private void Jump()//施加跳跃的力
    {
        if (isJump&&!physicsCheck.isGroundHead)
        {
            rb.velocity = Vector2.up * jumpSpeed;
            isJump = false;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowMultiplier - 1) * Time.deltaTime;
        }
    }

    private void BoxAdjust()
    {
        if (physicsCheck.isGround==false)
        {
            boxCollider.size = new Vector2(0.49f, OriginBox.y);
        }
        else
        {
            boxCollider.size = OriginBox;
        }
    }
}
