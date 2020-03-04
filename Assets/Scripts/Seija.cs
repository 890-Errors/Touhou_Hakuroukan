using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;

public class Seija : MonoBehaviour
{
    public int HPmax = 200;
    public int HP;
    public AudioSource audioSource;
    public DanmakU.DanmakuEmitter danmakuEmitter;

    public AudioClip damageHighHP;
    public AudioClip damageLowHP;
    public AudioClip enemyDead;

    // Start is called before the first frame update
    void Awake()
    {
        HP = HPmax;
        GetComponent<DanmakuCollider>().OnDanmakuCollision += OnDanmakuCollision;
        audioSource = GetComponent<AudioSource>();
        danmakuEmitter.enabled = true;
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    void OnDanmakuCollision(DanmakuCollisionList danmakuCollisions)
    {
        if (HP >= 0)
        {
            HP -= danmakuCollisions.Count;
            foreach(DanmakuCollision danmakuCollision in danmakuCollisions)
            {
                danmakuCollision.Danmaku.Destroy();
            }
            audioSource.PlayOneShot(HP >= HPmax * .2f ? damageHighHP : damageLowHP);
        }
        if (HP <= 0)
        {
            transform.RotateAround(gameObject.transform.position + Vector3.down, Vector3.back, 90);
            GetComponent<DanmakuCollider>().OnDanmakuCollision -= OnDanmakuCollision;
            transform.GetChild(0).GetComponent<DanmakU.DanmakuEmitter>().enabled = false;
            audioSource.PlayOneShot(enemyDead);
            FindObjectOfType<Player>().visualField = 0f;    //别打了，爷躺了
        }
    }
}
