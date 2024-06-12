using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("挂载组件")]
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
    [HideInInspector] public float direction=1;//方向
    private bool isJump;//判断跳跃
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    [HideInInspector] public bool isWalking;//判断走路
    [HideInInspector] public bool isGround;//判断在地面
    [HideInInspector] public bool isJumping = false;//判断跳跃中
    [HideInInspector] public bool isFalling = false;//判断下落中

    [Header("冲刺属性")]
    public float prepareRushTime;//冲刺蓄力时间
    private float nowTime;//目前蓄力时间
    private float blinkTime=0.05f;//闪烁速度
    private bool isBlinking;
    [HideInInspector] public bool isPrepareRush;//是否准备冲刺
    [HideInInspector] public bool canRush=true;//是否能冲刺
    private float rushTime = 0.2f;//冲刺时间
    private float rushCoolTime = 1f;//冲刺冷却时间
    [HideInInspector] public bool isRushing=false;//是否正在冲刺
    public float rushSpeed=24f;//冲刺速度
    [HideInInspector] public bool canDestory;

    [Header("小冲刺属性")]
    public float rushTimeMin = 0.05f;//小冲刺时间
    public float rushCoolTimeMin = 0.5f;//小冲刺冷却时间
    public float rushSpeedMin = 24f;//冲刺速度

    [Header("下砸属性")]
    public float smashDownWaitTime;//蓄力时间
    public float smashDownA;//下砸加速度
    public float smashDownSpeed;//下砸瞬间速度
    public float smashDownForce;//下砸后坐力
    public float smashDownRecoveryTime;//下砸硬直时间
    [HideInInspector] public bool canSmashDown;//能否下砸
    [HideInInspector] public bool isPrepareSmashDown;//是否准备下砸
    [HideInInspector] public bool isSmashingDown;//是否正在下砸

    [Header("移动属性")]
    public float speed;//速度
    public float jumpSpeed=30;//跳跃速度
    public float fallMultiplier = 2.5f;//下降修正
    public float lowMultiplier = 2f;//上升修正
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
    //移动逻辑
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
    //跳跃逻辑
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

    
    //死亡逻辑
    public void Die()
    {
        transform.position = restartPosition.position;
    }

    //下砸逻辑，用协程进行
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
        //使用物理模拟时，AddForce会在Fixedupdate结束后调用，但此时Fixedupdate返回
        yield return new WaitForFixedUpdate();
        rb.gravityScale = origionalGravity;
        GameObject se = GameObject.Instantiate(SmashDownEffect, SmashDownEffctPosition.position, spawnRotation);
        Destroy(se,1);
        rb.AddForce(Vector2.up * smashDownForce,ForceMode2D.Impulse);
        yield return new WaitForSeconds(smashDownRecoveryTime);
        isSmashingDown = false;
    }

    //计时器
    private void Timer()
    {
        nowTime += Time.deltaTime;
    }
    //冲刺蓄力检测
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

    //冲刺逻辑，用协程进行
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
    //短冲
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
