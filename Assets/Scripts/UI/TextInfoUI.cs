using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  UIManager.Instance.Get(UIType.type)...;
/// </summary>
public class TextInfoUI : UIBase
{
    // 오브젝트                      // 씬 상의 오브젝트 이름
    TextMeshProUGUI infoTMP; // Info TMP
    Image backgroundImage; // Background Image

    protected override void AllowmentComponent()
    {
        infoTMP = GetComponentInChildren<TextMeshProUGUI>();
        backgroundImage = GetComponent<Image>();
    }

    public override void ShowUI()
    {
        base.ShowUI();
    }

    public override void HideUI()
    {
        base.HideUI();
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
}