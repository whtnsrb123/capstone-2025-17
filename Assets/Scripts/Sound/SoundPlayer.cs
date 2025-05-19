using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    void Awake()
    {
        if (soundManager == null)
        {
            soundManager = FindObjectOfType<SoundManager>();
        }
    }


    public void PlayBGM(string bgmName)
    {
        soundManager.Play(bgmName, ESoundType.BGM);
    }

    public void PlaySFX(string sfxName)
    {
        soundManager.Play(sfxName, ESoundType.SFX);
    }
}
