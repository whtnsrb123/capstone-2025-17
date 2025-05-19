using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMatchMaking : MonoBehaviour
{
    [SerializeField] private PopupUI creatPopup;
    [SerializeField] private PopupUI joinPopup;

    public void OnClickCreateRoom()
    {
        creatPopup.ShowUI();
    }

    public void HideCreatePanel()
    {
        creatPopup.HideUI();
    }

    public void OnClickJoinRoom()
    {
        joinPopup.gameObject.GetComponentInChildren<TMP_InputField>().text = string.Empty;
        joinPopup.ShowUI();
    }

    public void HideJoinPanel()
    {
        joinPopup.HideUI();
    }

}
