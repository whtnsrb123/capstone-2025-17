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

        roomModel.RoomType = ServerInfo.RoomTypes.Random;
        roomManager.RandomRoom();
    }

    void OnClickCreateConfirmBtn()
    {
        // 방 생성 확인 버튼 클릭 시 
        SaveProfileInfo();

        string roomCode =$"{Random.Range(10000, 99999)}";

        roomModel.RoomType = ServerInfo.RoomTypes.Create;

        roomManager.CreateRoom(roomCode);
    }


    void OnClickJoinConfirmBtn()
    {
        // 방 참가하기 버튼 클릭 시 
        SaveProfileInfo();

        roomModel.RoomType = ServerInfo.RoomTypes.Join;
        string code = roomView.roomCodeTMPInp.text;

        roomManager.JoinRoom(code);
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

}
