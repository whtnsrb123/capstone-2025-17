using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static Dictionary<int, Player> s_players;

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

    public void JoinRoom(string code)
    {
        PhotonNetwork.JoinRoom
       (
            code
       );
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        s_players.Clear();
        RenderPlayers();
    }

    bool isFirstSend = true;

    public void SendClientInfo(string nickname, int characterId)
    {
        Hashtable hash = new Hashtable();
        if (isFirstSend)
        {
            isFirstSend = false;

            hash.Add("Nickname", nickname);
            hash.Add("CharacterId", characterId);
        }
        else
        {
            hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["Nickname"] = nickname;
            hash["CharacterId"] = characterId;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public Dictionary<string, int> RenderPlayers()
    {
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

    public string GetRoomCode()
    {
        string roomCode = PhotonNetwork.CurrentRoom.Name;
        return roomCode;
    }

}
