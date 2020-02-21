using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public DanmakU.DanmakuEmitter emitter;
    public float moveSpeedHigh = 8.0f;
    public float moveSpeedLow = 4.0f;
    public int HP;


    private Vector2 moveDirection;
    private SpriteRenderer hitbox;
    private bool isLowSpeed;

    private void Awake()
    {
        //获取必要组件的引用
        rb = gameObject.GetComponent<Rigidbody2D>();
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
        moveDirection = Vector2.zero;
        moveDirection.x = (int)Input.GetAxisRaw("Horizontal");
        moveDirection.y = (int)Input.GetAxisRaw("Vertical");
        if (Input.GetButton("LowSpeed"))
        {
            //切慢速，显示判定点
            isLowSpeed = true;
            hitbox.enabled = true;
        }
        else { 
            //取消慢速，不显示判定点
            isLowSpeed = false;
            hitbox.enabled = false;
        }
        if (Time.timeScale != 1) moveDirection = Vector2.zero;
        //Debug.Log($"{moveDirection.x}, {moveDirection.y}");
        //Debug.Log(Time.fixedDeltaTime);

        //射击
        if (Input.GetButton("Shoot"))
        {
            emitter.enabled = true;
        }
        else
        {
            emitter.enabled = false;
        }

        //使用符卡
        //待完成


        //闪现
        if (Input.GetButtonDown("Rush"))
        {
            Rush();
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
                rb.MovePosition(rb.position + moveDirection * moveSpeedLow * Time.deltaTime * 0.707f);
            else rb.MovePosition(rb.position + moveDirection * moveSpeedHigh * Time.deltaTime * 0.707f);
        }
        else
        {
            if (isLowSpeed)
                rb.MovePosition(rb.position + moveDirection * moveSpeedLow * Time.deltaTime);
            else rb.MovePosition(rb.position + moveDirection * moveSpeedHigh * Time.deltaTime);
        }
    }

    private void Rush()
    {
        
    }
}
