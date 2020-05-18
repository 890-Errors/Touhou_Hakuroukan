using UnityEngine;
using EZCameraShake;

public class Yorimasinobaku : MonoBehaviour, ISpellCard
{
    public int ID { get; }
    public int Cost { get; }
    public string SpellCardName { get; }
    public string SpellCardDesc { get; }

    public float followingTime;

    public Player Player;

    public Yorimasinobaku()
    {
        ID = 2;
        Cost = 2;
        SpellCardName = "凭依之缚";
        SpellCardDesc =
            "命令半身飞向敌人的攻击。\n" +
            "命中后半身将会暂时跟随对手。";
        followingTime = 4.0f;
    }

    public void SpellCardRelease()
    {
        var follower = Player.emitter.transform.parent.GetComponent<CameraFollower>();
        var grazer = Player.grazer;

        if (follower.followingObject != Player.gameObject || grazer.grazeLevel < Cost)     //半灵已经放出去了，或者擦弹等级不足
        {
            CameraShaker.Instance.ShakeOnce(10f, 4f, .2f, .2f);
            AudioManager.instance.PlaySingle("Invalid");
        }
        else
        {
            grazer.grazeLevel -= Cost;
            AudioManager.instance.PlaySingle(AudioManager.instance.seOK);
            Player.emitter.transform.parent
            .GetComponent<CameraFollower>().FollowSomething(Player.enemy, followingTime);
        }
    }
}
