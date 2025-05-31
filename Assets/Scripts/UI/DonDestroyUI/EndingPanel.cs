using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingPanel : MonoBehaviourPunCallbacks
{
    PopupUI popup;

    private void Start()
    {
        popup = GetComponent<PopupUI>();
    }

    public void OnClickOkBtn()
    {
        Managers.GameStateManager.RPC_LeaveRoomAllPlayer();
        popup.HideUI();
    }

}
