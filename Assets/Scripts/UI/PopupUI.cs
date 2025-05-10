using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
///  UIManager.Instance.Get(UIType.type)...;
/// </summary>

public class PopupUI : UIBase
{
    // 오브젝트
    public Button confirmButton; // Confirm Button
    public Button cancelButton; // Cancel Button

    public override void ShowUI()
    {
        base.ShowUI();
        UIManager.Instance.activatedPopups.Push(this);
    }

    public override void HideUI()
    {
        base.HideUI();

        UIManager.Instance.activatedPopups.TryPop(out _);
    }

    public override void ShowAndHideUI(float waitTime)
    {
        base.ShowAndHideUI(waitTime);
    }

    public override void Move(Vector2 direction, bool ease)
    {
        // TODO : 패널 움직이는 기능 구현 
        base.Move(direction, ease);
    }

    public void SetDynamicPopupEvent(UnityAction onConfirm, UnityAction onCancel)
    {
        // 확인 버튼
        if (confirmButton != null && onConfirm != null)
        {
            confirmButton?.onClick.AddListener(onConfirm);
        }

        // 취소 버튼 
        if (cancelButton != null && onCancel != null)
        {

            cancelButton?.onClick.AddListener(onCancel);
        }
    }


}