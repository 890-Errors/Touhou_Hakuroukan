using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DeckUIController : MonoBehaviour
{
    public static DeckUIController instance;

    public GameObject DeckContainer;
    public CameraFollower Cursor;
    public GameObject[] deck = new GameObject[5];
    public Text SpellCardName;
    public Text SpellCardDesc;
    public int indexSpellCardLoaded;

    public bool isWaiting = false;

    void Awake()
    {
        if (instance == null)       //单例模式
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        indexSpellCardLoaded = 0;
    }

    //private void Start()
    //{
    //    StartCoroutine("Wait");
    //}

    void Update()
    {       
        if (!isWaiting && DeckContainer.activeInHierarchy)
        {
            //int direction = (int)Input.GetAxisRaw("Horizontal");
            if (Input.GetKeyDown(KeyCode.RightArrow) && indexSpellCardLoaded < deck.Length - 1)
            {
                Cursor.followingObject = deck[++indexSpellCardLoaded];
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && indexSpellCardLoaded > 0)
            {
                Cursor.followingObject = deck[--indexSpellCardLoaded];
            }
            SpellCardName.text = deck[indexSpellCardLoaded].GetComponent<ISpellCard>()?.SpellCardName;
            SpellCardDesc.text = deck[indexSpellCardLoaded].GetComponent<ISpellCard>()?.SpellCardDesc;
        }
    }

    public void ToggleDeck()
    {
        if (!DeckContainer.activeInHierarchy)
        {
            DeckContainer.SetActive(true);
            PauseMenuManager.Instance.BackGround.gameObject.SetActive(true);
        }
        else
        {
            DeckContainer.SetActive(false);
            PauseMenuManager.Instance.BackGround.gameObject.SetActive(false);
        }
    }

    //IEnumerator Wait()
    //{
    //    yield return new WaitForSecondsRealtime(2f);
    //    isWaiting = false;
    //}
}
