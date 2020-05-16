using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Ibukihyou : MonoBehaviour, ISpellCard
{
    public int ID { get; }
    public int Cost { get; }
    public string SpellCardName { get; }
    public string SpellCardDesc { get; }

    public Player Player;
    public LifeUIController LifeUIController;

    public Ibukihyou()
    {
        ID = 3;
        Cost = 3;
        SpellCardName = "「伊吹瓢」";
        SpellCardDesc =
            "能涌出无限量酒的鬼之葫芦。\n" +
            "使用后，回复一点残机。";
    }

    //TODO: 可以用特性重写
    public void SpellCardRelease()
    {
        Grazer grazer = Player.grazer;
        if (grazer.grazeLevel >= Cost)
        {
            grazer.grazeLevel -= Cost;
            grazer.GrazeLevelUIController.SetGrazeLevel(grazer.grazeLevel);
            LifeUIController.SetLifeLevel(++Player.HP);
        }
        else
        {
            CameraShaker.Instance.ShakeOnce(10f, 4f, .2f, .2f);     //擦弹等级不足
        }
    }
}
