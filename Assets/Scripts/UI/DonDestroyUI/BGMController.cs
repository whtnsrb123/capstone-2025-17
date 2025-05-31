using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;

    private bool isPlaying = false;

    private void Start()
    {
        SoundManager[] otherSoundManagers = FindObjectsOfType<SoundManager>();
        if (otherSoundManagers.Length > 1)
        {
            Debug.Log($"{otherSoundManagers.Length} SoundManagers in this scene");
            Destroy(gameObject);
        }
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LobbyScene" && !isPlaying)
        {
            isPlaying = true;
            soundManager.Play("BGM", ESoundType.BGM);
        }
    }

}
