using UnityEngine;
using DanmakU;

public interface ISpellCard
{
    int Cost { get; set; }
    int ID { get; set; }
    string Name { get; set; }
    string Desc { get; set; }
    void SpellCardRelease();
}

public interface ISpellCardPlayer : ISpellCard
{
    Player Player { get; set; }
}

public interface ISpellCardEnemy : ISpellCard
{
    Player Player { get; set; }
}