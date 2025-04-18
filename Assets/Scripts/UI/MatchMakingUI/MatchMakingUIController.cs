using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMakingUIController : MonoBehaviour
{
    [SerializeField] MatchMakingController network;
    [SerializeField] RoomUI roomView;
    [SerializeField] ClientInfo profileModel;

    const string MakeRoomNameFailed = "Failed"; // 방 이름 생성에 실패한 경우

    static List<string> sRoomNameList = new List<string>(); // 방 목록을 저장할 변수

    private void Awake()
    {
        network = GetComponent<MatchMakingController>();
        roomView = GetComponent<RoomUI>();
        profileModel = GetComponent<ClientInfo>();
    }

    private void Start()
    {
        // room view 이벤트 등록
        roomView.randomBtn.onClick.AddListener(OnClickRandomBtn);

        // create view 이벤트 등록
        roomView.c_confirmBtn.onClick.AddListener(OnClickCreateConfirmBtn);

        // join view 이벤트 등록
        roomView.j_confirmBtn.onClick.AddListener(OnClickJoinConfirmBtn);
    }

    void OnClickRandomBtn()
    {
        ServerInfo.InitServerInfo();

        // 이미 room 생성 관련 작업을 처리 중이라면 중복 요청되지 않도록 한다 
        if (ClientInfo.sClientState == ConnectState.Room) return;

        // 랜덤 매치 버튼 클릭 시 
        SaveProfileInfo();

        string roomName = MakeRoomName();
        if (roomName != MakeRoomNameFailed)
        {
            // 방 이름 생성에 실패하지 않은 경우
            network.JoinRandomRoom(roomName);
        }
        else
        {
            // 방 이름 생성에 실패한 경우 
            NetworkHandler.Instance.SetRandomMatchExceptionPanel(NetworkHandler.MakeNameFailed);
        }
    }

    void OnClickCreateConfirmBtn()
    {
        ServerInfo.InitServerInfo();

        // 이미 room 생성 관련 작업을 처리 중이라면 중복 요청되지 않도록 한다 
        if (ClientInfo.sClientState == ConnectState.Room) return;

        // 방 생성 확인 버튼 클릭 시 
        SaveProfileInfo();

        string roomName = MakeRoomName();
        if (roomName != MakeRoomNameFailed)
        {
            network.CreateRoom(roomName);
        }
        else
        {
            // 방 이름 생성에 실패한 경우 
            NetworkHandler.Instance.SetCreateExceptionPanel(NetworkHandler.MakeNameFailed);
        }
    }


    void OnClickJoinConfirmBtn()
    {
        ServerInfo.InitServerInfo();

        string name = roomView.roomCodeTMPInp.text;
        // 참여 코드가 공백이 아니어야 한다
        if (!string.IsNullOrWhiteSpace(name))
        {
            // 방 참가하기 버튼 클릭 시 
            SaveProfileInfo();

            network.JoinRoom(name);
        }
    }

    public void SaveProfileInfo()
    {
        // 룸에 접속 시 클라이언트의 정보를 전송한다
        string nickname = profileModel.Nickname;
        int characterId = profileModel.CharacterId;

        network.SendClientInfo(nickname, characterId);
    }

    public void GetRoomNameList(List<string> roomNames)
    {
        sRoomNameList = roomNames;
    }

    // 중복된 Room Name인지 확인한다
    bool IsDuplicateRoomName(string myRoom)
    {
        foreach (string name in sRoomNameList)
        {
            if (myRoom == name)
            {
                return true;
            }
        }
        return false;
    }

    // 랜덤으로 Room Name을 생성한다
    string MakeRoomName()
    {
        int maxTry = 10;

        string roomName = $"{Random.Range(10000, 99999)}";

        while (IsDuplicateRoomName(roomName))
        {
            roomName = $"{Random.Range(10000, 99999)}";
            maxTry--;

            if (maxTry < 0)
            {
                roomName = MakeRoomNameFailed;
                break;
            }
        }

        return roomName;
    }



}
