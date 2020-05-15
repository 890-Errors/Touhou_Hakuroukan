using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Player.emitter.transform.parent.GetComponent<CameraFollower>().followingObject = Player.enemy.gameObject;
        new WaitForSeconds(followingTime);
        Player.emitter.transform.parent.GetComponent<CameraFollower>().followingObject = Player.gameObject;
        //StartCoroutine("Release");
    }

    public IEnumerator Release()
    {
        Player.GetComponent<CameraFollower>().followingObject = Player.enemy.gameObject;
        yield return new WaitForSeconds(followingTime);
        Player.GetComponent<CameraFollower>().followingObject = Player.gameObject;
    }
}
