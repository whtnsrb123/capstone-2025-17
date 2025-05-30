using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PlayerInRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerInRoomUIController ui;

    public void ChangeReadyState()
    {
        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] playerActorNumbers = (int[])hash[ServerInfo.PlayerActorNumbersKey];

        for (int i = 0; i < ServerInfo.RequiredPlayerCount; i++)
        {
            // 내 자리를 발견
            if (playerActorNumbers[i] == PhotonNetwork.LocalPlayer.ActorNumber)
            {

                ((bool[])hash[ServerInfo.ReadyStatesKey])[i] = !((bool[])hash[ServerInfo.ReadyStatesKey])[i];

                ServerInfo.ReadyStates[i] = !ServerInfo.ReadyStates[i];

                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

                break;
            }
        }
    }
    public void ChangeReadyState(int actorNumber, bool isReady)
    {
        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] playerActorNumbers = (int[])hash[ServerInfo.PlayerActorNumbersKey];

        for (int i = 0; i < ServerInfo.RequiredPlayerCount; i++)
        {
            // 내 자리를 발견
            if (playerActorNumbers[i] == actorNumber)
            {

                ((bool[])hash[ServerInfo.ReadyStatesKey])[i] = isReady;

                ServerInfo.ReadyStates[i] = isReady;

                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

                break;
            }
        }
    }

    // 게임 시작 전 발생할 수 있는 오류를 방지한다 
    public void ValidPlayerInRoom()
    {
        Dictionary<int, Player> currentPlayers = PhotonNetwork.CurrentRoom.Players;

        // 현재 실제 접속 중인 actorNumber로 배열 재구성
        int[] validActorNumbers = currentPlayers.Keys.ToArray();

        // Room의 CustomProperties를 수정해서 동기화
        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
        hash[ServerInfo.PlayerActorNumbersKey] = validActorNumbers;

        ServerInfo.PlayerActorNumbers.EventOff = true;

        for (int i = 0; i < validActorNumbers.Length; i++)
        {
            if (ServerInfo.PlayerActorNumbers[i] != validActorNumbers[i])
            {
                ServerInfo.PlayerActorNumbers[i] = validActorNumbers[i];
                Debug.LogWarning($"{i} 번째 플레이어의 ActorNumber 오류 수정 {ServerInfo.PlayerActorNumbers[i]} -> {validActorNumbers[i]}");
            }
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        ServerInfo.PlayerActorNumbers.EventOff = true;

    }


    // MasterClient는 새로운 플레이어 입장 시, 플레이어 표시 순서를 갱신한다
    public void DetectEnteredPlayer(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (CheckGameStart()) return;

        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] playerActorNumbers = (int[])hash[ServerInfo.PlayerActorNumbersKey];

        for (int i = 0; i < playerActorNumbers.Length; i++)
        {
            // 빈 자리를 발견
            if (playerActorNumbers[i] == -1)
            {
                ServerInfo.PlayerActorNumbers[i] = actorNumber;

                playerActorNumbers[i] = actorNumber;
                hash[ServerInfo.PlayerActorNumbersKey] = playerActorNumbers;

                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

                break;
            }
        }
    }

    // MasterClient는 플레이어 퇴장 시, 플레이어 표시 순서를 갱신한다
    public void DetectLeftPlayer(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (CheckGameStart()) return;

        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int[] playerActorNumbers = (int[])hash[ServerInfo.PlayerActorNumbersKey];
        bool[] readyStates = (bool[])hash[ServerInfo.ReadyStatesKey];

        for (int i = 0; i < ServerInfo.RequiredPlayerCount; i++)
        {
            // 떠난 플레이어 발견 
            if (ServerInfo.PlayerActorNumbers[i] == actorNumber)
            {
                ServerInfo.ReadyStates[i] = false;
                ServerInfo.PlayerActorNumbers[i] = -1;
                
                playerActorNumbers[i] = -1;
                readyStates[i] = false;

                hash[ServerInfo.PlayerActorNumbersKey] = playerActorNumbers;
                hash[ServerInfo.ReadyStatesKey] = readyStates;

                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

                break;
            }
        }
    }

    public string GetRoomCode()
    {
        string roomCode = PhotonNetwork.CurrentRoom.Name;
        return roomCode;
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            DetectLeftPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
        }
        PhotonNetwork.LeaveRoom();

        // 클라이언트는 방 퇴장을 요청한 상태
        ClientInfo.sClientState = ConnectState.Lobby;
    }

    public void GetPlayerActorNumbers()
    {
        int[] playerActorNumbers = (int[])PhotonNetwork.CurrentRoom.CustomProperties[ServerInfo.PlayerActorNumbersKey];
        bool[] readyStates = (bool[])PhotonNetwork.CurrentRoom.CustomProperties[ServerInfo.ReadyStatesKey];

        ServerInfo.PlayerActorNumbers.EventOff = true;
        ServerInfo.ReadyStates.EventOff = true;

        for (int i = 0; i < ServerInfo.RequiredPlayerCount; i++)
        {
            ServerInfo.PlayerActorNumbers[i] = playerActorNumbers[i];
            //Debug.Log($"playerActorNumber{i} = {ServerInfo.PlayerActorNumbers[i]}  = {playerActorNumbers[i]}");
            ServerInfo.ReadyStates[i] = readyStates[i];
        }

        ServerInfo.PlayerActorNumbers.EventOff = false;
        ServerInfo.ReadyStates.EventOff = false;
    }

    void DetectRoomPropertiesChange(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        var changedReadyStates = (bool[])propertiesThatChanged[ServerInfo.ReadyStatesKey];
        if (changedReadyStates == null) return; // 방 생성 시, 방 속성이 등록되기 전 한 번 호출되어 null 참조를 하게 된다 

        for (int i = 0; i < ServerInfo.RequiredPlayerCount; i++)
        {
            if (ServerInfo.ReadyStates[i] != changedReadyStates[i])
            {
                ServerInfo.ReadyStates[i] = changedReadyStates[i];
                break;
            }
        }

        var changedPlayerActorNumbers = (int[])propertiesThatChanged[ServerInfo.PlayerActorNumbersKey];

        for (int i = 0; i < ServerInfo.RequiredPlayerCount; i++)
        {
            if (ServerInfo.PlayerActorNumbers[i] != changedPlayerActorNumbers[i])
            {
                ServerInfo.PlayerActorNumbers[i] = changedPlayerActorNumbers[i];
                break;
            }
        }
    }

    public bool CheckGameStart()
    {
        Hashtable props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey(ServerInfo.IsGameStartKey))
        {
           return (bool)props[ServerInfo.IsGameStartKey];
        }
        return false;
    }

    public void SetGameStart()
    {
        Hashtable props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey(ServerInfo.IsGameStartKey))
        {
            Debug.Log("key detected");

            props[ServerInfo.IsGameStartKey] = true;

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    #region Callback Functions

    public override void OnJoinedRoom()
    {
        // 룸에 조인 성공
        ClientInfo.sCurrentState = ConnectState.Room;

        GetPlayerActorNumbers();

        ui.OnEnteredRoom();
        ui.UpdateMasterClient(PhotonNetwork.MasterClient.ActorNumber);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log(message);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        DetectRoomPropertiesChange(propertiesThatChanged);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);    

        DetectEnteredPlayer(newPlayer.ActorNumber);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        DetectLeftPlayer(otherPlayer.ActorNumber);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        ChangeReadyState(newMasterClient.ActorNumber, true);

        ui.ActivateStartButton();
        ui.UpdateMasterClient(newMasterClient.ActorNumber);
    }
    #endregion

}
