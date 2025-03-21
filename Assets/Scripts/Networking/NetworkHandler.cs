using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkHandler : MonoBehaviour
{
    public static NetworkHandler Instance;

    // ===== ���� �г� UI ������Ʈ ���� =====
    GameObject errorPanelPrefab; // ���� �г� ������
    GameObject currentErrorPanel = null; // ���� �����ϴ� �г�
    TextMeshProUGUI errorTypeTMP; // ���� �޽��� ����
    TextMeshProUGUI errorMessageTMP; // ���� �޽��� ����
    Button confirmButton; // Ȯ�� ��ư

    string errorText = "ERROR";

    private void Awake()
    {
        {
            // �׽�Ʈ��~~~~~~~~~~~~~~~~~~~~~~~~~~~
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        errorPanelPrefab = Resources.Load<GameObject>("Prefabs/UI/ErrorPanel");
    }

    // ������ ������ �� ���� ó�� 
   public  void SetDisconnectedExceptionPanel(int code, ConnectState state)
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
            case (int)DisconnectCause.MaxCcuReached:
                errorText = "Server Too Many People.. sorry";
                break;
            default:
                errorText = "Server is BoorAnJeong, Try Later~";
                break;
        }


        if (state == ConnectState.Room || state == ConnectState.InGame)
        {
            // ���� Ȥ�� �ΰ��ӿ��� Disconnected -> Rejoin �õ�
            OnDisconnect = ReconnectAndRejoin;
        }
        else
        {
            // �� �� ��� ������ Disconnected -> StartScene���� ���ư��� ������
            OnDisconnect = BackToStartScene;
        }
        ShowExceptionPanel("====You Can't Create====", errorText, OnDisconnect);
    }

    // �� ���� ��, ���� ó�� 
    public void SetCreateExceptionPanel(int code)
    {
        Action OnCreateFailed = null;
        switch (code)
        {
            // �������� �ʼ��� ���
            case ErrorCode.AuthenticationTicketExpired:
            case ErrorCode.InternalServerError:
                errorText = "You Need to be ReConnected";
                OnCreateFailed = BackToStartScene;
                break;
            default:
                errorText = "You Can't Create Room Now :(";
                break;
        }

        ShowExceptionPanel("====You Can't Create====", errorText, OnCreateFailed);
    }

    // �� ���� ��, ���� ó�� 
    public void SetJoinExceptionPanel(int code)
    {
        Action OnJoinFailed = null;

        switch(code)
        {
            // ���ΰ� ���õ� ���� �ڵ�
            case ErrorCode.GameClosed:
                errorText = "Game Closed : The Room Was Closed :(";
                break;
            case ErrorCode.GameDoesNotExist:
                errorText = "Game Does Not Exist : The Room Name Does Not Exists Now :(";
                break;
            case ErrorCode.GameFull:
                errorText = "Game Full : The Room is Full :(";
                break;
            default:
                errorText = "Join Room Failed :(";
                break;
        }

        ShowExceptionPanel("====You Can't Join====", errorText, OnJoinFailed);
    }

    // �ΰ��ӿ��� ��Ʈ��ũ ���� ó��
    public void SetInGameExceptionPanel(int code)
    {
        
    }

    // �г��� ���� ����
    void ShowExceptionPanel(string type, string message, Action action)
    {
        // �г��� ��� ĵ������ ã�´�
        Canvas canvas = FindObjectOfType<Canvas>();

        // �г��� ���������� �����Ѵ�
        currentErrorPanel = Instantiate(errorPanelPrefab, canvas.transform, false);

        // �г��� �ڽĵ� �� ����� UI ��� ã�� ������ �Ҵ��Ѵ�
        TextMeshProUGUI[] allTMPsChildren = currentErrorPanel.GetComponentsInChildren<TextMeshProUGUI>();
        
        foreach (TextMeshProUGUI tmp in allTMPsChildren)
        {
            if (tmp.name == "ErrorTypeTMP")
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

        // Ȯ�� ��ư�� ã�´�
        Button confirmButton = currentErrorPanel.GetComponentInChildren<Button>();
        // Ȯ�� ��ư Ŭ�� ��, ������ �Լ� �߰�
        confirmButton.onClick.AddListener(() =>  ActivePanelUI.Inactive(currentErrorPanel) );
        
        // ���� �г��� �� ������ �����Ƿ�, ��� �� �ٷ� �����ϱ�
        confirmButton.onClick.AddListener( () => Destroy( currentErrorPanel ) );
        
        if (action != null) { confirmButton.onClick.AddListener(() => action()); }
    }

    // ��Ʈ��ũ �翬���� �ʿ��� ���, ù ȭ�鿡�� ������ �õ� 
    void BackToStartScene()
    {
        PhotonNetwork.LoadLevel("StartScene");
    }

    void ReconnectAndRejoin()
    {
        PhotonNetwork.ReconnectAndRejoin();
    }


}
