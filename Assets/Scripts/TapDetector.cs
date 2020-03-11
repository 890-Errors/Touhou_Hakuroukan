using UnityEngine;
using UnityEngine.SceneManagement;

public class TapDector : MonoBehaviour
{
    public string sceneToLoad;
    public bool isTouched = false;

    private bool isLoading = false;

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR||UNITY_STANDALONE
        isTouched = Input.GetKeyDown(KeyCode.Mouse0);
#elif UNITY_ANDROID
        isTouched = Input.touchCount > 0 ? true : false;
#endif
        if (!isLoading && isTouched)
        {
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
            isLoading = true;
        }
    }
}
