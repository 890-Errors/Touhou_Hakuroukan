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
    public Grazer Grazer;

    public Ibukihyou()
    {
        ID = 3;
        Cost = 3;
        SpellCardName = "「伊吹瓢」";
        SpellCardDesc =
            "能涌出无限量酒的鬼之葫芦。\n" +
            "使用后，回复一点残机。";
    }

    public void SpellCardRelease()
    {
        if (Grazer.grazeLevel >= Cost)
        {
            Grazer.grazeLevel -= Cost;
            Grazer.GrazeLevelUIController.SetGrazeLevel(Grazer.grazeLevel);
            LifeUIController.SetLifeLevel(++Player.HP);
        }
        else
        {
            CameraShaker.Instance.ShakeOnce(10f, 4f, .2f, .2f);     //擦弹等级不足
        }
    }
}
