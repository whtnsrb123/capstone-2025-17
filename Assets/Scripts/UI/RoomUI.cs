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

    [SerializeField]
    MaterialStorage storage;

    static Dictionary<string, int> viewPlayerList;

    public void RenderPlayerUI(Dictionary<string, int> players)
    {

        viewPlayerList = players;

        int playerIdx = 0;

        foreach(KeyValuePair<string, int> kvp in players)
        {
            Debug.Log($"{playerIdx} 번째 플레이어의 닉네임 : {kvp.Key}, 아이디 : {kvp.Value}");

            playersUI[playerIdx].SetActive(true);
            playersRawImage[playerIdx].SetActive(true);

            SkinnedMeshRenderer sm = playersUI[playerIdx].GetComponentInChildren<SkinnedMeshRenderer>();

            sm.material = storage.GetMesh(kvp.Value);
            nicknamesUI[playerIdx].text = kvp.Key;

            playerIdx++;
        }
    }

    public void RemovePlayerUI(string nickname)
    {
        int playerIdx = 0;

        foreach (KeyValuePair<string, int> p in viewPlayerList)
        {
            Debug.Log($"키 : {p.Key}, 닉네임 : {nickname}이니까, {p.Key == nickname}" );
            if (p.Key == nickname)
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
