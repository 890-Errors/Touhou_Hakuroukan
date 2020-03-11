using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuLoader : MonoBehaviour
{
    private AsyncOperation operation;

    IEnumerator AsyncLoading()
    {
        yield return new WaitForSeconds(3.0f);     //加载太快了，先播上3s的动画
        operation = SceneManager.LoadSceneAsync("StartMenuScene", mode: LoadSceneMode.Single);         //加载1号scene
        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(AsyncLoading());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
