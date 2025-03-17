using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

// Room �� ���õ� ��Ʈ��ũ �۾��� ����ϴ� Ŭ����
public class RoomManager : MonoBehaviour
{
    public static Dictionary<int, Player> s_players;

    // �ؽ� Ű
    const string NicknameKey = "Nickname";
    const string CharacterIdKey = "CharacterId";

    // ���� ��ġ�� ��û�Ѵ� 
    public void RandomRoom(int maxPlayer = 4)
    {
        // �� �⺻ �Ӽ�
        Hashtable customProperties = new Hashtable
        {
            { "Seats", new int[] {-1, -1, -1, -1} }
        };

        RoomOptions room = new RoomOptions
        {
            MaxPlayers = maxPlayer,
            CustomRoomProperties = customProperties,
            EmptyRoomTtl = 0
        };

        // �濡 ������ �õ� ��, ���� �� �� �����ϱ�
        PhotonNetwork.JoinOrCreateRoom (
            "Random", // �� �̸�
            room, // �� �Ӽ�
            TypedLobby.Default // �κ� Ÿ��
            );
    }

    // �� ������ ��û�Ѵ� 
    public void CreateRoom(string roomName, int maxPlayer = 4)
    {
        Hashtable customProperties = new Hashtable
        {
            {"Seats", new int[] {-1, -1, -1, -1} }
        };

        RoomOptions room = new RoomOptions
        {
            MaxPlayers = maxPlayer,
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

    // �� ������ ��û�Ѵ� 
    public void JoinRoom(string code)
    {
        PhotonNetwork.JoinRoom
       (
            code
       );
    }

    // ���� ���´� 
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        s_players.Clear();
        RenderPlayersUI();
    }


    // Ŭ���̾�Ʈ�� ������ �����Ѵ� 
    bool isFirstSend = true;

    public void SendClientInfo(string nickname, int characterId)
    {
        Hashtable hash = new Hashtable();
        if (isFirstSend)
        {
            // ó������ ������ ������ ���
            isFirstSend = false;

            hash.Add(NicknameKey, nickname);
            hash.Add(CharacterIdKey, characterId);
        }
        else
        {
            // �̹� ������ ������ ���
            hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash[NicknameKey] = nickname;
            hash[CharacterIdKey] = characterId;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    // MasterClient�� ���ο� �÷��̾� ���� ��, �÷��̾� ǥ�� ������ �����Ѵ�
    public void UpdateEnteredPlayerSeats(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] seats = (int[])hash["Seats"];

        for (int i = 0; i < 4; i++)
        {
          // �� �ڸ��� �߰�
            if (seats[i] == -1)
            {
                seats[i] = actorNumber;
                hash["Seats"] = seats;

                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

                break;
            }
        }
     }

    // MasterClient�� �÷��̾� ���� ��, �÷��̾� ǥ�� ������ �����Ѵ�
    public void UpdateLeftPlayerSeats(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] seats = (int[])hash["Seats"];

        for (int i = 0; i < 4; i++)
        {
            // ���� �÷��̾� �߰� 
            if (seats[i] == actorNumber)
            {
                seats[i] = -1;
                hash["Seats"] = seats;

                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

                break;
            }
        }
    }

    // ���ŵ� �÷��̾� ǥ�� ���� ������ �޾ƿ´� 
    public int[] GetUpdatedPlayerSeats()
    {
        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] seats = (int[])hash["Seats"];
        return seats;
    }

    // ���濡�� �÷��̾� ǥ�ø� ���� ���� ���� �÷��̾� ������ �޾ƿ´�
    public Dictionary<int, System.Collections.Hashtable> RenderPlayersUI()
    {
        s_players = PhotonNetwork.CurrentRoom.Players;

        // Player�� ActorNumber�� ������ ���� HashTable 
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
                // hash�� ����ִ� ��� �ӽ÷� �⺻ �� ����
                newHash.Add(NicknameKey, $"USER_{Random.Range(100, 999)}");
                newHash.Add(CharacterIdKey, 0);

                playersInfo.Add(p.Key, newHash);
            }
        }
        return playersInfo;
    }


    // ���� ���� �� �ڵ带 �޾ƿ´�
    public string GetRoomCode()
    {
        string roomCode = PhotonNetwork.CurrentRoom.Name;
        return roomCode;
    }

    // ���� �濡���� actorNumber�� �޾ƿ´�
    public int GetActorNumber()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber;
    }

}
