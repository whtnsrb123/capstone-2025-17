using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum UIType
{
    StaticPopup,
    OptionPopup, // 메인 팝업
    IngameOption,
    LoadingPanel,
    TextInfo, // 텍스트 정보 
    Ending,
}

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIBase : MonoBehaviour
{
    public UIType type;

    public float fadeTime;
    public float delayTime;
    [field: SerializeField] public bool BlockClick { get; private set; }

    protected CanvasGroup group;
    protected RectTransform rectTransform;

    private Coroutine showCoroutine = null;
    private Coroutine showAndHideCoroutine = null;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        group.alpha = 0;
        group.blocksRaycasts = false;

        AllowmentComponent();
    }

    protected virtual void AllowmentComponent() { }

    public virtual void ShowUI()
    {
        if (showCoroutine != null) { UIManager.Instance.StopCoroutine(showCoroutine); }

        rectTransform.SetAsLastSibling();
        showCoroutine = UIManager.Instance.StartCoroutine(ShowUICoroutine(delayTime));
    }

    public IEnumerator ShowUICoroutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        group.blocksRaycasts |= true;
        group.DOFade(1, fadeTime).SetUpdate(true);
    }

    public virtual void ShowAndHideUI(float waitTime)
    {
        if (showAndHideCoroutine != null) { UIManager.Instance.StopCoroutine(showAndHideCoroutine); }

        rectTransform.SetAsLastSibling();
        showAndHideCoroutine = UIManager.Instance.StartCoroutine(ShowAndHideCoroutine(waitTime));
    }

    public IEnumerator ShowAndHideCoroutine(float waitTime)
    {
        group.blocksRaycasts = true;
        group.DOFade(1, fadeTime);

        yield return new WaitForSeconds(waitTime);

        group.DOFade(0, fadeTime);

        group.blocksRaycasts = false;
    }

    public virtual void HideUI()
    {
        rectTransform?.SetAsFirstSibling();

        group.DOFade(0, fadeTime).SetUpdate(true);

        group.blocksRaycasts = false;
    }

    public virtual void Move(Vector2 direction, bool ease)
    {
        var tween = rectTransform.DOAnchorPos(direction, fadeTime);
        if (ease) tween.SetEase(Ease.OutBack, 0.9f);
    }

}