using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeUIController : MonoBehaviour
{
    public GameObject[] lifeLevels = new GameObject[6];
    public Sprite iconLifeFull;
    public Sprite iconLifeEmpty;

    public void SetLifeLevel(int lifeToSet)
    {
        for (int i = 0; i < lifeLevels.Length; i++)
        {

            lifeLevels[i].GetComponent<Image>().sprite =
                (i < lifeToSet) ? iconLifeFull : iconLifeEmpty;

        }
    }
}
