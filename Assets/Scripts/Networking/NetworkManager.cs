using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum ConnectState
{
    Idle,
    Lobby,
    Room,
    InGame,
    Disconnected
}

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // 플레이어의 연결 상태
    static ConnectState sConnectState;

    static string _gameVersion = "1";

    public static Action OnConnectedToServer;
    public static Action OnRoomPlayerEntered;
    public static Action OnRoomEntered;
    public static Action OnRequestFailed;

    // =================== int로 변경해야 한다 : ActorNumber 사용 ===============
    public static Action<string> OnRoomPlayerLeaved;


    void Start()
    {
        sConnectState = ConnectState.Idle;

        DontDestroyOnLoad(gameObject);

        ConnectToMasterServer();
    }

    public static void ConnectToMasterServer()
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


    #region Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("NetworkManager.cs - On Connected To Master()");

        // 마스터 서버 접속 성공 시, 바로 로비 접속 시도
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("NetworkManager.cs - On Disconnected()");

        // 연결 실패 시, 재접속 시도 
        int retry = 0,  maxRetry = 5;
        if (retry < maxRetry)
        {
            retry++;

            Debug.Log("NetworkManager.cs - retry connect to master server");

            Invoke(nameof(ConnectToMasterServer), 2f);
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("On Joined Lobby()");

        sConnectState = ConnectState.Lobby;

        // StartSceneUI.cs 에서 등록된 이벤트
        OnConnectedToServer?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        // 방에 성공적으로 접속 시, 플레이어 정보를 서버에 전송하기
        if (PhotonNetwork.IsConnected)
        {
            sConnectState = ConnectState.Room;

            // 이벤트를 실행한다 
            OnRoomEntered?.Invoke();
            OnRoomPlayerEntered?.Invoke();
        }
        else
        {
            // 연결 실패 처리
        }
        
    }
    public override void OnLeftRoom()
    {
        Debug.Log("On Left Room()");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // 룸 연결 실패 처리
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("NetworkManager.cs - On Created Room()");
        Debug.Log($"NetworkManager.cs - {PhotonNetwork.CurrentRoom.Name}");
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnRoomPlayerEntered?.Invoke();
        Debug.Log("Entered Player");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        string nickname = (string)otherPlayer.CustomProperties["Nickname"];
        Debug.Log("NetworkManager - 나간 새끼 : " + nickname);
        OnRoomPlayerLeaved?.Invoke(nickname);
    }
    #endregion

}
