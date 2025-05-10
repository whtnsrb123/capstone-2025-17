using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;

public class ServerConnector : MonoBehaviourPunCallbacks
{
    public static ServerConnector Instance { get; private set; }

    public static Action OnConnectedToLobby; // 마스터 서버에 접속했을 때

    // 플레이어의 연결 상태

    const string _gameVersion = "1";
    const bool PlayerEntered = true; // 대기방의 플레이어가 입장/퇴장인지 구분하기 위해 매개변수로 쓰일 const 변수

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // StartScene 씬이 시작될 때 StartSceneUI.cs에서 호출한다
    // 어플리케이션 내내 존재해야 하는 싱글톤 클래스라서 Destroy 후 Awake, Start를 호출하기 어렵다 
    public void SetUpConnect()
    {
        if (ClientInfo.sCurrentState == ConnectState.Idle && ClientInfo.sClientState == ConnectState.Idle)
        {
            // 첫 접속인 경우
            DontDestroyOnLoad(gameObject);

            ConnectToMasterServer();
        }
        else if (PhotonNetwork.IsConnected)
        {
            // Room에서 접속이 끊긴 뒤 ReconnectAndRejoin()을 했지만 실패한 경우 
            Debug.Log("Room 재접속 실패 후, 네트워크 재연결");
            OnConnectedToLobby?.Invoke();
            ClientInfo.sClientState = ClientInfo.sCurrentState = ConnectState.Lobby;
        }
        else
        {
            // 이외 접속이 끊긴 뒤 재접속인 경우
            // StartCoroutine(TryReconnectToMasterServer());
        }
    }

    public void ConnectToMasterServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            // 씬 동기화
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = _gameVersion;

            // 접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        // 마스터 서버 접속 성공 시, 바로 로비 접속 시도
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        // 로비에 조인 성공
        ClientInfo.sCurrentState = ClientInfo.sClientState = ConnectState.Lobby;

        // StartSceneUI.cs 에서 등록된 이벤트 실행
        OnConnectedToLobby?.Invoke();
    }

    // 서버와의 접속이 끊겼을 경우, 재시도 함수
    IEnumerator TryReconnectToMasterServer()
    {
        int retry = 5;

        for (int r = 0; r < retry; r++)
        {
            // 2초마다 호출
            Invoke(nameof(this.ConnectToMasterServer), 0f);

            if (PhotonNetwork.IsConnected)
            {
                yield break;
            }
            yield return new WaitForSeconds(2f);
        }

    }
}