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

    const string CONNECT_SUCCESS = "네트워크에 접속했습니다!";
    const string CONNECT_TRY = "네트워크 접속 중...";


    void Start()
    {
        FadeUI.Fade?.Invoke(true);

        // Master Server 연결 시 실행할 이벤트 등록 
        ServerConnector.OnConnectedToLobby += SetConnectedEvent;

        connectButton.interactable = false;
        connectInfoTMP.text = CONNECT_TRY;

        // StartScene 씬이 시작될 때, 서버 접속을 시도한다
        // 네트워크 예외 발생 시, 시작 화면으로 돌아오기 때문에 해당 함수에서 호출한다
        ServerConnector.Instance.SetUpConnect();
    }

    private void OnDestroy()
    {
        ServerConnector.OnConnectedToLobby -= SetConnectedEvent;
    }

    void SetConnectedEvent()
    {
        connectButton.interactable = true;
        connectInfoTMP.text = CONNECT_SUCCESS;
    }

    public void OnClickConnectButton()
    {
        FadeUI.Fade?.Invoke(false);
        if (LoadingPanel.Instance != null)
        {
            LoadingPanel.Instance.BeforeLoadedLobbyScene();
        }
        PhotonNetwork.LoadLevel("LobbyScene");
    }

}
