using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrazeLevelUIController : MonoBehaviour
{
    Text grazeLevelTextContainer;

    private void Awake()
    {
        grazeLevelTextContainer = GetComponent<Text>();
    }
    // Start is called before the first frame update
    string[] grazeLevelsTexts = new string[]
    {
        "",
        "<color=#AB3B3A>心</color>",
        "<color=#AB3B3A>心</color><color=#FFB11B>形</color>",
        "<color=#AB3B3A>心</color><color=#FFB11B>形</color><color=#268785>意</color>",
        "<color=#AB3B3A>心</color><color=#FFB11B>形</color><color=#268785>意</color><color=#005CAF>魂</color>",
        "<color=#AB3B3A>心</color><color=#FFB11B>形</color><color=#268785>意</color><color=#005CAF>魂</color><color=#66327C>神</color>",
    };

    public void SetGrazeLevel(int levelToSet) => grazeLevelTextContainer.text = grazeLevelsTexts[levelToSet] ?? grazeLevelTextContainer.text;
}
