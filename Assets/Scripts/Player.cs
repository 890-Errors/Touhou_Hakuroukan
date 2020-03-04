using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D bc;
    public DanmakU.DanmakuEmitter emitter;
    public GameObject enemy;
    public AudioSource audioSourceShoot;
    public AudioSource audioSourceDead;
    public int HP;
    public float moveSpeedHigh = 12.0f;
    public float moveSpeedLow = 6.0f;
    public float rushSpeed = 50.0f;
    public int rushFrames = 10;
    public float rushCoolTime = 2.0f;
    public float visualField = 30;
    public AudioClip sePlayerDead;
    public AudioClip sePlayerShoot;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lastNonzeroMoveDirection = Vector2.right;
    private SpriteRenderer hitbox;

    private bool isLowSpeed;
    private bool isRushing;
    private bool isRushCooling;

    private void Awake()
    {
        //获取必要组件的引用
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<Collider2D>();
        emitter = transform.GetChild(0).GetChild(0).GetComponent<DanmakU.DanmakuEmitter>();
        hitbox = transform.GetChild(1).GetComponent<SpriteRenderer>();
        audioSourceShoot = GetComponents<AudioSource>()[0];
        audioSourceDead = GetComponents<AudioSource>()[1];

        //默认不攻击、关闭判定点
        emitter.enabled = false;
        hitbox.enabled = false;

        GetComponent<DanmakU.DanmakuCollider>().OnDanmakuCollision += OnDanmakuCollision;

    }

    // Update is called once per frame
    void Update()
    {

        //处理移动相关的操作
        moveDirection.x = (int)Input.GetAxisRaw("Horizontal");
        moveDirection.y = (int)Input.GetAxisRaw("Vertical");
        if (moveDirection != Vector2.zero) lastNonzeroMoveDirection = moveDirection;

        if (Input.GetButton("LowSpeed"))
        {
            //切慢速，显示判定点
            isLowSpeed = true;
            hitbox.enabled = true;
        }
        else
        {
            //取消慢速，不显示判定点
            isLowSpeed = false;
            hitbox.enabled = false;
        }
        if (Time.timeScale != 1) moveDirection = Vector2.zero;

        //射击,超过一定距离即失去目标
        if (Input.GetButton("Shoot"))
        {
            if (Input.GetButtonDown("Shoot"))
            {
                audioSourceShoot.clip = sePlayerShoot;
                audioSourceShoot.loop = true;
                audioSourceShoot.Play();
            }
            Vector3 enemyDirection = enemy.transform.position - emitter.gameObject.transform.position;
            emitter.gameObject.transform.right = enemyDirection.magnitude <= visualField ?
                enemyDirection : (Vector3)(lastNonzeroMoveDirection - new Vector2(0.01f, 0.01f));       //使用魔法让forward不会因这句指向奇怪的地方
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
        bc.enabled = false;
        isRushCooling = true;
        isRushing = true;
        (var moveSpeedHighTemp, var moveSpeedLowTemp) = (moveSpeedHigh, moveSpeedLow);
        (moveSpeedHigh, moveSpeedLow) = (rushSpeed, rushSpeed);
        moveDirection = lastNonzeroMoveDirection;
        //这里可以插个Rush特效
        for (int i = 0; i < rushFrames; ++i) yield return new WaitForFixedUpdate();     //保持一定帧数高速移动
        isRushing = false;
        bc.enabled = true;
        (moveSpeedHigh, moveSpeedLow) = (moveSpeedHighTemp, moveSpeedLowTemp);
        yield return new WaitForSeconds(rushCoolTime);
        isRushCooling = false;
    }

    void Move(Vector2 direction, float speed)
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime *
            ((direction.x != 0 && direction.y != 0) ? 0.707f : 1.0f));
    }

    void OnDanmakuCollision(DanmakU.DanmakuCollisionList danmakuCollisions)
    {
        if (HP > 0)
        {
            HP -= danmakuCollisions.Count;
            foreach (DanmakU.DanmakuCollision danmakuCollision in danmakuCollisions)
            {
                danmakuCollision.Danmaku.Destroy();
            }
            StartCoroutine("Invincible");   //中弹无敌一秒
        }
        if (HP <= 0)
        {
            transform.RotateAround(gameObject.transform.position + Vector3.down, Vector3.back, 90);
            GetComponent<DanmakU.DanmakuCollider>().OnDanmakuCollision -= OnDanmakuCollision;
            enabled = false;
        }
    }

    IEnumerator Invincible()    //无敌一秒
    {
        bc.enabled = false;
        audioSourceDead.PlayOneShot(sePlayerDead);
        yield return new WaitForSeconds(1.0f);
        bc.enabled = true;
    }

}
