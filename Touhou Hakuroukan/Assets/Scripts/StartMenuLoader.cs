using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuLoader : MonoBehaviour
{
    private AsyncOperation operation;

    IEnumerator AsyncLoading()
    {
        yield return new WaitForSeconds(3);     //加载太快了，先播上3s的动画
        operation = SceneManager.LoadSceneAsync(2, mode: LoadSceneMode.Single);
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
