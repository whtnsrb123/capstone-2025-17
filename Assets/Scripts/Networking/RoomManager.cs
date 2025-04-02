using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

// Room 과 관련된 네트워크 작업을 담당하는 클래스
public class RoomManager : MonoBehaviour
{
    public static Dictionary<int, Player> s_players;

    const int RequiredPlayerCount = 4;

    // 해시 키
    const string NicknameKey = "Nickname";
    const string CharacterIdKey = "CharacterId";

    // 접속이 끊긴 플레이어를 룸에 유지할 시간
    private int playerTtl = 10000;
    
    // 아무도 없는 룸을 유지할 시간 
    private int roomTtl = 0;

    // ========================== 매치 메이킹 ====================================

    // 랜덤 매치를 요청한다 
    public void JoinRandomRoom(string roomName)
    {
        // 방 기본 속성
        Hashtable customProperties = new Hashtable
        {
            // 플레이어 ActorNumber를 기록하는 배열이다
            { "Seats", new int[] {-1, -1, -1, -1} },
            // 랜덤 매치인 방을 검색하거나, 생성하는 데 사용된다
            { "MatchType", "Random"}
        };

        // MatchType이 RandomMatch 인 방만 찾도록 한다
        Hashtable expectedCustomProperties = new Hashtable
        {
            { "MatchType", "Random"}
        };

        RoomOptions room = new RoomOptions
        {
            MaxPlayers = RequiredPlayerCount,
            PlayerTtl = playerTtl,
            CustomRoomProperties = customProperties,
            CustomRoomPropertiesForLobby = new string[] {"MatchType"}, // 로비에서 검색할 방 속성 지정
            EmptyRoomTtl = roomTtl,
        };

        bool sent = PhotonNetwork.JoinRandomOrCreateRoom
        (
            expectedCustomProperties, // 검색 조건
            RequiredPlayerCount, // 최대 플레이어 수
            MatchmakingMode.FillRoom, // 많은 방부터 우선 채우기
            TypedLobby.Default, //  기본 로비만 사용
            null, // sql 로비 필터 없음
            roomName, // 생성 시 방 이름
            room, // 방 옵션
            null // 플레이어 조건 
        );

        // 클라이언트는 방 입장을 요청한 상태
        NetworkManager.sClientState = ConnectState.Room;

        // 접속이 끊겨, 요청이 전송되지 않은 경우
        if (!sent)
            NetworkHandler.Instance.SetJoinExceptionPanel(NetworkHandler.RequestNotSent);

    }

    // 방 생성을 요청한다 
    public void CreateRoom(string roomName)
    {
        Hashtable customProperties = new Hashtable
        {
            {"Seats", new int[] {-1, -1, -1, -1} },
            // 랜덤 매치를 시도하는 클라이언트에게 제외되도록 한다
            { "MatchType",  "Create"}
        };

        RoomOptions room = new RoomOptions
        {
            MaxPlayers = RequiredPlayerCount,
            PlayerTtl = playerTtl,
            CustomRoomProperties = customProperties,
            CustomRoomPropertiesForLobby = new string[] { "MatchType" },
            EmptyRoomTtl = roomTtl,
        };

        bool sent = PhotonNetwork.CreateRoom
       (
            roomName,
            room,
            TypedLobby.Default
       );

        // 클라이언트는 방 입장을 요청한 상태
        NetworkManager.sClientState = ConnectState.Room;

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

    // 방을 나온다 
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        s_players.Clear();

        // 클라이언트는 방 퇴장을 요청한 상태
        NetworkManager.sClientState = ConnectState.Lobby;
    }

    // =========================== 플레이어 정보 처리 =================================

    // 클라이언트의 정보를 전송한다 
    public void SendClientInfo(string nickname, int characterId)
    {
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        
       // 닉네임 전송
        if (!hash.ContainsKey(NicknameKey))
        {
            hash.Add(NicknameKey, nickname);
        }
        else
        {
            hash[NicknameKey] = nickname;
        }

        // 선택한 스킨 정보 전송
        if (!hash.ContainsKey(CharacterIdKey))
        {
            hash.Add(CharacterIdKey, characterId);
        }
        else
        {
            hash[CharacterIdKey] = characterId;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    // MasterClient는 새로운 플레이어 입장 시, 플레이어 표시 순서를 갱신한다
    public void UpdateEnteredPlayerSeats(int actorNumber)
    {
        if (!IsMasterClient())
            return;

        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] seats = (int[])hash["Seats"];

        for (int i = 0; i < RequiredPlayerCount; i++)
        {
            // 빈 자리를 발견
            if (seats[i] == -1)
            {
                seats[i] = actorNumber;
                hash["Seats"] = seats;

                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

                break;
            }
        }
    }

    // MasterClient는 플레이어 퇴장 시, 플레이어 표시 순서를 갱신한다
    public void UpdateLeftPlayerSeats(int actorNumber)
    {
        if (!IsMasterClient())
            return;

        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] seats = (int[])hash["Seats"];

        for (int i = 0; i < RequiredPlayerCount; i++)
        {
            // 떠난 플레이어 발견 
            if (seats[i] == actorNumber)
            {
                seats[i] = -1;
                hash["Seats"] = seats;

                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

                break;
            }
        }
    }

    // 갱신된 플레이어 표시 순서 정보를 받아온다 
    public int[] GetPlayerSeats()
    {
        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] seats = (int[])hash["Seats"];
        return seats;
    }

    // 대기방에서 플레이어 표시를 위해 현재 방의 플레이어 정보를 받아온다
    public Dictionary<int, System.Collections.Hashtable> GetPlayerInRoomInfos()
    {
        s_players = PhotonNetwork.CurrentRoom.Players;

        // Player의 ActorNumber와 정보를 담은 HashTable 
        Dictionary<int, System.Collections.Hashtable> playersInfo = new Dictionary<int, System.Collections.Hashtable>();

        foreach (KeyValuePair<int, Player> p in s_players)
        {
            Hashtable hash = p.Value.CustomProperties;

            System.Collections.Hashtable newHash = new System.Collections.Hashtable();

            if (hash != null)
            {
                string nickname = (string)hash[NicknameKey];
                int characterId = (int)hash[CharacterIdKey];

                newHash.Add(NicknameKey, nickname);
                newHash.Add(CharacterIdKey, characterId);

                playersInfo.Add(p.Key, newHash);
            }
            else
            {
                // hash가 비어있는 경우 임시로 기본 값 설정
                newHash.Add(NicknameKey, $"USER_{Random.Range(100, 999)}");
                newHash.Add(CharacterIdKey, 0);

                playersInfo.Add(p.Key, newHash);
            }
        }
        return playersInfo;
    }

    // 현재 방에서의 actorNumber를 받아온다
    public int GetActorNumber()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber;
    }

    public bool IsMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }

    // 현재 방의 룸 코드를 받아온다
    public string GetRoomCode()
    {
        string roomCode = PhotonNetwork.CurrentRoom.Name;
        return roomCode;
    }
}
