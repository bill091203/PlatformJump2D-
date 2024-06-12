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
    [Header("��������")]
    public float speed;
    public float jumpSpeed;
    public float fallMultiplier = 2.5f;//�½�����
    public float lowMultiplier = 2f;//��������


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        physicsCheck = GetComponent<PhysicsCheck>();//������ײ���

        inputcontrol = new PlayerInputControl();//���ü�������

        spriteRenderer = GetComponent<SpriteRenderer>();

        boxCollider = GetComponent<BoxCollider2D>();
        OriginBox = boxCollider.size;
    }

    //private void OnEnable()
    //{
    //    inputcontrol.Enable();// �������ʾ
    //}
    //private void OnDisable()
    //{
    //    inputcontrol.Disable();//���������
    //}

    void Update()//������ƶ�
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
    public void Move()//�����ƶ���ת
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
    private void Jump()//ʩ����Ծ����
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
