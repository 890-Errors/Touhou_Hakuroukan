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
    public int HP;
    public float moveSpeedHigh = 12.0f;
    public float moveSpeedLow = 6.0f;
    public float rushSpeed = 50.0f;
    public int rushFrames = 10;
    public float rushCoolTime = 2.0f;
    public float visualField = 30;
    
    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lastNonzeroMoveDirection = Vector2.right;
    private SpriteRenderer hitbox;

    private bool isLowSpeed;
    private bool isRushCooling;

    private void Awake()
    {
        //获取必要组件的引用
        rb = gameObject.GetComponent<Rigidbody2D>();
        bc = gameObject.GetComponent<Collider2D>();
        emitter = gameObject.transform.GetChild(0).GetChild(0).GetComponent<DanmakU.DanmakuEmitter>();
        hitbox = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();

        //默认不攻击、关闭判定点
        emitter.enabled = false;
        hitbox.enabled = false;
            
    }

    // Update is called once per frame
    void Update()
    {

        //移动
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
            Vector3 enemyDirection = enemy.transform.position - emitter.gameObject.transform.position;
            emitter.gameObject.transform.right = enemyDirection.magnitude <= visualField ? 
                enemyDirection : (Vector3)(lastNonzeroMoveDirection - new Vector2(0.01f, 0.01f));       //使用魔法让forward不会因这句指向奇怪的地方
            emitter.enabled = true;
        }
        else
        {
            emitter.enabled = false;
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
        
        if (moveDirection.x != 0 && moveDirection.y != 0)       //对角线移动，削减速度
        {
            if (isLowSpeed)
                rb.MovePosition(rb.position + moveDirection * moveSpeedLow * Time.fixedDeltaTime * 0.707f);
            else rb.MovePosition(rb.position + moveDirection * moveSpeedHigh * Time.fixedDeltaTime * 0.707f);
        }
        else
        {
            if (isLowSpeed)
                rb.MovePosition(rb.position + moveDirection * moveSpeedLow * Time.deltaTime);
            else rb.MovePosition(rb.position + moveDirection * moveSpeedHigh * Time.deltaTime);
        }
    }

    IEnumerator Rush()
    {
        bc.enabled = false;
        isRushCooling = true;
        (var moveSpeedHighTemp, var moveSpeedLowTemp) = (moveSpeedHigh, moveSpeedLow);
        (moveSpeedHigh, moveSpeedLow) = (rushSpeed, rushSpeed);
        moveDirection = lastNonzeroMoveDirection;
        //这里可以插个Rush特效
        for (int i = 0; i < rushFrames; ++i) yield return new WaitForFixedUpdate();     //保持一定帧数高速移动
        bc.enabled = true;
        (moveSpeedHigh, moveSpeedLow) = (moveSpeedHighTemp, moveSpeedLowTemp);
        yield return new WaitForSeconds(rushCoolTime);
        isRushCooling = false;
    }

}
