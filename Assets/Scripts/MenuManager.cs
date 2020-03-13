using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public float timeDelayStart = .6f;
    public Image BackGround;
    public static MenuManager Instance;

    //用List作为菜单选项堆栈
    protected List<GameObject> SelectedObjectInParentMenu = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }



    public void ToNextScene()
    {
        StartCoroutine("DelayToNextScene");
    }


    public void Quit()
    {
        Application.Quit();
    }

    //进入子菜单
    public virtual void MenuEnter(GameObject Menu)
    {
        BackGround.GetComponent<Animator>().SetTrigger("bgDarker");                             //背景调暗
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
    public virtual void MenuQuit(GameObject Menu)
    {
        int menuStackDepth = SelectedObjectInParentMenu.Count;
        if (menuStackDepth > 0)       //判断当前是否在子菜单中
        {
            BackGround.GetComponent<Animator>().SetTrigger("bgBrighter");                           //背景调亮
            Menu.GetComponent<Animator>().SetTrigger("menuSlideOut");                               //当前子菜单滑出
            StartCoroutine(DelaySetActiveFalse(Menu, .25f));                                        //0.25s后关闭子菜单（为了播放动画）
            SelectedObjectInParentMenu[menuStackDepth - 1].transform.parent.gameObject.SetActive(true);//上级菜单启动
            SelectedObjectInParentMenu[menuStackDepth - 1].GetComponent<Button>().Select();         //选中上级菜单之前的选项
            SelectedObjectInParentMenu.RemoveAt(menuStackDepth - 1);                                //上级菜单之前的选项出栈
            menuStackDepth--;
        }
    }

    protected IEnumerator DelayToNextScene()
    {
        yield return new WaitForSecondsRealtime(timeDelayStart);
        SceneManager.LoadScene(GameManager.instance.buildIndex + 1);
    }

    protected IEnumerator DelaySetActiveFalse(GameObject gameObject, float timeToDelay)
    {
        yield return new WaitForSecondsRealtime(timeToDelay);
        gameObject.SetActive(false);
    }
}
