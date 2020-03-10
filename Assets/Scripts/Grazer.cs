using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;

public class Grazer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip seGraze;
    public int grazeNum;
    public DanmakuEmitter emitter;

    private DanmakuCollisionList danmakuCollisionsLastFrame;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        emitter = transform.parent.GetComponent<Player>().emitter;

        GetComponent<DanmakuCollider>().OnDanmakuCollision += Graze;    //注册碰撞事件处理（擦弹）
    }

    //擦弹
    void Graze(DanmakuCollisionList danmakuCollisions)
    {
        for (int i = 0; i < danmakuCollisions.Count; i++)
        {
            //TODO: 较好的检测自己子弹办法？
            if (!WhoseDanmaku.IsMyDanmaku(danmakuCollisions[i].Danmaku, emitter) 
                && !danmakuCollisions[i].Danmaku.Grazed)            //用弹速检测是否是自己的子弹
            {
                var thisDanmaku = danmakuCollisions[i].Danmaku;     //danmakuCollisons是readonly的
                thisDanmaku.Grazed = true;

                audioSource.PlayOneShot(seGraze);   //擦弹音效

                grazeNum += danmakuCollisions.Count;
                //TODO: 擦弹回蓝
            }
        }
    }
}
