﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;
using System.Linq;

public class Grazer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip seGraze;
    public int grazeNum;
    public DanmakuEmitter emitter;

    private List<int> danmakuIdCollided;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        emitter = transform.parent.GetComponent<Player>().emitter;
        danmakuIdCollided = new List<int>();

        GetComponent<DanmakuCollider>().OnDanmakuCollision += Graze;    //注册碰撞事件处理（擦弹）
    }

    //擦弹
    void Graze(DanmakuCollisionList danmakuCollisions)
    {
        //取碰撞中不属于自己的子弹之ID
        var danmakuIdColliding = from danmakucollison in danmakuCollisions
                                 where !WhoseDanmaku.IsMyDanmaku(danmakucollison.Danmaku, emitter)
                                 select danmakucollison.Danmaku.Id;
        foreach(var danmakuId in danmakuIdColliding.Except(danmakuIdCollided))
        {
            audioSource.PlayOneShot(seGraze);   //擦弹音效
            grazeNum += danmakuCollisions.Count;
            //TODO: 擦弹回蓝

        }
        danmakuIdCollided = danmakuIdColliding.ToList();      //记录下本帧已擦过的弹
    }
}