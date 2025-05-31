using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float clickScale = 0.9f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] public bool disabledBtn {  get; set; }

    private Vector3 originalScale;
    private Tweener currentTween;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        disabledBtn = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateScale(originalScale * hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateScale(originalScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AnimateScale(originalScale * clickScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 손 떼면 hover 상태로 돌아가야 함
        AnimateScale(originalScale * hoverScale);
    }

    private void AnimateScale(Vector3 targetScale)
    {
        if (disabledBtn) { return; }

        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        currentTween = transform.DOScale(targetScale, duration).SetEase(Ease.OutBack);
    }
}
