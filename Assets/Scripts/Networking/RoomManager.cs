using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

// Room 과 관련된 네트워크 작업을 담당하는 클래스
public class RoomManager : MonoBehaviour
{
    public static Dictionary<int, Player> s_players;

    // 해시 키
    const string NicknameKey = "Nickname";
    const string CharacterIdKey = "CharacterId";

    // 랜덤 매치를 요청한다 
    public void RandomRoom()
    {
        Debug.Log("Join Or Create Room()");

        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 4;

        PhotonNetwork.JoinOrCreateRoom(
            "Random",
            room,
            TypedLobby.Default
            );
    }

    // 방 생성을 요청한다 
    public void CreateRoom(string roomName, int maxPlayer = 4)
    {
        RoomOptions room = new RoomOptions();
        room.MaxPlayers = maxPlayer;

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
        RenderPlayers();
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

    // 대기방에서 플레이어 표시를 위해 현재 방의 플레이어 정보를 받아온다
    public Dictionary<int, System.Collections.Hashtable> RenderPlayers()
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

}
