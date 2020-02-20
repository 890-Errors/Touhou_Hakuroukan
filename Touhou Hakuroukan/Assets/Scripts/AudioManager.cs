using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioClip seCancel;
    public AudioClip seSelect;
    public AudioClip seOK;
    public AudioClip seInvalid;

    public AudioSource seAudioSource;

    public void PlaySingle(AudioClip clip)
    {
        seAudioSource.clip = clip;
        seAudioSource.Play();
    }
}
