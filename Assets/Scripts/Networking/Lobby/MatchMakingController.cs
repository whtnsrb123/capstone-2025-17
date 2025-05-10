using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MatchMakingController : MonoBehaviourPunCallbacks
{
    [SerializeField] MatchMakingUIController ui;

    // 접속이 끊긴 플레이어를 룸에 유지할 시간
    // TODO : 테스트 용도로 0으로 설정한다
    private int playerTtl = 0;

    // 아무도 없는 룸을 유지할 시간 
    private int roomTtl = 0;

    public void JoinRandomRoom(string roomName)
    {
        Debug.Log("랜덤 시도");
        // 방 기본 속성
        Hashtable customProperties = new Hashtable
        {
            // 플레이어 ActorNumber를 기록하는 배열이다
            { ServerInfo.PlayerActorNumbersKey, ServerInfo.PlayerActorNumbers.ToArray()},
            // 플레이어 Ready State를 기록하는 배열이다 
            { ServerInfo.ReadyStatesKey, ServerInfo.ReadyStates.ToArray()},
            // 게임이 시작됐는지 확인하는 변수 
            { ServerInfo.IsGameStartKey, false },
        };

        RoomOptions room = new RoomOptions
        {
            MaxPlayers = ServerInfo.RequiredPlayerCount,
            PlayerTtl = playerTtl,
            CustomRoomProperties = customProperties,
            EmptyRoomTtl = roomTtl,
        };

        bool sent = PhotonNetwork.JoinRandomOrCreateRoom
        (
            null, // 검색 조건
            ServerInfo.RequiredPlayerCount, // 최대 플레이어 수
            MatchmakingMode.FillRoom, // 많은 방부터 우선 채우기
            TypedLobby.Default, //  기본 로비만 사용
            null, // sql 로비 필터 없음
            roomName, // 생성 시 방 이름
            room, // 방 옵션
            null // 플레이어 조건 
        );

        // 클라이언트는 방 입장을 요청한 상태
        ClientInfo.sClientState = ConnectState.Room;

        // 접속이 끊겨, 요청이 전송되지 않은 경우
        if (!sent)
            NetworkHandler.Instance.SetJoinExceptionPanel(NetworkHandler.RequestNotSent);

    }

    // 방 생성을 요청한다 
    public void CreateRoom(string roomName)
    {
        Debug.Log("생성 시도");
        Hashtable customProperties = new Hashtable
        {
            // 플레이어 ActorNumber를 기록하는 배열이다
            { ServerInfo.PlayerActorNumbersKey, ServerInfo.PlayerActorNumbers.ToArray()},
            // 플레이어 Ready State를 기록하는 배열이다 
            { ServerInfo.ReadyStatesKey, ServerInfo.ReadyStates.ToArray()},
            // 게임이 시작됐는지 확인하는 변수
            { ServerInfo.IsGameStartKey, false },
        };

        RoomOptions room = new RoomOptions
        {
            MaxPlayers = ServerInfo.RequiredPlayerCount,
            PlayerTtl = playerTtl,
            CustomRoomProperties = customProperties,
            EmptyRoomTtl = roomTtl,
        };

        bool sent = PhotonNetwork.CreateRoom
       (
            roomName,
            room,
            TypedLobby.Default
       );

        // 클라이언트는 방 입장을 요청한 상태
        ClientInfo.sClientState = ConnectState.Room;

        // 접속이 끊겨, 요청이 전송되지 않은 경우
        if (!sent)
            NetworkHandler.Instance.SetCreateExceptionPanel(NetworkHandler.RequestNotSent);
    }

    // 방 참가를 요청한다 
    public void JoinRoom(string code)
    {
        bool sent = PhotonNetwork.JoinRoom
       (
            code
       );

        // 접속이 끊겨, 요청이 전송되지 않은 경우
        if (!sent)
            NetworkHandler.Instance.SetJoinExceptionPanel(NetworkHandler.RequestNotSent);
    }
    public void SendClientInfo(string nickname, int characterId)
    {
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

        // 닉네임 전송
        if (!hash.ContainsKey(ClientInfo.NicknameKey))
        {
            hash.Add(ClientInfo.NicknameKey, nickname);
        }
        else
        {
            hash[ClientInfo.NicknameKey] = nickname;
        }

        // 선택한 스킨 정보 전송
        if (!hash.ContainsKey(ClientInfo.CharacterIdKey))
        {
            hash.Add(ClientInfo.CharacterIdKey, characterId);
        }
        else
        {
            hash[ClientInfo.CharacterIdKey] = characterId;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Lobby가 아닌 곳에서는 해당 콜백을 무시한다
        if ((ClientInfo.sCurrentState == ConnectState.Lobby) && (ClientInfo.sClientState == ConnectState.Lobby))
        {
            base.OnRoomListUpdate(roomList);

            List<string> roomNames = new List<string>();

            // 이름만 전달하기
            foreach (var room in roomList)
            {
                roomNames.Add(room.Name);
            }

            ui.GetRoomNameList(roomNames);
        }
    }
}
