using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public GameObject cBox;
    public GameObject SmashDownEffect;
    public GameObject rushEffectL;
    public GameObject rushEffectR;
    public Transform SmashDownEffctPosition;
    private Quaternion spawnRotation;

    private float movement;
    private float direction=1;//����
    private bool isJump;//�ж���Ծ
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public bool isGround;//�ж��ڵ���
    public bool isJumping = false;//�ж���Ծ��
    public bool isFalling = false;//�ж�������
    public LayerMask groundLayer;
    public LayerMask boxLayer;

    public float prepareRushTime;//�������ʱ��
    private float nowTime;//Ŀǰ����ʱ��
    public bool isPrepareRush;//�Ƿ�׼������
    private bool canRush=true;//�Ƿ��ܳ��
    private float rushTime = 0.2f;//���ʱ��
    private float rushCoolTime = 1f;//�����ȴʱ��
    public bool isRushing=false;//�Ƿ����ڳ��
    public float rushSpeed=24f;//����ٶ�

    private bool canSmashDown;//�ܷ�����
    public bool isPrepareSmashDown;//�Ƿ�׼������
    public bool isSmashingDown;//�Ƿ���������
    public float smashDownWaitTime;//����ʱ��
    public float smashDownA;//���Ҽ��ٶ�
    public float smashDownSpeed;//����˲���ٶ�
    public float smashDownForce;//���Һ�����
    public float smashDownRecoveryTime;//����Ӳֱʱ��

    public float speed;//�ٶ�
    public float jumpSpeed=30;//��Ծ�ٶ�
    public float fallMultiplier = 2.5f;//�½�����
    public float lowMultiplier = 2f;//��������

    public TrailRenderer tr;
    public Transform restartPosition;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr= GetComponent<SpriteRenderer>();
        spawnRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    }

    void FixedUpdate()
    {
        //�����ٶ�����
        if (Input.GetKey(KeyCode.K))
        {
            rb.velocity = Vector2.zero;
            isPrepareRush = true;
        }
        if (isRushing || isSmashingDown || isPrepareRush||isPrepareSmashDown||Input.GetKey(KeyCode.K))
        {
            return;
        }
        Jump();
        Transform();
    }

    private void Update()
    {
        //�����ٶ�����
        if (Input.GetKey(KeyCode.K))
        {
            rb.velocity = Vector2.zero;
            isPrepareRush = true;
        }

        isGround = Physics2D.OverlapCircle(cBox.transform.position, 0.3f, groundLayer)||
        Physics2D.OverlapCircle(cBox.transform.position, 0.3f, boxLayer);
        if (isGround)
        {
            isJumping = false;
            isFalling = false;
            canSmashDown = false;
        }
        sr.flipX = direction > 0;
        RushCheck();
        if (isRushing || isSmashingDown || isPrepareRush || isPrepareSmashDown || Input.GetKey(KeyCode.K))
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            isJump = true;
        }
        if (canSmashDown && Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine (SmashDown());
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(cBox.transform.position, 0.3f);
    }
    //�ƶ��߼�
    private void  Transform()
    {
        movement = Input.GetAxisRaw("Horizontal");
        transform.Translate(new Vector2(movement, 0) * speed * Time.deltaTime);
        if (movement != 0)
        {
            direction = movement;
        }
    }
    //��Ծ�߼�
    private void Jump()
    {
        if (isJump)
        {
            rb.velocity = Vector2.up * jumpSpeed;
            isGround = false;
            isJump = false;
            isJumping = true;
            canSmashDown = true;
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            isFalling = true;
            isJumping = false;
            canSmashDown = true;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowMultiplier - 1) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.K))
        {
            rb.velocity = Vector2.zero;
        }
    }

    
    //�����߼�
    public void Die()
    {
        transform.position = restartPosition.position;
    }

    //�����߼�����Э�̽���
    private IEnumerator SmashDown()
    {
        canSmashDown = false;
        isPrepareSmashDown = true;
        rb.velocity = Vector2.zero;
        float origionalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        yield return new WaitForSeconds(smashDownWaitTime);
        isPrepareSmashDown = false;
        isSmashingDown = true;
        rb.velocity = Vector2.up*(-smashDownSpeed);
        rb.gravityScale = smashDownA;
        while (!isGround)
        {
            yield return null;
        }
        //ʹ������ģ��ʱ��AddForce����Fixedupdate��������ã�����ʱFixedupdate����
        yield return new WaitForFixedUpdate();
        rb.gravityScale = origionalGravity;
        GameObject se = GameObject.Instantiate(SmashDownEffect, SmashDownEffctPosition.position, spawnRotation);
        Destroy(se,1);
        rb.AddForce(Vector2.up * smashDownForce,ForceMode2D.Impulse);
        yield return new WaitForSeconds(smashDownRecoveryTime);
        isSmashingDown = false;
    }

    //��ʱ��
    private void Timer()
    {
        nowTime += Time.deltaTime;
    }
    //����������
    private void RushCheck()
    {
        movement = Input.GetAxisRaw("Horizontal");
        if (movement != 0)
        {
            direction = movement;
        }
        if (Input.GetKey(KeyCode.K) && canRush)
        {
            isPrepareRush = true;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            Timer();
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            isPrepareRush = false;
            rb.gravityScale = 1;
            if (nowTime >= prepareRushTime)
            {
                nowTime = 0;
                StartCoroutine(Rush());
            }
        }
    }
 
    //����߼�����Э�̽���
    #region[rush]
    private IEnumerator Rush()
    {
        canRush = false;
        isRushing = true;
        float origionalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        if (direction > 0) 
        {
            rushEffectR.SetActive(true);
            rb.velocity = Vector2.right * rushSpeed;
        }
        if (direction < 0)
        {
            rushEffectL.SetActive(true);
            rb.velocity = Vector2.left * rushSpeed;
        }
        yield return new WaitForSeconds(rushTime);
        rb.gravityScale = 1;
        isRushing = false;
        rushEffectL.SetActive(false);
        rushEffectR.SetActive(false);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(rushCoolTime);
        canRush=true;
    }
    #endregion
    
}
