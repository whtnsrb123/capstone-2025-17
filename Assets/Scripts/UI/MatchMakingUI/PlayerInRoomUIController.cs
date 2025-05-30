using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerInRoomUIController : MonoBehaviour
{
    [SerializeField] GameObject roomPanel;

    [SerializeField] PlayerInRoomController network;
    [SerializeField] RoomUI roomView;

    TMP_Text readyOrStartButtonTMP;

    private void Start()
    {
        ServerInfo.PlayerActorNumbers.OnValueChanged += UpdatePlayers;
        ServerInfo.ReadyStates.OnValueChanged += UpdateReadyStates;
        ServerInfo.ReadyStates.OnValueChanged += ActivateStartButton;

        // room panel 이벤트 등록
        roomView.readyOrStartBtn.onClick.AddListener(OnClickReadyOrStartBtn);
        roomView.leaveBtn.onClick.AddListener(OnClickLeaveBtn);

        readyOrStartButtonTMP = roomView.readyOrStartBtn.GetComponentInChildren<TMP_Text>();
    }

    private void OnDestroy()
    {
        ServerInfo.PlayerActorNumbers.OnValueChanged -= UpdatePlayers;
        ServerInfo.ReadyStates.OnValueChanged -= UpdateReadyStates;
        ServerInfo.ReadyStates.OnValueChanged -= ActivateStartButton;

        roomView.readyOrStartBtn.onClick.RemoveAllListeners();
        roomView.leaveBtn.onClick.RemoveAllListeners();
    }


    public void OnEnteredRoom()
    {
        // 룸 접속 성공 시 메소드 호출
        roomPanel.SetActive(true);

        string roomCode = network.GetRoomCode();
        roomView.roomCode.text = $"방 코드 : {roomCode}";

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            // MasterClient일 때, OnPlayerEntered()가 호출되지 않으므로 나의 ActorNumber를 스스로 전송한다 
            network.DetectEnteredPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
            network.ChangeReadyState();
            
            if (!GameStateManager.isServerTest)
            {
                readyOrStartButtonTMP.text = "게임 시작";
                roomView.readyOrStartBtn.enabled = true;
            }
            else
            {
                // 시작하기 버튼을 비활성화 한다 
                readyOrStartButtonTMP.text = "대기 중 ...";
                roomView.readyOrStartBtn.enabled = false;
            }
        }
        else
        {
            roomView.readyOrStartBtn.enabled = true;
            readyOrStartButtonTMP.text = "준비하기";
        }

        roomView.InitPanel();

        ClientInfo.sClientState = ConnectState.Room;
    }

    void OnClickLeaveBtn()
    {
        // 룸 나가기
        network.LeaveRoom();
        // ToDo : room View 초기화하기 
    }

    public MissionManager mm;

    void OnClickReadyOrStartBtn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!GameStateManager.isServerTest)
            {
                network.SetGameStart();
                GameStarter.GameStart();
                return;
            }
            // 마스터 클라이언트
            network.ValidPlayerInRoom();
            LoadingPanel.Instance.SetLoadingPanelVisibility(true);
            network.SetGameStart();
            GameStarter.GameStart();
        }
        else
        {
            // 플레이어
            network.ChangeReadyState();
        }
    }

    public void ActivateStartButton(int trash1 = 0, bool trash2 = false)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // 필요 인원 충족 시 start button 활성화 
        for (int i = 0; i < ServerInfo.ReadyStates.Length; i++)
        {
            if ((ServerInfo.PlayerActorNumbers[i] != -1 && !ServerInfo.ReadyStates[i]) && GameStateManager.isServerTest)
            {
                roomView.readyOrStartBtn.enabled = false;
                roomView.readyOrStartBtn.GetComponent<ButtonScaler>().disabledBtn = true;
                readyOrStartButtonTMP.text = "대기 중...";
                return;
            }
        }
        roomView.readyOrStartBtn.enabled = true;
        roomView.readyOrStartBtn.GetComponent<ButtonScaler>().disabledBtn = false;
        readyOrStartButtonTMP.text = "게임 시작";
    }

    public void UpdatePlayers(int index, int value)
    {
        roomView.UpdatePlayer(index, value);
        ActivateStartButton();
    }

    public void UpdateReadyStates(int index, bool ready)
    {
        roomView.UpdateReadyState(index, ready);
    }

    public void UpdateMasterClient(int actorNumber)
    {
        for (int i = 0; i < ServerInfo.PlayerActorNumbers.Length; i++)
        {
            if (ServerInfo.PlayerActorNumbers[i] == actorNumber)
            {
                roomView.masterClientCrown[i].gameObject.SetActive(true);
            }
            else
            {
                roomView.masterClientCrown[i].gameObject.SetActive(false);
            }
        }
    }

}
