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
            BackGround.gameObject.SetActive(true);
            pauseMenu.transform.GetChild(0).GetComponent<Button>().Select();            //选中暂停菜单第一项
            Time.timeScale = 0f;
            AudioManager.instance.PlaySingle("Pause");                       //播放暂停音效

        }
        else
        {
            //EventSystem.current.currentSelectedGameObject.GetComponent<Animator>().SetTrigger("deselect");
            Time.timeScale = 1f;
            pauseMenuFucker.Select();                                                   //魔法操作，勿动
            pauseMenu.GetComponent<Animator>().SetTrigger("menuSlideOut");
            StartCoroutine(DelaySetActiveFalse(pauseMenu, .25f));                       //0.25s后关闭子菜单（为了播放动画）
            BackGround.gameObject.SetActive(false);
        }
    }

    //进入子菜单
    public override void MenuEnter(GameObject Menu)
    {
        int menuStackDepth = SelectedObjectInParentMenu.Count;
        SelectedObjectInParentMenu.Add(EventSystem.current.currentSelectedGameObject);          //上级菜单当前选项入栈
        menuStackDepth++;
        var HigherMenu = SelectedObjectInParentMenu[menuStackDepth - 1].transform.parent.gameObject;
        HigherMenu.GetComponent<Animator>().SetTrigger("menuSlideOut");                         //上级菜单滑出
        StartCoroutine(DelaySetActiveFalse(HigherMenu, .25f));                                  //0.25s后关闭上级菜单（为了播放动画）
        Menu.SetActive(true);                                                                   //子菜单启动
        Menu.transform.GetChild(0).GetComponent<Button>().Select();                             //选中子菜单第一项
    }

    //退出子菜单
    public override void MenuQuit(GameObject Menu)
    {
        int menuStackDepth = SelectedObjectInParentMenu.Count;
        if (menuStackDepth > 0)       //判断当前是否在子菜单中
        {
            Menu.GetComponent<Animator>().SetTrigger("menuSlideOut");                               //当前子菜单滑出
            StartCoroutine(DelaySetActiveFalse(Menu, .25f));                                        //0.25s后关闭子菜单（为了播放动画）
            SelectedObjectInParentMenu[menuStackDepth - 1].transform.parent.gameObject.SetActive(true);//上级菜单启动
            SelectedObjectInParentMenu[menuStackDepth - 1].GetComponent<Button>().Select();         //选中上级菜单之前的选项
            SelectedObjectInParentMenu.RemoveAt(menuStackDepth - 1);                                //上级菜单之前的选项出栈
            menuStackDepth--;
        }
    }

    //重新开始
    public void RestartGame()
    {
        Time.timeScale = 1;
        //取消DontDestroyOnLoad
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        //SceneManager.MoveGameObjectToScene(AudioManager.instance.gameObject, SceneManager.GetActiveScene());
        //SceneManager.MoveGameObjectToScene(GameManager.instance.gameObject, SceneManager.GetActiveScene());
        SceneManager.LoadScene(2);      //第一关的BuildIndex是2
    }
}
