using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;

public class Grazer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip seGraze;
    public int grazeNum;

    private DanmakuCollisionList danmakuCollisionsLastFrame;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GetComponent<DanmakuCollider>().OnDanmakuCollision += Graze;
    }

    //擦弹
    void Graze(DanmakuCollisionList danmakuCollisions)
    {
        //danmakuCollisionsLastFrame??
        
        audioSource.PlayOneShot(seGraze);   //擦弹音效

        grazeNum += danmakuCollisions.Count;
        //TODO:擦弹回蓝
        
    }
}
