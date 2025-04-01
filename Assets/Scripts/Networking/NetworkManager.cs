using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

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
    public static NetworkManager Instance;

    public static Action OnConnectedToServer; // 마스터 서버에 접속했을 때
    public static Action OnRoomEntered; // 룸에 입장했을 때
    public static Action OnRequestFailed; // 네트워크 요청이 실패했을 때
    public static Action OnRoomSeatsUpdated; // Seats 정보가 갱신될 때 
    public static Action<int, bool> OnRoomPlayerUpdated; // 룸 플레이어 리스트가 변동됐을 때

    // 플레이어의 연결 상태
    static ConnectState sConnectState;

    const string _gameVersion = "1"; 
    const bool PlayerEntered = true; // 대기방의 플레이어가 입장/퇴장인지 구분하기 위해 매개변수로 쓰일 const 변수

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
   

    #region 네트워크 작업 요청 콜백 함수들
    public override void OnConnectedToMaster()
    {
        // 마스터 서버 접속 성공 시, 바로 로비 접속 시도
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        // 로비에 조인 성공
        sConnectState = ConnectState.Lobby;

        // StartSceneUI.cs 에서 등록된 이벤트 실행
        OnConnectedToServer?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        // 룸에 조인 성공
         sConnectState = ConnectState.Room;

         // 이벤트를 실행한다 
         OnRoomEntered?.Invoke();
    }

    public override void OnLeftRoom()
    {
        //sConnectState = ConnectState.Lobby;
        Debug.Log("On Left Room()");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 다른 플레이어가 방에 입장한 경우
        OnRoomPlayerUpdated?.Invoke(newPlayer.ActorNumber, PlayerEntered);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 다른 플레이어가 방을 나간 경우 
        OnRoomPlayerUpdated?.Invoke(otherPlayer.ActorNumber, !PlayerEntered);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        OnRoomSeatsUpdated?.Invoke();

        Debug.Log("OnRoomPropertiesUpdate");
    }


    #endregion

    #region 서버 예외 처리 콜백 함수들

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);

        // 랜덤 매치 예외 처리
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);

        // 방 생성 예외 처리
        NetworkHandler.Instance.SetCreateExceptionPanel(returnCode);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        // 조인 예외 처리
        NetworkHandler.Instance.SetJoinExceptionPanel(returnCode);

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("On Disconnected");
        NetworkHandler.Instance.SetDisconnectedExceptionPanel((int)cause, sConnectState);
    }
    #endregion

}
