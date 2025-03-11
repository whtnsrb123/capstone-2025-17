using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

// Room 과 관련된 네트워크 작업을 담당하는 클래스
public class RoomManager : MonoBehaviour
{
    public static Dictionary<int, Player> s_players;

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

            hash.Add("Nickname", nickname);
            hash.Add("CharacterId", characterId);
        }
        else
        {
            // 이미 정보를 전송한 경우
            hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["Nickname"] = nickname;
            hash["CharacterId"] = characterId;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public Dictionary<string, int> RenderPlayers()
    {
        // 대기방에서 플레이어 표시를 위해 현재 방의 플레이어 목록을 받아온다
        s_players = PhotonNetwork.CurrentRoom.Players;

        Debug.Log(s_players.Count + "명");

        Dictionary<string, int > playersInfo = new Dictionary<string, int>();


        foreach(KeyValuePair<int, Player> p in s_players)
        {
            Hashtable hash = p.Value.CustomProperties;

            if (hash != null)
            {
                string nickname = (string)hash["Nickname"];
                int characterId = (int)hash["CharacterId"];

                Debug.Log($"Room Manager_ 닉네임 {nickname}, 캐릭터 {characterId}");

                playersInfo.Add(nickname, characterId);
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
