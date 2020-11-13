using TMPro;
using UnityEngine;

public class PixelSnapping : MonoBehaviour
{
    public TextMeshProUGUI textPixelSnapping;
    private bool isPixelSnapping = false;   

    // Start is called before the first frame update
    void Awake()
    {
        textPixelSnapping = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textPixelSnapping.text = "PixelSnapping  " + (isPixelSnapping ? "ON" : "OFF");

    }

    public void ChangePixelSnapping()
    {
        isPixelSnapping = !isPixelSnapping;
        textPixelSnapping.text = "PixelSnapping  " + (isPixelSnapping ? "ON" : "OFF");
        GameManager.instance.gameOption.isPixelSnapping = isPixelSnapping;
    }
}
