#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;


public class ExitPanelUI : MonoBehaviour
{
    public Button confirmButton;
    public Button cancelButton;

    private PopupUI popup;

    private void Start()
    {
        popup = GetComponent<PopupUI>();       
    }

    public void OnClickConfirmButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif

    }

    public void OnClickExitButton()
    {
        popup.ShowUI();
    }

    public void OnClickCancelButton()
    {
        popup.HideUI();
    }


}
