using UnityEngine;

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
            case "OK":
                PlaySingle(seOK);
                break;
            case "Invalid":
                PlaySingle(seInvalid);
                break;
            default:
                throw new System.Exception("Invalid name of sound effect.");
        }
    }


}
