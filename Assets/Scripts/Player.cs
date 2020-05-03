using System;
using System.Collections;
using UnityEngine;
using DanmakU;

public class Player : MonoBehaviour,IHealthPoint
{
    public int HP { get=>hp; set=>hp = value; }

    public int hp = 500;
    public float moveSpeedHigh = 12.0f;
    public float moveSpeedLow = 6.0f;
    public float rushSpeed = 50.0f;
    public int rushFrames = 10;
    public float rushCoolTime = 2.0f;
    public float visualField = 30;

    private bool isLowSpeed;
    private bool isRushing;
    private bool isRushCooling;

    public Rigidbody2D rb;
    public CircleCollider2D hitboxCollider;
    public IHitbox hitbox;
    public DanmakuEmitter emitter;
    public GameObject enemy;
    public AudioSource audioSourceShoot;
    public AudioSource audioSourceDead;
    public AudioClip sePlayerDead;
    public AudioClip sePlayerShoot;
    public TrailRenderer trailRenderer;
    public SpriteRenderer spriteRendererYoumu;
    public SpriteRenderer spriteRendererHanrei;
    public Grazer grazer;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lastNonzeroMoveDirection = Vector2.right;
    private SpriteRenderer hitboxRenderer;
    private Vector3 enemyDirection;

    private void Awake()
    {
        //获取必要组件的引用
        rb = GetComponent<Rigidbody2D>();
        emitter = transform.GetChild(0).GetChild(0).GetComponent<DanmakuEmitter>();
        hitbox = transform.GetChild(1).GetComponent<IHitbox>();
        hitboxRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        audioSourceShoot = GetComponents<AudioSource>()[0];
        audioSourceDead = GetComponents<AudioSource>()[1];
        trailRenderer = GetComponent<TrailRenderer>();
        spriteRendererYoumu = GetComponent<SpriteRenderer>();

        //默认不攻击、关闭判定点、无拖尾
        emitter.enabled = false;
        hitboxRenderer.enabled = false;
        trailRenderer.emitting = false;

        //GetComponent<DanmakuCollider>().OnDanmakuCollision += OnDanmakuCollision;

    }

    // Update is called once per frame
    void Update()
    {

        //处理移动相关的操作
        moveDirection.x = (int)Input.GetAxisRaw("Horizontal");
        moveDirection.y = (int)Input.GetAxisRaw("Vertical");
        if (moveDirection != Vector2.zero) lastNonzeroMoveDirection = moveDirection;
        enemyDirection = enemy.transform.position - emitter.gameObject.transform.position;
        emitter.gameObject.transform.right = enemyDirection.magnitude <= visualField ?
            enemyDirection : (Vector3)(lastNonzeroMoveDirection - new Vector2(0.01f, 0.01f));       //使用魔法让forward不会因这句指向奇怪的地方
        //瞄准方向欧拉角Z分量绝对值>90度，则转身
        spriteRendererYoumu.flipX = (emitter.transform.rotation.eulerAngles.z > 90 && emitter.transform.rotation.eulerAngles.z < 270);
        spriteRendererHanrei.flipX = spriteRendererYoumu.flipX;


        if (Input.GetButton("LowSpeed"))
        {
            //切慢速，显示判定点
            isLowSpeed = true;
            hitboxRenderer.enabled = true;
        }
        else
        {
            //取消慢速，不显示判定点
            isLowSpeed = false;
            hitboxRenderer.enabled = false;
        }
        if (Time.timeScale != 1) moveDirection = Vector2.zero;

        //射击,超过一定距离即失去目标
        if (Input.GetButton("Shoot") && Time.timeScale != 0)
        {
            if (Input.GetButtonDown("Shoot"))
            {
                audioSourceShoot.clip = sePlayerShoot;
                audioSourceShoot.loop = true;
                audioSourceShoot.Play();
            }

            emitter.enabled = true;
            //audioSource.PlayOneShot(sePlayerShoot);
        }
        else
        {
            emitter.enabled = false;
            audioSourceShoot.Stop();
        }

        //使用符卡
        //待完成


        //闪现
        if (Input.GetButtonDown("Rush") && !isRushCooling)
        {
            StartCoroutine("Rush");
        }


        //切换符卡
        //时间减慢
        if (Input.GetButtonDown("Change"))
        {
            Time.timeScale = 0.02f;
            Time.fixedDeltaTime *= Time.timeScale;
            Debug.Log("time stop.");
        }
        if (Input.GetButtonUp("Change"))
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    private void FixedUpdate()
    {
        Move(isRushing ? lastNonzeroMoveDirection : moveDirection, isLowSpeed ? moveSpeedLow : moveSpeedHigh);
    }

    IEnumerator Rush()
    {
        hitbox.Collider2D.enabled = false;
        isRushCooling = true;
        isRushing = true;
        (var moveSpeedHighTemp, var moveSpeedLowTemp) = (moveSpeedHigh, moveSpeedLow);
        (moveSpeedHigh, moveSpeedLow) = (rushSpeed, rushSpeed);
        moveDirection = lastNonzeroMoveDirection;
        //这里可以插个Rush特效
        trailRenderer.emitting = true;
        for (int i = 0; i < rushFrames; ++i) yield return new WaitForFixedUpdate();     //保持一定帧数高速移动
        isRushing = false;
        hitbox.Collider2D.enabled = true;
        (moveSpeedHigh, moveSpeedLow) = (moveSpeedHighTemp, moveSpeedLowTemp);
        trailRenderer.emitting = false;
        yield return new WaitForSeconds(rushCoolTime);
        isRushCooling = false;
    }

    void Move(Vector2 direction, float speed)
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime *
            ((direction.x != 0 && direction.y != 0) ? 0.707f : 1.0f));
    }

    //void OnDanmakuCollision(DanmakuCollisionList danmakuCollisions)
    //{
    //    for (int i = 0; i < danmakuCollisions.Count; i++)
    //    {
    //        if (!WhoseDanmaku.IsMyDanmaku(danmakuCollisions[i].Danmaku, emitter))    //先判断是否是自机的弹幕
    //        {
    //            if (HP > 0)
    //            {
    //                HP -= danmakuCollisions.Count;
    //                danmakuCollisions[i].Danmaku.Destroy();
    //            }
    //            if (HP <= 0)
    //            {
    //                transform.RotateAround(gameObject.transform.position + Vector3.down, Vector3.back, 90);
    //                GetComponent<DanmakuCollider>().OnDanmakuCollision -= OnDanmakuCollision;
    //                enabled = false;
    //            }
    //            StartCoroutine("Invincible");   //中弹无敌一秒
    //            break;//自机一帧只处理一个弹幕碰撞
    //        }
    //    }
    //}

    //IEnumerator Invincible()    //无敌一秒
    //{
    //    hitboxCollider.enabled = false;
    //    audioSourceDead.PlayOneShot(sePlayerDead);
    //    yield return new WaitForSeconds(1.0f);
    //    hitboxCollider.enabled = true;
    //}

}
