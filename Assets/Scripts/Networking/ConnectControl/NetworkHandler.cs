using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    Transform dontDestroyCanvas; // UI Object가 표시될 전용 캔버스 
    RectTransform dontDestroyRect;


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
                errorText = "요청이 너무 많습니다.\n잠시 뒤 다시 시도해주세요.";
                break;
            case (int) DisconnectCause.ServerTimeout:
            case (int) DisconnectCause.ClientTimeout:
                errorText = "응답 시간을 초과했습니다.";
                break;
            case (int) DisconnectCause.CustomAuthenticationFailed:
                errorText = "인증에 실패했습니다.";
                break;
            case (int) DisconnectCause.MaxCcuReached:
                errorText = "서버 인원이 너무 많습니다.";
                break;
            default:
                errorText = "서버가 불안정합니다.\n 다시 시도해주세요.";
                break;
        }


        if (ClientInfo.sClientState == ConnectState.Room)
        {
            // 대기방 혹은 인게임에서 Disconnected -> Rejoin 시도
            OnDisconnect = ReconnectAndRejoin;
        }
        else
        {
            // 그 외 모든 곳에서 Disconnected -> StartScene으로 돌아가서 재접속
            OnDisconnect = BackToStartScene;
        }
        ShowExceptionPanel("네트워크 접속 끊김", errorText, OnDisconnect);
    }

    // 랜덤 매치 시, 예외 처리
    public void SetRandomMatchExceptionPanel(int code)
    {
        Action OnRandomMathFailed = null;

        switch (code)
        {
            // 재접속이 필수인 경우
            case MakeNameFailed:
                errorText = "알 수 없는 원인으로 매칭이 실패했습니다.";
                break;
            case RequestNotSent:
                errorText = "요청이 전송되지 않았습니다.";
                break;
            default:
                errorText = "랜덤 매치에 실패했습니다.\n 잠시 뒤 다시 시도해주세요.";
                break;
        }

        ShowExceptionPanel("랜덤 매치 실패", errorText, OnRandomMathFailed);
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
                errorText = "연결이 끊겼습니다.\n재접속을 시도해주세요.";
                OnCreateFailed = BackToStartScene;
                break;
            case RequestNotSent:
                errorText = "요청이 전송되지 않았습니다.";
                break;
            case MakeNameFailed:
                errorText = "알 수 없는 원인으로 방 생성에 실패했습니다.";
                break;
            default:
                errorText = "현재는 방 생성이 불가합니다.";
                break;
        }

        ShowExceptionPanel("방 생성 실패", errorText, OnCreateFailed);
    }

    // 방 조인 시, 예외 처리 
    public void SetJoinExceptionPanel(int code)
    {
        //  예외를 중복으로 처리하지 않도록 return  
        if (ClientInfo.sClientState != ConnectState.Lobby) return;

        Action OnJoinFailed = null;

        switch(code)
        {
            // 조인과 관련된 에러 코드
            case ErrorCode.GameClosed:
                errorText = "이미 게임이 시작된 방입니다.";
                break;
            case ErrorCode.GameDoesNotExist:
                errorText = "존재하지 않는 방입니다.";
                break;
            case ErrorCode.GameFull:
                errorText = "인원이 가득 찼습니다.";
                break;
            case RequestNotSent:
                errorText = "요청이 전송되지 않았습니다.";
                break;
            default:
                errorText = "알 수 없는 원인으로 참가가 불가합니다.";
                break;
        }

        ShowExceptionPanel("방 참가 실패", errorText, OnJoinFailed);
    }

    // 인게임에서 네트워크 예외 처리
    public void SetInGameExceptionPanel(int code)
    {

    }

    // 패널을 씬에 띄운다
    void ShowExceptionPanel(string type, string message, Action action)
    {
        // 패널을 띄울 캔버스를 찾는다
        dontDestroyCanvas = GameObject.Find("DontDestroyCanvas").transform;
        Debug.Assert(dontDestroyCanvas != null, "DontDestroyCanvas null");

        // 패널을 프리팹으로 생성한다
        currentErrorPanel = Instantiate(errorPanelPrefab, dontDestroyCanvas.transform, false);
        Debug.Assert(currentErrorPanel != null, "cannot find error panel!");

        dontDestroyRect = currentErrorPanel.GetComponent<RectTransform>();
        Debug.Assert(currentErrorPanel != null, "rect transform is null!");
        dontDestroyRect.SetAsLastSibling();


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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 네트워크 재연결이 필요한 경우, 첫 화면에서 재접속 시도 
    void BackToStartScene()
    {
        SceneManager.LoadScene("StartScene");
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
            ClientInfo.sCurrentState = ConnectState.Room;
            ClientInfo.sClientState = ConnectState.Room;
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
        if (ClientInfo.sClientState == ConnectState.Room)
        {
            // ReconnectAndRejoin()이 실패한 경우에 호출된 OnJOinRoomFailed()를 처리한다

            // sClientState가 Room이면 다시 Room 재참여를 시도하므로, Lobby로 수정한다 
            ClientInfo.sClientState = ConnectState.Lobby; 
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
        ClientInfo.sCurrentState = ConnectState.Disconnected;

        // Disconnected 예외 처리
        SetDisconnectedExceptionPanel((int)cause);
    }
    #endregion


}
