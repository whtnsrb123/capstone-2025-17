using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Voice.PUN;

public class InGameOptionUI : MonoBehaviourPunCallbacks
{
    #region 옵션 패널 UI 오브젝트
    [TabGroup("InGame Optioin Buttons")] [SerializeField] Button continueButton;
    [TabGroup("InGame Optioin Buttons")] [SerializeField] Button optionButton;
    [TabGroup("InGame Optioin Buttons")] [SerializeField] Button leaveButton;

    [TabGroup("Popups")] [SerializeField] GameObject OptionPanel;
    [TabGroup("Popups")] [SerializeField] GameObject leavePanel;
    #endregion

    PopupUI popup;
    PopupUI leavePopup;

    private void Start()
    {
        popup = GetComponent<PopupUI>();
        leavePopup = leavePanel.transform.GetComponent<PopupUI>();

        SetEvent();
    }

    void SetEvent()
    {
        // 인게임 옵션 패널 
        continueButton.onClick.AddListener(() => OnClickContinueButton()); 
        optionButton.onClick.AddListener(() => OptionPanel.GetComponent<PopupUI>().ShowUI() );
        leaveButton.onClick.AddListener( ()=> CheckLeaveGame());

        // 나가기 패널 
        leavePopup.confirmButton.onClick.AddListener(OnClickLeaveConfirmButton);
        leavePopup.cancelButton.onClick.AddListener(OnClickLeaveCancelButton);
    }

    void OnClickContinueButton()
    {
        popup.HideUI();
        UIManager.Instance.isEscPanelActive = false;
    }

    void CheckLeaveGame()
    {
        Debug.Log("check leave game");
        leavePopup.ShowUI();
    }

    void OnClickLeaveConfirmButton()
    {
        leavePopup.HideUI();
        popup.HideUI();

        if (PunVoiceClient.Instance.Client != null &&
            PunVoiceClient.Instance.Client.IsConnected)
        {
            PunVoiceClient.Instance.Client.Disconnect();
        }


        PhotonNetwork.LeaveRoom();
    }

    void OnClickLeaveCancelButton()
    {
        leavePopup.HideUI();
    }

    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().name != "LobbyScene")
        {
            Debug.Log("option panel");
            LoadingPanel.Instance?.BeforeLoadedLobbyScene();
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
