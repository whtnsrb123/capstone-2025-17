using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneUI : MonoBehaviour
{
    public Button connectButton;
    public TextMeshProUGUI connectInfoTMP;

    const string CONNECT_SUCCESS = "CONNECT COMPLETED!";
    const string CONNECT_TRY = "ON CONNECTING...";

    void Awake()
    {
        // Master Server 연결 시 실행할 이벤트 등록 
        NetworkManager.OnConnectedToServer += SetConnectedEvent;
    }

    void Start()
    {
        FadeUI.Fade?.Invoke(true);
        connectButton.interactable = false;
        connectInfoTMP.text = CONNECT_TRY;
    }

    private void OnDestroy()
    {
        NetworkManager.OnConnectedToServer -= SetConnectedEvent;
    }

    void SetConnectedEvent()
    {
        connectButton.interactable = true;
        connectInfoTMP.text = CONNECT_SUCCESS;
    }

    public void OnClickConnectButton()
    {
        FadeUI.Fade?.Invoke(false);
        PhotonNetwork.LoadLevel("LobbyScene");
    }

}
