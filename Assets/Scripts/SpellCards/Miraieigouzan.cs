using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Miraieigouzan : MonoBehaviour, ISpellCard
{
    public int ID { get; }
    public int Cost { get; }
    public string SpellCardName { get; }
    public string SpellCardDesc { get; }

    public Player Player;
    public LifeUIController LifeUIController;

    public Miraieigouzan()
    {
        ID = 5;
        Cost = 5;
        SpellCardName = "人鬼「未来永劫斩」";
        SpellCardDesc =
            "向对手冲刺斩击后，再进行连续追击。\n" +
            "威力极高且拥有无敌时间，可谓是攻防一体的完美符卡。";
    }

    //TODO: 可以用特性重写
    public void SpellCardRelease()
    {
        Grazer grazer = Player.grazer;
        if (grazer.grazeLevel < Cost)
        {
            CameraShaker.Instance.ShakeOnce(10f, 4f, .2f, .2f);     //擦弹等级不足
            AudioManager.instance.PlaySingle("Invalid");
        }
        else
        {
            grazer.grazeLevel -= Cost;
            grazer.GrazeLevelUIController.SetGrazeLevel(grazer.grazeLevel);
            LifeUIController.SetLifeLevel(++Player.HP);
        }
    }
}
