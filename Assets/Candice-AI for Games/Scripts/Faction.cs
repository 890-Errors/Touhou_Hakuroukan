using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Faction", menuName ="Faction")]
public class Faction : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite artwork;
    public Color color;
}
