using System.Collections.Generic;
using UnityEngine;
using System.Collections;

// Controller
public class RoomUIController : MonoBehaviour
{ 
    [SerializeField]
    RoomManager roomManager;

    [SerializeField]
    GameObject roomPanel;

    RoomUI roomView;
    ServerInfo roomModel;
    ClientInfo profileModel;

    static List<string> sRoomNameList = new List<string>(); // 방 목록을 저장할 변수
    const string MakeRoomNameFailed = "Failed"; // 방 이름 생성에 실패한 경우

    void Awake()
    {
        roomView = GetComponent<RoomUI>();
        roomModel = GetComponent<ServerInfo>();
        profileModel = GetComponent<ClientInfo>();
    }

    private void Start()
    {
        // room view 이벤트 등록
        roomView.randomBtn.onClick.AddListener(OnClickRandomBtn);
        roomView.leaveBtn.onClick.AddListener(OnClickLeaveBtn);

        // create view 이벤트 등록
        roomView.c_confirmBtn.onClick.AddListener(OnClickCreateConfirmBtn);
        
        // join view 이벤트 등록
        roomView.j_confirmBtn.onClick.AddListener(OnClickJoinConfirmBtn);

        // NetworkManager 이벤트 등록 
        NetworkManager.OnRoomListUpdated += GetRoomNameList;
        NetworkManager.OnRoomPlayerUpdated += UpdatePlayerSeats;
        NetworkManager.OnRoomSeatsUpdated += UpdatePlayersUI;
        NetworkManager.OnRoomEntered += OnEnteredRoom;

    }

    private void OnDestroy()
    {
        // NetworkManager 이벤트 해제
        NetworkManager.OnRoomPlayerUpdated -= UpdatePlayerSeats;
        NetworkManager.OnRoomSeatsUpdated -= UpdatePlayersUI;
        NetworkManager.OnRoomEntered -= OnEnteredRoom;
    }

    // =================== Lobby Buttons =====================
    void OnClickRandomBtn()
    {
        // 랜덤 매치 버튼 클릭 시 
        SaveProfileInfo();

        string roomName = MakeRoomName();
        if (roomName != MakeRoomNameFailed)
        {
            // 방 이름 생성에 실패하지 않은 경우
            roomModel.RoomType = ServerInfo.RoomTypes.Random;
            roomManager.JoinRandomRoom(roomName);
        }
        else
        {
            // 방 이름 생성에 실패한 경우 
            NetworkHandler.Instance.SetRandomMatchExceptionPanel(NetworkHandler.MakeNameFailed);
        }
    }

    void OnClickCreateConfirmBtn()
    {
        // 방 생성 확인 버튼 클릭 시 
        SaveProfileInfo();

        string roomName = MakeRoomName();
        if (roomName != MakeRoomNameFailed)
        {
            roomModel.RoomType = ServerInfo.RoomTypes.Create;
            roomManager.CreateRoom(roomName);
        }
        else
        {
            // 방 이름 생성에 실패한 경우 
            NetworkHandler.Instance.SetCreateExceptionPanel(NetworkHandler.MakeNameFailed);
        }
    }


    void OnClickJoinConfirmBtn()
    {
        string name = roomView.roomCodeTMPInp.text;
        // 참여 코드가 공백이 아니어야 한다
        if (!string.IsNullOrWhiteSpace(name))
        {
            // 방 참가하기 버튼 클릭 시 
            SaveProfileInfo();

            roomModel.RoomType = ServerInfo.RoomTypes.Join;
            roomManager.JoinRoom(name);
        }
    }

    // ========================= In Room ===========================

    void OnEnteredRoom()
    {
        // 룸 접속 성공 시 메소드 호출
        roomPanel.SetActive(true);

        string roomCode = roomManager.GetRoomCode();
        roomView.roomCode.text = $"Room Code : {roomCode}";

        // MasterClient일 때, 나의 ActorNumber를 스스로 전송한다 
        roomManager.UpdateEnteredPlayerSeats(roomManager.GetActorNumber());
    }

    void OnClickLeaveBtn()
    {
        // 룸 나가기
        roomManager.LeaveRoom();
    }

    public void SaveProfileInfo()
    {
        // 룸에 접속 시 클라이언트의 정보를 전송한다
        string nickname = profileModel.Nickname;
        int characterId = profileModel.CharacterId;

        roomManager.SendClientInfo(nickname, characterId);
    }

    void UpdatePlayerSeats(int actorNumber, bool isEntered)
    {
        // RoomMananger는 현재 룸의 CustomProperties의 "Seats" 정보를 업데이트 한다.

        if (isEntered) 
        {
            // 입장한 경우
            roomManager.UpdateEnteredPlayerSeats(actorNumber); // 입장한 플레이어의 actorNumber
        }
        else 
        {
            // 퇴장한 경우
            roomManager.UpdateLeftPlayerSeats(actorNumber); // 나간 플레이어의 actorNumber
        }
            
    }

    void GetUpdatedPlayerSeats()
    {
        int[] seats = roomManager.GetUpdatedPlayerSeats();
        roomView.GetPlayerSeats(seats);
    }

    public void UpdatePlayersUI()
    {
        GetUpdatedPlayerSeats();

        Dictionary<int, Hashtable> playersInfo = roomManager.RenderPlayersUI();

        roomView.UpdatePlayerUI(playersInfo);

    }

    // ====================== Room Name 생성하기 =========================

    // OnRoomListUpdate() 콜백에서 Room Name 리스트를 받아온다
    void GetRoomNameList(List<string> roomNames)
    {
        sRoomNameList = roomNames;
    }

    // 중복된 Room Name인지 확인한다
    bool IsDuplicateRoomName(string myRoom)
    {
        foreach(string name in sRoomNameList)
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

        string roomName =  $"{Random.Range(10000, 99999)}";

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
