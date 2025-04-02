using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;

public enum ConnectState
{
    Idle,
    Lobby,
    Room,
    Disconnected
}

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    public static Action OnConnectedToLobby; // 마스터 서버에 접속했을 때
    public static Action<List<string>> OnRoomListUpdated; // 방 목록이 업데이트 됐을 때 
    public static Action OnRoomEntered; // 룸에 입장했을 때
    public static Action OnRoomSeatsUpdated; // Seats 정보가 갱신될 때 
    public static Action<int, bool> OnRoomPlayerUpdated; // 룸 플레이어 리스트가 변동됐을 때

    // 플레이어의 연결 상태
    static ConnectState sCurrentState = ConnectState.Idle; // 실제 네트워크 접속 상태
    static ConnectState sClientState = ConnectState.Idle; // 클라이언트 요청에 따른 접속 상태 

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

    // StartScene 씬이 시작될 때 StartSceneUI.cs에서 호출한다
    // 어플리케이션 내내 존재해야 하는 싱글톤 클래스라서 Destroy 후 Awake, Start를 호출하기 어렵다 
    public void SetUpConnect()
    {
        if (sCurrentState == ConnectState.Idle && sClientState == ConnectState.Idle)
        {
            // 첫 접속인 경우
            DontDestroyOnLoad(gameObject);

            ConnectToMasterServer();
        }
        else if (PhotonNetwork.IsConnected)
        {
            // Room에서 접속이 끊긴 뒤 재시도를 했지만 실패한 경우 
            OnConnectedToLobby?.Invoke();
            sClientState = sCurrentState = ConnectState.Lobby;
        }
        else
        {
            // 이외 접속이 끊긴 뒤 재접속인 경우
            StartCoroutine(TryReconnectToMasterServer());
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

    # region 연결 상태에 대한 프로퍼티 함수들
    public ConnectState GetCurrenttState()
    {
        return sCurrentState;
    }

    public void SetCurrenttState(ConnectState state)
    {
        sCurrentState = state;
    }
    public ConnectState GetClienttState()
    {
        return sClientState;
    }

    public void SetClientState(ConnectState state)
    {
        sClientState = state;
    }
    #endregion

    #region 네트워크 작업 요청 콜백 함수들
    public override void OnConnectedToMaster()
    {
        // 마스터 서버 접속 성공 시, 바로 로비 접속 시도
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        // 로비에 조인 성공
        sCurrentState = sClientState = ConnectState.Lobby;

        // StartSceneUI.cs 에서 등록된 이벤트 실행
        OnConnectedToLobby?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        // 룸에 조인 성공
        sCurrentState = ConnectState.Room;

         // 이벤트를 실행한다 
         OnRoomEntered?.Invoke();
    }

    public override void OnLeftRoom()
    {
        // 방을 나간 경우
        // 클라이언트의 요청이거나, 네트워크 오류로 발생한다
        sCurrentState = ConnectState.Lobby;

        Debug.Log("On Left Room()");
    }
    #endregion

    // Lobby에서 RoomList를 받아오기
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Lobby가 아닌 곳에서는 해당 콜백을 무시한다
        if ((sCurrentState == ConnectState.Lobby) && (sClientState == ConnectState.Lobby))
        {
            base.OnRoomListUpdate(roomList);

            List<string> roomNames = new List<string>();

            // 이름만 전달하기
            foreach (var room in roomList)
            {
                roomNames.Add(room.Name);
            }

            OnRoomListUpdated?.Invoke(roomNames);
        }
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

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        OnRoomSeatsUpdated?.Invoke();

        Debug.Log("OnRoomPropertiesUpdate");
    }


    #region 서버 예외 처리 콜백 함수들

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);

        // 랜덤 매치 예외 처리
        NetworkHandler.Instance.SetJoinExceptionPanel(returnCode);
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
        if ( sClientState == ConnectState.Room)
        {
            // ReconnectAndRejoin이 실패한 경우에 호출된 OnJOinRoomFailed()를 처리한다
            sClientState = ConnectState.Lobby;  // sClientState가 Room이면 다시 Room 재참여를 시도하므로, Lobby로 수정한다 
            NetworkHandler.Instance.SetDisconnectedExceptionPanel(0);
        }
        else
        {
            // 일반적인 Join의 실패 사례 
            NetworkHandler.Instance.SetJoinExceptionPanel(returnCode);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("On Disconnected");
        sCurrentState = ConnectState.Disconnected;

        // Disconnected 예외 처리
        NetworkHandler.Instance.SetDisconnectedExceptionPanel((int)cause);
    }
    #endregion

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
