using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource musicSource, effectsSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else { Destroy(this.gameObject); }
    }


    public void PlaySound(AudioClip clip)
    {
        effectsSource.PlayOneShot(clip);
    }

    public void StopSound(AudioClip clip)
    {
        effectsSource.Stop();
    }

    public void ChangeMasterVolume(float volume) // this does not yet do anything need to have options to turn off sfx and music
    {
        AudioListener.volume = volume;
    }



}
