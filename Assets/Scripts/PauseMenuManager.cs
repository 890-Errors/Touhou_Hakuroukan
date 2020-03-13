using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenuManager : MenuManager
{
    new public static PauseMenuManager Instance;
    public GameObject pauseMenu;
    public Button pauseMenuFucker;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.instance.buildIndex >= 2 && Time.timeScale != 0)
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.activeInHierarchy == false)
        {
            pauseMenu.SetActive(true);                                                  //暂停菜单启动
            pauseMenu.transform.GetChild(0).GetComponent<Button>().Select();            //选中暂停菜单第一项
            Time.timeScale = 0f;
            //BackGround.GetComponent<Animator>().SetTrigger("bgDarker");               //背景调暗
            FindObjectOfType<AudioManager>().PlaySingle("Pause");                       //播放暂停音效
            
        }
        else
        {
            //EventSystem.current.currentSelectedGameObject.GetComponent<Animator>().SetTrigger("deselect");
            Time.timeScale = 1f;
            pauseMenuFucker.Select();                                                   //魔法操作，勿动
            pauseMenu.GetComponent<Animator>().SetTrigger("menuSlideOut");
            StartCoroutine(DelaySetActiveFalse(pauseMenu, .25f));                       //0.25s后关闭子菜单（为了播放动画）
        }
    }

    //重新开始
    public void RestartGame()
    {
        SceneManager.LoadScene(2);      //第一关的BuildIndex是2
    }
}
