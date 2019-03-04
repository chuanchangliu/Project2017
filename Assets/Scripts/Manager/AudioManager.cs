using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class AudioManager : Singleton<AudioManager>
{
    private AudioManager()
    {
        Debug.Log("====private AudioManager()====");
    }

    public void Init()
    {
        Debug.Log("====AudioManager Init====");
    }
}
