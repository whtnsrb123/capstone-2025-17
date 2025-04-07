using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    // lobby 
    public Button randomBtn;

    // room panel
    public Button readyOrStartBtn;
    public Button leaveBtn;
    public TextMeshProUGUI roomCode;

    public GameObject[] playersUI;
    public TextMeshProUGUI[] nicknamesUI;
    public GameObject[] playersRawImage;
    public TextMeshProUGUI[] playersReadyStatesUI;

    // create panel
    public Button c_confirmBtn;
    public Button c_cancelBtn;

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


    public void UpdatePlayerUI(Dictionary<int, Hashtable> updatedPlayers)
    {
        viewPlayerList = updatedPlayers;

        for (int i = 0; i < viewSeats.Length; i++)
        {
            if (viewSeats[i] == -1)
            {
                playersUI[i].SetActive(false);
                nicknamesUI[i].text = string.Empty;
                playersRawImage[i].SetActive(false);
                playersReadyStatesUI[i].text = string.Empty;
            }
            else
            {
                foreach (KeyValuePair<int, Hashtable> kvp in updatedPlayers)
                {
                    if(kvp.Key == viewSeats[i])
                    {
                        int characterId = (int)kvp.Value[ClientInfo.CharacterIdKey];
                        string nickname = (string)kvp.Value[ClientInfo.NicknameKey];

                        playersUI[i].SetActive(true);
                        playersRawImage[i].SetActive(true);
                        playersReadyStatesUI[i].text = (viewReadyStates[i] ? "Ready" : "Not Ready");

                        smRenderers[i].material = storage.GetMesh(characterId);
                        nicknamesUI[i].text = nickname;
                    }
                }
            }

        }
    }

    public void InitPanel()
    {
        viewPlayerList.Clear();
        viewSeats = null;
        viewReadyStates = null;
    }

}
