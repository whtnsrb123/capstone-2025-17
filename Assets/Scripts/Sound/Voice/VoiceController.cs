using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Xml.Serialization;

/// <summary>
/// VoiceManager 의 컴포넌트로 부착해야 한다 
/// </summary>
public class VoiceController : MonoBehaviour
{
    #region Inspector 
    [SerializeField] private AudioMixerGroup chatGroup;

    [SerializeField] private List<Object> excludedSceneList;

    #endregion

    private PunVoiceClient voiceClient;
    private Recorder recorder;


    private void Awake()
    {
        voiceClient = GetComponent<PunVoiceClient>();
        recorder = GetComponent<Recorder>();
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
        for (int i = 0; i < excludedSceneList.Count; i++)
        {
            if (scene.name == excludedSceneList[i].name)
            {
                Debug.Log("voice controller doesn't work on this scene");
                return;
            }
        }
    }



    #region Test

    private void Update()
    {
        Test_VoiceOnOff();
    }

    private void Test_VoiceOnOff()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled;
            Debug.Log("my voice transmitt is" + (recorder.TransmitEnabled ? "enabled" : "stopped"));
        }
    }
    #endregion

}
