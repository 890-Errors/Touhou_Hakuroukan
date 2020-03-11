using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public class GameOption
    {
        public bool isPixelSnapping;
        [Range(0, 100)] public int volumeBGM;
        [Range(0, 100)] public int volumeSE;

        public GameOption()
        {
            isPixelSnapping = false;
            volumeBGM = 50;
            volumeSE = 50;
        }
    }

    public static GameManager instance;
    public GameOption gameOption;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)   //单例模式
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        gameOption = new GameOption();

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (2 <= scene.buildIndex)
        {
            //设置PixelSnapping效果
            Camera.main.GetComponent<UnityEngine.U2D.PixelPerfectCamera>().pixelSnapping = gameOption.isPixelSnapping;
        }
    }

}
