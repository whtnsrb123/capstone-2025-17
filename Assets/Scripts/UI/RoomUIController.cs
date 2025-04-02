using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;

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

        // create view 이벤트 등록
        roomView.c_confirmBtn.onClick.AddListener(OnClickCreateConfirmBtn);
        
        // join view 이벤트 등록
        roomView.j_confirmBtn.onClick.AddListener(OnClickJoinConfirmBtn);

        // room panel 이벤트 등록
        roomView.readyOrStartBtn.onClick.AddListener(OnClickReadyBtn);
        roomView.leaveBtn.onClick.AddListener(OnClickLeaveBtn);


        // NetworkManager 이벤트 등록 
        NetworkManager.OnRoomListUpdated += GetRoomNameList;
        NetworkManager.OnRoomPlayerInOut += UpdatePlayerSeats;
        NetworkManager.OnRoomPropsUpdated += UpdatePlayersUI;
        NetworkManager.OnRoomPropsUpdated += ActivateStartButton;
        NetworkManager.OnRoomEntered += OnEnteredRoom;

    }

    private void OnDestroy()
    {
        // NetworkManager 이벤트 해제
        NetworkManager.OnRoomListUpdated -= GetRoomNameList;
        NetworkManager.OnRoomPlayerInOut -= UpdatePlayerSeats;
        NetworkManager.OnRoomPropsUpdated -= UpdatePlayersUI;
        NetworkManager.OnRoomPropsUpdated -= ActivateStartButton;
        NetworkManager.OnRoomEntered -= OnEnteredRoom;
    }

    // =================== Lobby Buttons =====================
    void OnClickRandomBtn()
    {
        // 이미 room 생성 관련 작업을 처리 중이라면 중복 요청되지 않도록 한다 
        if (NetworkManager.sClientState == ConnectState.Room) return;

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
        // 이미 room 생성 관련 작업을 처리 중이라면 중복 요청되지 않도록 한다 
        if (NetworkManager.sClientState == ConnectState.Room) return;

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
        
        bool isMasterClient = roomManager.IsMasterClient();

        if (isMasterClient)
        {
            Debug.Log("나는 마스터");
            // MasterClient일 때, OnPlayerEntered()가 호출되지 않으므로 나의 ActorNumber를 스스로 전송한다 
            roomManager.UpdateEnteredPlayerSeats(roomManager.GetActorNumber());
            roomManager.ChangeReadyState();
            // 시작하기 버튼을 비활성화 한다 
            roomView.readyOrStartBtn.GetComponentInChildren<TMP_Text>().text = "Start Game zzzz";
            roomView.readyOrStartBtn.enabled = false;
        }
        else
        {
            roomView.readyOrStartBtn.enabled = true;
            roomView.readyOrStartBtn.GetComponentInChildren<TMP_Text>().text = "Ready";
        }
        
        NetworkManager.sClientState = ConnectState.Room;
    }

    void OnClickLeaveBtn()
    {
        // 룸 나가기
        roomManager.LeaveRoom();
        // ToDo : room View 초기화하기 
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

    int[] GetUpdatedPlayerSeats()
    {
        int[] seats = roomManager.GetPlayerSeats();
        return seats;
    }

    void OnClickReadyBtn()
    {
        if (roomManager.IsMasterClient()) return;
        roomManager.ChangeReadyState();
    }

    void ActivateStartButton()
    {
        // 필요 인원 충족 시 start button 활성화 
        if (roomManager.IsMasterClient())
        {
            bool[] states = roomManager.GetPlayerReadyStates();

            for (int i = 0; i < states.Length; i++)
            {
                if (!states[i])
                {
                    roomView.readyOrStartBtn.enabled = false;
                    roomView.readyOrStartBtn.GetComponentInChildren<TMP_Text>().text = "not yet";
                    return;
                }
            }
            roomView.readyOrStartBtn.GetComponentInChildren<TMP_Text>().text = "go go";
            roomView.readyOrStartBtn.enabled = true;
        }
    }

    public void UpdatePlayersUI()
    {
        // room view에 플레이어의 ActorNumber 전달
        roomView.GetPlayerSeats(GetUpdatedPlayerSeats());

        // room view에 플레이어의 Ready State 전달
        roomView.GetPlayerReadyStates(roomManager.GetPlayerReadyStates());

        // room view에 플레이어의 정보 전달
        roomView.UpdatePlayerUI(roomManager.GetPlayerInRoomInfos());
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
