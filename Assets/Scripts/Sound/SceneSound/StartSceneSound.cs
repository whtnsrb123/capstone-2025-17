using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneSound : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;

    public void OnClickStartBtn()
    {
        soundManager.Play("buttonClick", ESoundType.SFX);
    }

}
