﻿using DanmakU;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour, IHitbox
{
    public Collider2D Collider2D { get; set; }
    public DanmakuCollider DanmakuCollider { get; set; }
    public IHealthPoint ParentController { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        ParentController = transform.parent.gameObject.GetComponent<IHealthPoint>();
        DanmakuCollider = GetComponent<DanmakuCollider>();
        Collider2D = GetComponent<Collider2D>();

        DanmakuCollider.OnDanmakuCollision += OnDanmakuCollision;
    }

    public void OnDanmakuCollision(DanmakuCollisionList danmakuCollisions)
    {
        for (int i = 0; i < danmakuCollisions.Count; i++)
        {
            //先判断是否是自机的弹幕
            if (!WhoseDanmaku.IsMyDanmaku(danmakuCollisions[i].Danmaku, (ParentController as Player).emitter))
            {
                if (ParentController.HP > 0)
                {
                    ParentController.HP -= danmakuCollisions.Count;
                }
                if (ParentController.HP <= 0)
                {
                    transform.RotateAround(gameObject.transform.position + Vector3.down, Vector3.back, 90);
                    GetComponent<DanmakuCollider>().OnDanmakuCollision -= OnDanmakuCollision;
                    enabled = false;
                }
                StartCoroutine("Invincible");   //中弹无敌一秒
                break;//自机一帧只处理一个弹幕碰撞
            }
        }
    }
    IEnumerator Invincible()    //无敌一秒
    {
        Collider2D.enabled = false;
        (ParentController as Player).audioSourceDead.PlayOneShot((ParentController as Player).sePlayerDead);
        yield return new WaitForSeconds(1.0f);
        Collider2D.enabled = true;
    }
}