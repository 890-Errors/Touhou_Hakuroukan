using UnityEngine;
using UnityEngine.EventSystems;

public class MouseCracker : MonoBehaviour
{
    GameObject lastselect;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        lastselect = new GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastselect);
        }
        else
        {
            lastselect = EventSystem.current.currentSelectedGameObject;
        }
    }
}
