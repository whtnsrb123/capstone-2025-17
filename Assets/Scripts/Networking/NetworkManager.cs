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
    public static Action OnConnectedToServer; // ������ ������ �������� ��
    public static Action OnRoomEntered; // �뿡 �������� ��
    public static Action OnRequestFailed; // ��Ʈ��ũ ��û�� �������� ��
    public static Action OnRoomSeatsUpdated; // Seats ������ ���ŵ� �� 
    public static Action<int, bool> OnRoomPlayerUpdated; // �� �÷��̾� ����Ʈ�� �������� ��

    // �÷��̾��� ���� ����
    static ConnectState sConnectState;

    const string _gameVersion = "1";
    const bool PlayerEntered = true;


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
            // �� ����ȭ
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = _gameVersion;

            // ���� �õ�
            PhotonNetwork.ConnectUsingSettings();
        }
    }
   

    #region Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("NetworkManager.cs - On Connected To Master()");

        // ������ ���� ���� ���� ��, �ٷ� �κ� ���� �õ�
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("NetworkManager.cs - On Disconnected()");

        // ���� ���� ��, ������ �õ� 
        int retry = 0,  maxRetry = 5;
        if (retry < maxRetry)
        {
            retry++;

            Debug.Log("NetworkManager.cs - retry connect to master server");

            Invoke(nameof(ConnectToMasterServer), 2f);
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("On Joined Lobby()");

        sConnectState = ConnectState.Lobby;

        // StartSceneUI.cs ���� ��ϵ� �̺�Ʈ
        OnConnectedToServer?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        // �濡 ���������� ���� ��, �÷��̾� ������ ������ �����ϱ�
        if (PhotonNetwork.IsConnected)
        {
            sConnectState = ConnectState.Room;

            // �̺�Ʈ�� �����Ѵ� 
            OnRoomEntered?.Invoke();
        }
        else
        {
            // ���� ���� ó��
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("On Left Room()");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // �� ���� ���� ó��
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log(returnCode);
        Debug.Log(message);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("NetworkManager.cs - On Created Room()");
        Debug.Log($"NetworkManager.cs - {PhotonNetwork.CurrentRoom.Name}");
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // �ٸ� �÷��̾ �濡 ������ ���
        OnRoomPlayerUpdated?.Invoke(newPlayer.ActorNumber, PlayerEntered);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // �ٸ� �÷��̾ ���� ���� ��� 
        OnRoomPlayerUpdated?.Invoke(otherPlayer.ActorNumber, !PlayerEntered);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        OnRoomSeatsUpdated?.Invoke();

        Debug.Log("OnRoomPropertiesUpdate");
    }


    #endregion

}
