using DanmakU;
using UnityEngine;
using UnityEngine.UI;

public class Seija : MonoBehaviour, IHealthPoint
{
    public int HPmax = 200;
    public int HP { get; set; }
    public AudioSource audioSource;
    public DanmakuEmitter emitter;
    public Image HealthBar;
    public AudioClip damageHighHP;
    public AudioClip damageLowHP;
    public AudioClip enemyDead;
    public Player Player;

    private float HealthRate { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        HP = HPmax;
        GetComponent<DanmakuCollider>().OnDanmakuCollision += OnDanmakuCollision;
        audioSource = GetComponent<AudioSource>();
        //danmakuEmitter.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        HealthRate = HP / (float)HPmax;
        HealthBar.fillAmount = HealthRate;

        if (HP <= 0) Die();

        //以下为运行时测试使用
        if (Input.GetKeyDown(KeyCode.B))
        {
            emitter.FireRate *= 50;
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            emitter.FireRate /= 50;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            HP = HPmax;
            Player.HP = 6;
        }
    }

    void OnDanmakuCollision(DanmakuCollisionList danmakuCollisions)
    {
        for (int i = 0; i < danmakuCollisions.Count; i++)
        {
            if (WhoseDanmaku.IsMyDanmaku(danmakuCollisions[i].Danmaku, Player.emitter))
            {
                if (HP >= 0)
                {
                    HP -= 1;
                    danmakuCollisions[i].Danmaku.Destroy();
                    audioSource.PlayOneShot(HP >= HPmax * .2f ? damageHighHP : damageLowHP);
                }
                else
                {
                    Die();
                }
            }
        }
    }

    void Die()
    {
        transform.RotateAround(gameObject.transform.position + Vector3.down, Vector3.back, 90);
        GetComponent<DanmakuCollider>().OnDanmakuCollision -= OnDanmakuCollision;
        transform.GetChild(0).GetComponent<DanmakuEmitter>().enabled = false;
        audioSource.PlayOneShot(enemyDead);
        FindObjectOfType<Player>().visualField = 0f;    //别打了，爷躺了
        GetComponent<SeijaAI>().enabled = false;
        this.enabled = false;
    }
}
