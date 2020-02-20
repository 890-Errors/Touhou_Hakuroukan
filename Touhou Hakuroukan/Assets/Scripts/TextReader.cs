using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextReader : MonoBehaviour
{
    public string pathToLoad;
    // Start is called before the first frame update
    void Start()
    {
        TextAsset txt = Resources.Load(pathToLoad) as TextAsset;
        gameObject.GetComponent<Text>().text = txt.text;
    }
}
