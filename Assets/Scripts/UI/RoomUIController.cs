using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
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
        roomView.randomBtn.onClick.AddListener(() => OnClickRandomBtn());
        roomView.leaveBtn.onClick.AddListener(() => OnClickLeaveBtn());

        // create view 이벤트 등록
        roomView.c_confirmBtn.onClick.AddListener(() => OnClickCreateConfirmBtn());
        
        // join view 이벤트 등록
        roomView.j_confirmBtn.onClick.AddListener(() => OnClickJoinConfirmBtn());

        // NetworkManager 이벤트 등록 
        NetworkManager.OnRoomPlayerEntered += RenderPlayers;
        NetworkManager.OnRoomPlayerLeaved += RemoveRenderedPlayers;
        NetworkManager.OnRoomEntered += OnEnteredRoom;
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
        Debug.Log(roomCode);
        int _maxPlayer = (int)roomView.maxPlayerCount.value;

        roomModel.RoomType = ServerInfo.RoomTypes.Create;

        roomManager.CreateRoom(roomCode, _maxPlayer);
    }


    void OnClickJoinConfirmBtn()
    {
        // 방 참가하기 버튼 클릭 시 
        SaveProfileInfo();

        roomModel.RoomType = ServerInfo.RoomTypes.Join;
        string code = roomView.roomCodeTMPInp.text;

        roomManager.JoinRoom(code);
    }

    // ================== In Room ===========================

    void OnEnteredRoom()
    {
        // 룸 접속 성공 시 메소드 호출
        roomPanel.SetActive(true);

        string roomCode = roomManager.GetRoomCode();
        roomView.roomCode.text = $"Room Code : {roomCode}";
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

    Dictionary<int, Hashtable> playersInfo = new Dictionary<int, System.Collections.Hashtable>();

    public void RenderPlayers()
    {
        // RoomManager에게 플레이어 리스트를 요청해서 받아온 뒤,
        // RoomUI 에게 전달하여 UI를 업데이트 시킨다
        playersInfo = roomManager.RenderPlayers();

        roomView.RenderPlayerUI(playersInfo);
    }

    public void RemoveRenderedPlayers(int actorNumber)
    {
        // 방을 나간 플레이어 UI에서 삭제하기 
        roomView.RemovePlayerUI(actorNumber);
    }

}
