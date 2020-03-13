using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    //存储游戏设置
    public class GameOption
    {
        public bool isPixelSnapping;
        [Range(0, 100)] public int volumeBGM;
        [Range(0, 100)] public int volumeSE;

        //默认设置
        public GameOption()
        {
            isPixelSnapping = false;
            volumeBGM = 50;
            volumeSE = 50;
        }
    }

    public static GameManager instance;
    public GameOption gameOption = new GameOption();
    public int buildIndex = 0;

    void Awake()
    {
        if (instance == null)       //单例模式
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;      //加载场景时加载游戏设置
    }

    //加载场景时加载游戏设置
    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        buildIndex = scene.buildIndex;      //获取当前buildIndex
        if (2 <= buildIndex)
        {
            //设置PixelSnapping效果
            Camera.main.GetComponent<UnityEngine.U2D.PixelPerfectCamera>().pixelSnapping = gameOption.isPixelSnapping;

            //设置背景音乐和音效的音量

            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (var source in audioSources)
            {
                if (source.tag == "BGMSource")
                    source.volume = gameOption.volumeBGM;
                else if (source.tag == "SESource")
                    source.volume = gameOption.volumeSE;
            }
        }
    }

}
