using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Audio;

public class SpeakerManager : MonoBehaviourPun
{
    [Title("Speaker Panel")] public GameObject[] speakerPanels; // inspector 
    [Title("Audio Source")] private AudioMixerGroup speakerMixerGroup;

    // debug
    [Title("For Debug")]
    [ShowInInspector] private TextMeshProUGUI[] speakerNames; // show player's nickname 
    [ShowInInspector] private Dictionary<int, GameObject> playerIdToPanelsMap; // Remotevoice.PlayerId <----> Nickname Panel

    [ShowInInspector] private Speaker[] speakers; // all speaker components in Scene 'Mission1'
    [ShowInInspector] private Dictionary<int, int> playerIdToActorNumberMap; // playerId is not playerActorNumber. playerId is used by the local client to distinguish remote speakers. 

    private Recorder recorder;

    private bool[] wasRemotePlaying;
    private bool wasLocalPlaying;

    private const int myPlayerId = -1;

    private float epslion = 0.005f;
    private bool isSpeakerReady;

    private void Start()
    {
        Debug.Log("start waiting...");
        recorder = GetComponent<Recorder>();
        isSpeakerReady = false;

        StartCoroutine(WaitForSpeakers());
    }

    private void Update()
    {
        if (isSpeakerReady)
        {
            CheckRemoteSpeakerPlaying();
            CheckLocalSpeakerPlaying();
        }
    }

    IEnumerator WaitForSpeakers()
    {
        yield return new WaitForSeconds(3f);

        Debug.Log("start finding...");
        FindSpeakers();
    }

    public void FindSpeakers()
    {
        speakers = FindObjectsOfType<Speaker>();

        playerIdToActorNumberMap = new Dictionary<int, int>();
        playerIdToActorNumberMap.Add(myPlayerId, PhotonNetwork.LocalPlayer.ActorNumber); // add local player

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            // local player can't access RemoteVoice
            if (speakers[i].RemoteVoice != null)
            {
                int actorNumber = speakers[i].transform.parent.GetComponentInChildren<PhotonView>().OwnerActorNr;
                playerIdToActorNumberMap.Add(speakers[i].RemoteVoice.PlayerId, actorNumber);
            }
            else
            {
                Debug.LogWarning($"speakers[{i}].RemoteVoice is null!"); // local client doesn't have RemoteVoice
            }
        }

        isSpeakerReady = true;

        SetVoiceAudioSource();
        InitializeSpeakerPanel();
    }

    private void SetVoiceAudioSource()
    {
        for (int i = 0; i < speakers.Length; i++)
        {
            speakers[i].GetComponent<AudioSource>().outputAudioMixerGroup = speakerMixerGroup;
        }
    }


    private void InitializeSpeakerPanel()
    {
        speakerNames = new TextMeshProUGUI[speakerPanels.Length];
        playerIdToPanelsMap = new Dictionary<int, GameObject>();
        wasRemotePlaying = new bool[speakers.Length];

        int index = 0;
        foreach (var kvp in playerIdToActorNumberMap)
        {
            // Maps playerId to corresponding UI elements. playerId <----> UI 
            playerIdToPanelsMap.Add(kvp.Key, speakerPanels[index]);
            speakerNames[index] = speakerPanels[index].GetComponentInChildren<TextMeshProUGUI>();

            // get player's nickname with playerActorNumber
            PhotonNetwork.CurrentRoom.Players.TryGetValue(kvp.Value, out Player player);

            speakerNames[index].text = player.CustomProperties[ClientInfo.NicknameKey].ToString();
            index++;
        }

        Debug.Log($"Total speakers count {speakers.Length}");

        for (int i = 0; i < speakerPanels.Length && i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (speakers[i].RemoteVoice != null)
            {
                SetRemoteSpeakersUI(speakers[i].RemoteVoice.PlayerId, false);

                // debug
                int notNullPlayer = playerIdToActorNumberMap[speakers[i].RemoteVoice.PlayerId];
                PhotonNetwork.CurrentRoom.Players.TryGetValue(notNullPlayer, out Player player);
                Debug.Log($"{player.CustomProperties[ClientInfo.NicknameKey].ToString()} has RemoteVoice!");
            }
            else
            {
                SetLocalSpeakerUI(myPlayerId, false);
            }
        }

    }

    private void CheckRemoteSpeakerPlaying()
    {
        for (int i = 0; i < speakers.Length; i++)
        {
            if (speakers[i].RemoteVoice == null) { continue; } // local speaker does't have RemoteVoice

            bool isPlaying = speakers[i].IsPlaying;

            if (isPlaying == wasRemotePlaying[i]) continue;

            SetRemoteSpeakersUI(speakers[i].RemoteVoice.PlayerId, isPlaying);

            wasRemotePlaying[i] = isPlaying;
        }
    }

    private void CheckLocalSpeakerPlaying()
    {
        bool isPlaying = false;
        if (recorder.LevelMeter.CurrentAvgAmp > epslion)
        {
            isPlaying = true;
        }

        if (wasLocalPlaying != isPlaying)
        {
            SetLocalSpeakerUI(myPlayerId, isPlaying);
        }

        wasLocalPlaying = isPlaying;
    }

    private void SetRemoteSpeakersUI(int playerId, bool visibility)
    {
        playerIdToPanelsMap[playerId].SetActive(visibility);

        Debug.Log($"{playerIdToPanelsMap[playerId].GetComponentInChildren<TextMeshProUGUI>().text}" +
            (visibility ? " starts speaking" : " stops speaking"));
    }

    private void SetLocalSpeakerUI(int playerId, bool visibility)
    {
        playerIdToPanelsMap[myPlayerId].SetActive(visibility);

        Debug.Log($"local client " + (visibility ? " starts speaking" : " stops speaking"));
    }
}
