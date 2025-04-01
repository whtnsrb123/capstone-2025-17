using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
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

    // 랜덤 매치를 요청한다 
    public void RandomRoom()
    {
        // 방 기본 속성
        Hashtable customProperties = new Hashtable
        {
            { "Seats", new int[] {-1, -1, -1, -1} }
        };

        RoomOptions room = new RoomOptions
        {
            MaxPlayers = RequiredPlayerCount,
            CustomRoomProperties = customProperties,
            EmptyRoomTtl = 0
        };

        // 방에 조인을 시도 후, 실패 시 방 생성하기
        PhotonNetwork.JoinOrCreateRoom (
            "Random", // 방 이름
            room, // 방 속성
            TypedLobby.Default // 로비 타입
            );
    }

    // 방 생성을 요청한다 
    public void CreateRoom(string roomName)
    {
        Hashtable customProperties = new Hashtable
        {
            {"Seats", new int[] {-1, -1, -1, -1} }
        };

        RoomOptions room = new RoomOptions
        {
            MaxPlayers = RequiredPlayerCount,
            CustomRoomProperties = customProperties,
            EmptyRoomTtl = 0
        };

        PhotonNetwork.CreateRoom
       (
            roomName,
            room,
            TypedLobby.Default
       );
    }

    // 방 참가를 요청한다 
    public void JoinRoom(string code)
    {
        PhotonNetwork.JoinRoom
       (
            code
       );
    }

    // 방을 나온다 
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        s_players.Clear();
        RenderPlayersUI();
    }


    // 클라이언트의 정보를 전송한다 
    bool isFirstSend = true;

    public void SendClientInfo(string nickname, int characterId)
    {
        Hashtable hash = new Hashtable();
        if (isFirstSend)
        {
            // 처음으로 정보를 전송한 경우
            isFirstSend = false;

            hash.Add(NicknameKey, nickname);
            hash.Add(CharacterIdKey, characterId);
        }
        else
        {
            // 이미 정보를 전송한 경우
            hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash[NicknameKey] = nickname;
            hash[CharacterIdKey] = characterId;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    // MasterClient는 새로운 플레이어 입장 시, 플레이어 표시 순서를 갱신한다
    public void UpdateEnteredPlayerSeats(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] seats = (int[])hash["Seats"];

        for (int i = 0; i < 4; i++)
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
        if (!PhotonNetwork.IsMasterClient)
            return;

        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] seats = (int[])hash["Seats"];

        for (int i = 0; i < 4; i++)
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
    public int[] GetUpdatedPlayerSeats()
    {
        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] seats = (int[])hash["Seats"];
        return seats;
    }

    // 대기방에서 플레이어 표시를 위해 현재 방의 플레이어 정보를 받아온다
    public Dictionary<int, System.Collections.Hashtable> RenderPlayersUI()
    {
        s_players = PhotonNetwork.CurrentRoom.Players;

        // Player의 ActorNumber와 정보를 담은 HashTable 
        Dictionary<int, System.Collections.Hashtable> playersInfo = new Dictionary<int, System.Collections.Hashtable>();

        foreach(KeyValuePair<int, Player> p in s_players)
        {
            Hashtable hash = p.Value.CustomProperties;

            System.Collections.Hashtable newHash = new System.Collections.Hashtable();

            if (hash != null)
            {
                string nickname = (string)hash[NicknameKey];
                int characterId = (int)hash[CharacterIdKey];

                newHash.Add(NicknameKey, nickname);
                newHash.Add(CharacterIdKey , characterId);

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


    // 현재 방의 룸 코드를 받아온다
    public string GetRoomCode()
    {
        string roomCode = PhotonNetwork.CurrentRoom.Name;
        return roomCode;
    }

    // 현재 방에서의 actorNumber를 받아온다
    public int GetActorNumber()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber;
    }

}
