using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource seAudioSource;
    public AudioSource bgmAudioSource;

    //菜单音效
    public AudioClip seCancel;
    public AudioClip seSelect;
    public AudioClip seOK;
    public AudioClip seInvalid;
    public AudioClip sePause;

    //加载场景后的默认BGM
    public AudioClip[] bgmForScene = new AudioClip[10];

    private void Awake()
    {
        if (instance == null)       //单例模式
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject.tag == "NormalButton")
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                PlaySingle("Select");
            if (Input.GetButtonDown("Submit"))
                PlaySingle("Submit");
            if (Input.GetButtonDown("Cancel"))
                PlaySingle("Cancel");
        }
        if (EventSystem.current.currentSelectedGameObject.tag == "CancelButton")
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                PlaySingle("Select");
            if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel"))
                PlaySingle("Cancel");
        }
        if (EventSystem.current.currentSelectedGameObject.tag == "NoCancelButton")
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                PlaySingle("Select");
            if (Input.GetButtonDown("Submit"))
                PlaySingle("Submit");
        }
    }

    public void PlaySingle(AudioClip clip)
    {
        seAudioSource.clip = clip;
        seAudioSource.Play();
    }

    public void PlaySingle(string clip)
    {
        switch (clip)
        {
            case "Cancel":
                PlaySingle(seCancel);
                break;
            case "Select":
                PlaySingle(seSelect);
                break;
            case "Submit":
                PlaySingle(seOK);
                break;
            case "Invalid":
                PlaySingle(seInvalid);
                break;
            case "Pause":
                PlaySingle(sePause);
                break;
            default:
                throw new System.Exception("Invalid name of sound effect.");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //StartCoroutine("DelayPlayBGM");
        bgmAudioSource.clip = bgmForScene[GameManager.instance.buildIndex];
        bgmAudioSource.Play();

    }

    private IEnumerator DelayPlayBGM()
    {
        yield return new WaitForSecondsRealtime(.5f);
        bgmAudioSource.clip = bgmForScene[GameManager.instance.buildIndex];
        bgmAudioSource.Play();
    }

}
