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
    private float direction=1;//方向
    private bool isJump;//判断跳跃
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public bool isGround;//判断在地面
    public bool isJumping = false;//判断跳跃中
    public bool isFalling = false;//判断下落中
    public LayerMask groundLayer;
    public LayerMask boxLayer;

    public float prepareRushTime;//冲刺蓄力时间
    private float nowTime;//目前蓄力时间
    public bool isPrepareRush;//是否准备下砸
    private bool canRush=true;//是否能冲刺
    private float rushTime = 0.2f;//冲刺时间
    private float rushCoolTime = 1f;//冲刺冷却时间
    public bool isRushing=false;//是否正在冲刺
    public float rushSpeed=24f;//冲刺速度

    private bool canSmashDown;//能否下砸
    public bool isPrepareSmashDown;//是否准备下砸
    public bool isSmashingDown;//是否正在下砸
    public float smashDownWaitTime;//蓄力时间
    public float smashDownA;//下砸加速度
    public float smashDownSpeed;//下砸瞬间速度
    public float smashDownForce;//下砸后坐力
    public float smashDownRecoveryTime;//下砸硬直时间

    public float speed;//速度
    public float jumpSpeed=30;//跳跃速度
    public float fallMultiplier = 2.5f;//下降修正
    public float lowMultiplier = 2f;//上升修正

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
        //蓄力速度清零
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
        //蓄力速度清零
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
    //移动逻辑
    private void  Transform()
    {
        movement = Input.GetAxisRaw("Horizontal");
        transform.Translate(new Vector2(movement, 0) * speed * Time.deltaTime);
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
        if (Input.GetKey(KeyCode.K))
        {
            rb.velocity = Vector2.zero;
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
 
    //冲刺逻辑，用协程进行
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
