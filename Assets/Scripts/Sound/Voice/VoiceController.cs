using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// VoiceManager 의 컴포넌트로 부착해야 한다 
/// </summary>
public class VoiceController : MonoBehaviour
{
    #region Inspector 
    [SerializeField] private Toggle transmitVoice;
    [SerializeField] private Toggle autoJoinVoice;

    [SerializeField] private List<Object> excludedSceneList;

    #endregion

    private PunVoiceClient voiceClient;
    private Recorder recorder;

    private const string RecordWhenJoin = "RecordWhenJoin";
    private const string TransmitVoice = "TransmitVoice";
    private const string SamplingRate = "SamplingRate";

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

        LoadSetting();
/*        ConnectUI();
*/    }



    private void LoadSetting()
    {
        // pun2와 동일한 세팅 사용
        voiceClient.UsePunAppSettings = true;
        voiceClient.UsePunAuthValues = true;

        // Load Setting 
        // 방 입장 시 자동 참가 
        if (PlayerPrefs.HasKey(RecordWhenJoin))
        {
            recorder.RecordWhenJoined = PlayerPrefs.GetInt(RecordWhenJoin) == 1 ? true : false;
        }
        else
        {
            recorder.RecordWhenJoined = true;
        }

        // 전송 여부 
        if (PlayerPrefs.HasKey(TransmitVoice))
        {
            recorder.TransmitEnabled = PlayerPrefs.GetInt(TransmitVoice) == 1 ? true : false;
        }
        else
        {
            recorder.TransmitEnabled = true;
        }


        if (PlayerPrefs.HasKey(SamplingRate))
        {
            // recorder.SamplingRate = PlayerPrefs.GetInt(SamplingRate);
        }
    }

    private void ConnectUI()
    {
        transmitVoice.onValueChanged.AddListener(SetTransmitVoice);
        autoJoinVoice.onValueChanged.AddListener(SetAutoJoinVoice);
    }

    private void SetTransmitVoice(bool isOn)
    {
        recorder.TransmitEnabled = isOn;

        PlayerPrefs.SetInt(TransmitVoice, (isOn ? 1 : 0));
    }

    private void SetAutoJoinVoice(bool isOn)
    {
        recorder.RecordWhenJoined = isOn;

        PlayerPrefs.SetInt(RecordWhenJoin, (isOn ? 1 : 0));
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
