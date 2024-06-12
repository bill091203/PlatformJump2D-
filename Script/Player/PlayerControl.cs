using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("�������")]
    public GameObject cBox;
    public GameObject SmashDownEffect;
    public GameObject rushEffectL;
    public GameObject rushEffectR;
    public Transform SmashDownEffctPosition;
    public TrailRenderer tr;
    public Transform restartPosition;
    public LayerMask groundLayer;
    public LayerMask boxLayer;
    private Quaternion spawnRotation;
    private Vector2 _posistion = Vector2.zero;

    private float movement;
    [HideInInspector] public float direction=1;//����
    private bool isJump;//�ж���Ծ
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    [HideInInspector] public bool isWalking;//�ж���·
    [HideInInspector] public bool isGround;//�ж��ڵ���
    [HideInInspector] public bool isJumping = false;//�ж���Ծ��
    [HideInInspector] public bool isFalling = false;//�ж�������

    [Header("�������")]
    public float prepareRushTime;//�������ʱ��
    private float nowTime;//Ŀǰ����ʱ��
    private float blinkTime=0.05f;//��˸�ٶ�
    private bool isBlinking;
    [HideInInspector] public bool isPrepareRush;//�Ƿ�׼�����
    [HideInInspector] public bool canRush=true;//�Ƿ��ܳ��
    private float rushTime = 0.2f;//���ʱ��
    private float rushCoolTime = 1f;//�����ȴʱ��
    [HideInInspector] public bool isRushing=false;//�Ƿ����ڳ��
    public float rushSpeed=24f;//����ٶ�
    [HideInInspector] public bool canDestory;

    [Header("С�������")]
    public float rushTimeMin = 0.05f;//С���ʱ��
    public float rushCoolTimeMin = 0.5f;//С�����ȴʱ��
    public float rushSpeedMin = 24f;//����ٶ�

    [Header("��������")]
    public float smashDownWaitTime;//����ʱ��
    public float smashDownA;//���Ҽ��ٶ�
    public float smashDownSpeed;//����˲���ٶ�
    public float smashDownForce;//���Һ�����
    public float smashDownRecoveryTime;//����Ӳֱʱ��
    [HideInInspector] public bool canSmashDown;//�ܷ�����
    [HideInInspector] public bool isPrepareSmashDown;//�Ƿ�׼������
    [HideInInspector] public bool isSmashingDown;//�Ƿ���������

    [Header("�ƶ�����")]
    public float speed;//�ٶ�
    public float jumpSpeed=30;//��Ծ�ٶ�
    public float fallMultiplier = 2.5f;//�½�����
    public float lowMultiplier = 2f;//��������
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr= GetComponent<SpriteRenderer>();
        spawnRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    }

    void FixedUpdate()
    {
        if (isRushing || isSmashingDown || isPrepareRush || isPrepareSmashDown)
        {
            return;
        }
        Jump();
        Transform();
    }

    private void Update()
    {
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
        if (isRushing || isSmashingDown || isPrepareRush || isPrepareSmashDown )
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.K) && canRush)
        {
            isPrepareRush = true;
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
        if (movement< 0)
        {
            transform.Translate(Vector2.left * speed*Time.deltaTime);
        }
        if (movement > 0)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        isWalking = movement != 0;
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
        canDestory = true;
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
        if (!canRush||isSmashingDown||isPrepareSmashDown)
        {
            return;
        }
        movement = Input.GetAxisRaw("Horizontal");
        if (movement != 0)
        {
            direction = movement;
        }
        if (Input.GetKey(KeyCode.K))
        {
            rb.velocity = Vector2.zero;
            isPrepareRush = true;
            rb.gravityScale = 0;
            Timer();
            if (nowTime >= prepareRushTime&&!isBlinking)
            {
                StartCoroutine(Blink());
            }
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            isPrepareRush = false;
            StopCoroutine(Blink());
            rb.gravityScale = 1;
            if (nowTime >= prepareRushTime)
            {
                nowTime = 0;
                StartCoroutine(Rush());
            }
            else
            {
                nowTime = 0;
                StartCoroutine(RushMin());
            }
        }
    }

    //����߼�����Э�̽���
    #region[rush]
    private IEnumerator Rush()
    {
        canRush = false;
        isRushing = true;
        canDestory = true;
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
    //�̳�
    private IEnumerator RushMin()
    {
        canRush = false;
        isRushing = true;
        isPrepareRush = false;
        canDestory = false;
        float origionalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        if (direction > 0)
        {
            rushEffectR.SetActive(true);
            rb.velocity = Vector2.right * rushSpeedMin;
        }
        if (direction < 0)
        {
            rushEffectL.SetActive(true);
            rb.velocity = Vector2.left * rushSpeedMin;
        }
        yield return new WaitForSeconds(rushTimeMin);
        rb.gravityScale = 1;
        isRushing = false;
        rushEffectL.SetActive(false);
        rushEffectR.SetActive(false);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(rushCoolTimeMin);
        canRush = true;
    }

    private IEnumerator Blink() 
    {
        isBlinking= true;
        while (isPrepareRush)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkTime);
        }
        isBlinking = false;
        sr.enabled = true;
    }
}
