using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [Header("Match Making Button")]
    // lobby 
    public Button randomBtn;

    [Header("Room")]
    // room panel
    public Button readyOrStartBtn;
    public Button leaveBtn;
    public TextMeshProUGUI roomCode;

    public GameObject[] playersUI;
    public TextMeshProUGUI[] nicknamesUI;
    public GameObject[] playersRawImage;
    public TextMeshProUGUI[] playersReadyStatesUI;

    [Header("Create Room")]
    // create panel
    public Button c_confirmBtn;
    public Button c_cancelBtn;

    [Header("Join Room")]
    // joine panel
    public TMP_InputField roomCodeTMPInp;
    public Button j_confirmBtn;
    public Button j_cancelBtn;

    // 캐릭터 모델의 메시가 저장된 Scriptable Object 변수 
    [SerializeField]
    MaterialStorage storage;

    SkinnedMeshRenderer[] smRenderers;
    Dictionary<int, Hashtable> viewPlayerList;
    int[] viewSeats;
    bool[] viewReadyStates;

    private void Start()
    {
        SetSkinnedMeshRenderers();
    }

    public void GetPlayerSeats(int[] para)
    {
        viewSeats = para;
    }
    public void GetPlayerReadyStates(bool[] para)
    {
        viewReadyStates = para;
    }


    void SetSkinnedMeshRenderers()
    {
        smRenderers = new SkinnedMeshRenderer[4];
        for (int i = 0; i < 4; i++)
        {
            smRenderers[i] = playersUI[i].GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }

    public void UpdatePlayer(int index, int actorNumber)
    {
        if (actorNumber == -1)
        {
            // 플레이어 퇴장
            playersUI[index].SetActive(false);
            nicknamesUI[index].text = string.Empty;
            playersRawImage[index].SetActive(false);
            playersReadyStatesUI[index].text = string.Empty;
        }
        else
        {
            int characterId = (int)PhotonNetwork.CurrentRoom.Players[actorNumber].CustomProperties[ClientInfo.CharacterIdKey];
            string nickname = (string)PhotonNetwork.CurrentRoom.Players[actorNumber].CustomProperties[ClientInfo.NicknameKey];

            Debug.Log($"{index} 번째 플레이어의 아이디는 {actorNumber} 닉네임은 {nickname}");

            playersUI[index].SetActive(true);
            playersRawImage[index].SetActive(true);
            playersReadyStatesUI[index].text = ServerInfo.ReadyStates[index] ? "준비 완료" : "준비 중";

            smRenderers[index].material = storage.GetMesh(characterId);
            nicknamesUI[index].text = nickname;
        }
    }

    public void UpdateReadyState(int index, bool ready)
    {
        playersReadyStatesUI[index].text = ServerInfo.ReadyStates[index] ? "준비 완료" : "준비 중";
    }

    public void InitPanel()
    {
        for (int i = 0; i < ServerInfo.PlayerActorNumbers.Length; i++)
        {
            UpdatePlayer(i, ServerInfo.PlayerActorNumbers[i]);
            Debug.Log($"{i} 번째에  {ServerInfo.PlayerActorNumbers[i]} 를 그리다");
        }
    }

}
