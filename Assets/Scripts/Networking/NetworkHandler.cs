using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkHandler : MonoBehaviourPunCallbacks
{
    // ===== 에러 패널 UI 오브젝트 변수 =====
    // 잘 사용되지 않으므로 동적으로 할당 및 생성
    GameObject errorPanelPrefab; // 에러 패널 프리팹
    GameObject currentErrorPanel = null; // 씬에 존재하는 패널
    TextMeshProUGUI errorTypeTMP; // 에러 메시지 유형
    TextMeshProUGUI errorMessageTMP; // 에러 메시지 내용
    Button confirmButton; // 확인 버튼

    string errorText = "ERROR";

    #region 사용자정의 에러 코드
    public const int RequestNotSent = 0; // 접속이 끊겨, 요청이 전송되지 않은 경우
    public const int MakeNameFailed = 1; // 방 이름을 생성하지 못한 경우
    #endregion

    public static NetworkHandler Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        errorPanelPrefab = Resources.Load<GameObject>("Prefabs/UI/ErrorPanel");
    }


    // 접속이 끊겼을 때 예외 처리 
    public  void SetDisconnectedExceptionPanel(int code)
    {
        Action OnDisconnect = null;
        switch (code)
        {
            case (int) DisconnectCause.DisconnectByOperationLimit:
                errorText = "Too many Operation Call... No More Please";
                break;
            case (int) DisconnectCause.ServerTimeout:
            case (int) DisconnectCause.ClientTimeout:
                errorText = "Too long response time hurry up next time~";
                break;
            case (int) DisconnectCause.CustomAuthenticationFailed:
                errorText = "You InJeung failed";
                break;
            case (int) DisconnectCause.MaxCcuReached:
                errorText = "Server Too Many People.. sorry";
                break;
            default:
                errorText = "Server is BoorAnJeong, Try Later~";
                break;
        }


        if (NetworkManager.sClientState == ConnectState.Room)
        {
            // 대기방 혹은 인게임에서 Disconnected -> Rejoin 시도
            OnDisconnect = ReconnectAndRejoin;
        }
        else
        {
            // 그 외 모든 곳에서 Disconnected -> StartScene으로 돌아가서 재접속
            OnDisconnect = BackToStartScene;
        }
        ShowExceptionPanel("====Disconnected====", errorText, OnDisconnect);
    }

    // 랜덤 매치 시, 예외 처리
    public void SetRandomMatchExceptionPanel(int code)
    {
        Action OnRandomMathFailed = null;

        switch (code)
        {
            // 재접속이 필수인 경우
            case MakeNameFailed:
                errorText = "I dont know reason bur failed random match";
                break;
            case RequestNotSent:
                errorText = "Disconnect, You failed";
                break;
            default:
                errorText = "You Can't random match Now :(";
                break;
        }

        ShowExceptionPanel("No Random Now", errorText, OnRandomMathFailed);
    }


    // 방 생성 시, 예외 처리 
    public void SetCreateExceptionPanel(int code)
    {
        Action OnCreateFailed = null;
        switch (code)
        {
            // 재접속이 필수인 경우
            case ErrorCode.AuthenticationTicketExpired:
            case ErrorCode.InternalServerError:
                errorText = "You Need to be ReConnected";
                OnCreateFailed = BackToStartScene;
                break;
            case RequestNotSent:
                errorText = "Disconnect, You failed";
                break;
            case MakeNameFailed:
                errorText = "I dont know reason bur failed random match";
                break;
            default:
                errorText = "You Can't Create Room Now :(";
                break;
        }

        ShowExceptionPanel("====You Can't Create====", errorText, OnCreateFailed);
    }

    // 방 조인 시, 예외 처리 
    public void SetJoinExceptionPanel(int code)
    {
        //  예외를 중복으로 처리하지 않도록 return  
        if (NetworkManager.sClientState != ConnectState.Lobby) return;

        Action OnJoinFailed = null;

        switch(code)
        {
            // 조인과 관련된 에러 코드
            case ErrorCode.GameClosed:
                errorText = "Game Closed : The Room Was Closed :(";
                break;
            case ErrorCode.GameDoesNotExist:
                errorText = "Game Does Not Exist : The Room Name Does Not Exists Now :(";
                break;
            case ErrorCode.GameFull:
                errorText = "Game Full : The Room is Full :(";
                break;
            case RequestNotSent:
                errorText = "Disconnect, You failed";
                break;
            default:
                errorText = "Join Room Failed :(";
                break;
        }

        ShowExceptionPanel("====You Can't Join====", errorText, OnJoinFailed);
    }

    // 인게임에서 네트워크 예외 처리
    public void SetInGameExceptionPanel(int code)
    {

    }

    // 패널을 씬에 띄운다
    void ShowExceptionPanel(string type, string message, Action action)
    {
        // 패널을 띄울 캔버스를 찾는다
        Canvas canvas = FindObjectOfType<Canvas>();

        // 패널을 프리팹으로 생성한다
        currentErrorPanel = Instantiate(errorPanelPrefab, canvas.transform, false);
        Debug.Assert(canvas != null, "핸들러 프리팹을 찾지 못함");

        // 패널의 자식들 중 사용할 UI 요소 찾아 변수에 할당한다
        TextMeshProUGUI[] allTMPsChildren = currentErrorPanel.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (TextMeshProUGUI tmp in allTMPsChildren)
        {
            if (tmp.name == "ErrorTitleTMP")
            {
                errorTypeTMP = tmp;
                errorTypeTMP.text = type;
            }
            else if (tmp.name == "ErrorMessageTMP")
            {
                errorMessageTMP = tmp;
                errorMessageTMP.text = message;
            }
        }

        // 확인 버튼을 찾는다
        Button confirmButton = currentErrorPanel.GetComponentInChildren<Button>();
        // 확인 버튼 클릭 시, 실행할 함수 추가
        confirmButton.onClick.AddListener(() =>  ActivePanelUI.Inactive(currentErrorPanel) );

        // 예외 패널을 잘 사용되지 않으므로, 사용 후 바로 삭제하기
        confirmButton.onClick.AddListener( () => Destroy( currentErrorPanel ) );

        // case에 따라 추가 로직이 필요한 경우 등록 
        if (action != null) { confirmButton.onClick.AddListener(() => action()); }
    }

    // 네트워크 재연결이 필요한 경우, 첫 화면에서 재접속 시도 
    void BackToStartScene()
    {
        PhotonNetwork.LoadLevel("StartScene");
    }

    void ReconnectAndRejoin()
    {
        // Reconnect And Rejoin 시도 
        if (!PhotonNetwork.ReconnectAndRejoin())
        {
            // 돌아갈 Room이 없어진 경우
            BackToStartScene();
            Debug.Log("돌아갈 룸 없음");
        }
        else
        {
            Debug.Log("돌아갈 룸 있음");
            NetworkManager.sCurrentState = ConnectState.Room;
            NetworkManager.sClientState = ConnectState.Room;
        }
    }


    #region 서버 예외 처리 콜백 함수들

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);

        // 랜덤 매치 예외 처리
        SetJoinExceptionPanel(returnCode);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);

        // 방 생성 예외 처리
        SetCreateExceptionPanel(returnCode);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        // 조인 예외 처리
        if (NetworkManager.sClientState == ConnectState.Room)
        {
            // ReconnectAndRejoin()이 실패한 경우에 호출된 OnJOinRoomFailed()를 처리한다

            // sClientState가 Room이면 다시 Room 재참여를 시도하므로, Lobby로 수정한다 
            NetworkManager.sClientState = ConnectState.Lobby; 
            SetDisconnectedExceptionPanel(0);
        }
        else
        {
            // 일반적인 Join의 실패를 처리한다
            SetJoinExceptionPanel(returnCode);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("On Disconnected");
        NetworkManager.sCurrentState = ConnectState.Disconnected;

        // Disconnected 예외 처리
        SetDisconnectedExceptionPanel((int)cause);
    }
    #endregion


}
