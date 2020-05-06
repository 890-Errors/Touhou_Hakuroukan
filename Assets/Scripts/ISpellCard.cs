using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ISpellCard
{
    int ID { get; }
    int Cost { get; }
    string SpellCardName { get; }
    string SpellCardDesc { get; }

    void SpellCardRelease();
}
