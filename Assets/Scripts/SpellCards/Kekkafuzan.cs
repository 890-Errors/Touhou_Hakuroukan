using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Kekkafuzan : MonoBehaviour, ISpellCard
{
    public int ID { get; }
    public int Cost { get; }
    public string SpellCardName { get; }
    public string SpellCardDesc { get; }

    public Player Player;
    public LifeUIController LifeUIController;

    public Kekkafuzan()
    {
        ID = 1;
        Cost = 1;
        SpellCardName = "结跏趺斩";
        SpellCardDesc =
            "同时斩下楼观剑和白楼剑，\n" +
            "放出绿色的剑风阻挡弹幕。";
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
