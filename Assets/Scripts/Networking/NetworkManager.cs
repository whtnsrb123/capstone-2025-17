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
    const bool PlayerEntered = true; // ������ �÷��̾ ����/�������� �����ϱ� ���� �Ű������� ���� const ����

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
   

    #region ��Ʈ��ũ �۾� ��û �ݹ� �Լ���
    public override void OnConnectedToMaster()
    {
        // ������ ���� ���� ���� ��, �ٷ� �κ� ���� �õ�
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        // �κ� ���� ����
        sConnectState = ConnectState.Lobby;

        // StartSceneUI.cs ���� ��ϵ� �̺�Ʈ ����
        OnConnectedToServer?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        // �뿡 ���� ����
         sConnectState = ConnectState.Room;

         // �̺�Ʈ�� �����Ѵ� 
         OnRoomEntered?.Invoke();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("On Left Room()");
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

    #region ���� ���� ó�� �ݹ� �Լ���

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);

        // ���� ��ġ ���� ó��
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);

        // �� ���� ���� ó��
        NetworkHandler.Instance.SetCreateExceptionPanel(returnCode);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        // ���� ���� ó��
        NetworkHandler.Instance.SetJoinExceptionPanel(returnCode);

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        NetworkHandler.Instance.SetDisconnectedExceptionPanel((int)cause, sConnectState);

    }




    #endregion

}
