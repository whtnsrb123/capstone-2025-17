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
    public Button leaveBtn;
    public TextMeshProUGUI roomCode;
    public GameObject[] playersUI;
    public TextMeshProUGUI[] nicknamesUI;
    public GameObject[] playersRawImage;

    // create panel
    public Slider maxPlayerCount;
    public Button c_confirmBtn;
    public Button c_cancelBtn;

    // joine panel
    public TMP_InputField roomCodeTMPInp;
    public Button j_confirmBtn;
    public Button j_cancelBtn;

    // 캐릭터 모델의 메시가 저장된 Scriptable Object 변수 
    [SerializeField]
    MaterialStorage storage;

    static Dictionary<int, Hashtable> viewPlayerList;
    static int[] viewSeats;

    public void GetPlayerSeats(int[] para)
    {
        Debug.Log("GetPlayerSeats");
        viewSeats = para;
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
            }
            else
            {
                foreach (KeyValuePair<int, Hashtable> kvp in updatedPlayers)
                {
                    if(kvp.Key == viewSeats[i])
                    {
                        playersUI[i].SetActive(true);
                        playersRawImage[i].SetActive(true);

                        SkinnedMeshRenderer sm = playersUI[i].GetComponentInChildren<SkinnedMeshRenderer>();

                        int characterId = (int)kvp.Value["CharacterId"];
                        string nickname = (string)kvp.Value["Nickname"];

                        sm.material = storage.GetMesh(characterId);
                        nicknamesUI[i].text = nickname;
                    }
                }
            }

        }
    }

}
