using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource seAudioSource;
    public AudioSource bgmAudioSource;

    //菜单音效
    public AudioClip seCancel;
    public AudioClip seSelect;
    public AudioClip seOK;
    public AudioClip seInvalid;

    //加载场景后的默认BGM
    public AudioClip[] bgmForScene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
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
            default:
                throw new System.Exception("Invalid name of sound effect.");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        bgmAudioSource.clip = bgmForScene[SceneManager.GetActiveScene().buildIndex];
        bgmAudioSource.Play();
    }


}
