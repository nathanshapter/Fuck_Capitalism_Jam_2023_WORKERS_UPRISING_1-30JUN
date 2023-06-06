using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSource : MonoBehaviour
{
    [SerializeField] AudioClip intro, level1, level2;
    AudioSource source;

    private void Start()
    {
        source= GetComponent<AudioSource>();

        AudioManager.instance.PlayMusic(intro);
    }
}
