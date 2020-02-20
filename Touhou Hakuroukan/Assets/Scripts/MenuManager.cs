using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public float timeDelayStart = .6f;
    public GameObject MainMenu;
    
    public Image BackGround;

   public void StartGame()
    {
        StartCoroutine("DelayStartGame");
    }

    public void Quit()
    {
        Application.Quit();
    }

    //进入子菜单
    public void MenuEnter(GameObject Menu)
    {
        BackGround.GetComponent<Animator>().SetTrigger("bgDarker");     //背景调暗
        MainMenu.GetComponent<Animator>().SetTrigger("menuSlideOut");   //主菜单滑出
        StartCoroutine(DelaySetActiveFalse(MainMenu,.25f));             //0.25s后关闭主菜单
        Menu.SetActive(true);
        Menu.transform.GetChild(0).GetComponent<Button>().Select();     //选中子菜单第一项
    }

    //退出子菜单
    public void MenuQuit(GameObject Menu)
    {
        BackGround.GetComponent<Animator>().SetTrigger("bgBrighter");   //背景调亮
        Menu.GetComponent<Animator>().SetTrigger("menuSlideOut");       //主菜单滑入
        StartCoroutine(DelaySetActiveFalse(Menu, .25f));                //0.25s后关闭子菜单
        MainMenu.SetActive(true);
        MainMenu.transform.GetChild(0).gameObject.GetComponent<Button>().Select();      //选中主菜单第一项
    }

    IEnumerator DelayStartGame()
    {
        yield return new WaitForSeconds(timeDelayStart);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator DelaySetActiveFalse(GameObject gameObject, float timeToDelay)
    {
        yield return new WaitForSeconds(timeToDelay);
        gameObject.SetActive(false);
    }
}
