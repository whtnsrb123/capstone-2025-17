using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private string bgmName;

    private void Start()
    {
        //PlayBGM(bgmName);
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
