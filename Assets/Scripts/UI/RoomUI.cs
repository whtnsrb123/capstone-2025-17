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


    // 대기 방에서 플레이어를 표시한다 
    public void RenderPlayerUI(Dictionary<int, Hashtable> players)
    {

        viewPlayerList = players;

        int playerIdx = 0;

        foreach(KeyValuePair<int, Hashtable> kvp in players)
        {
            Debug.Log($"{playerIdx} 번째 플레이어의 고유 아이디 : {kvp.Key}");

            playersUI[playerIdx].SetActive(true);
            playersRawImage[playerIdx].SetActive(true);

            SkinnedMeshRenderer sm = playersUI[playerIdx].GetComponentInChildren<SkinnedMeshRenderer>();

            int characterId = (int) kvp.Value["CharacterId"];
            string nickname = (string)kvp.Value["Nickname"];

            sm.material = storage.GetMesh(characterId);
            nicknamesUI[playerIdx].text = nickname;

            playerIdx++;
        }
    }

    // 대기 방을 떠난 플레이어를 UI에서 제거한다

    public void RemovePlayerUI(int actorNumber)
    {
        int playerIdx = 0;

        foreach (KeyValuePair<int, Hashtable> p in viewPlayerList)
        {
            if (p.Key == actorNumber)
            {
                playersUI[playerIdx].SetActive (false);
                nicknamesUI[playerIdx].text = string.Empty;
                playersRawImage[playerIdx].SetActive(false);

                viewPlayerList.Remove(p.Key);

                return;
            }
            playerIdx++;
        }

    }

}
