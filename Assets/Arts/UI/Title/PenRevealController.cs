using UnityEngine;

public class PenRevealController : MonoBehaviour
{
    public Transform pen; // 움직이는 펜 오브젝트
    public Material penDrawMat;
    public Material revealMat;
    public RenderTexture maskTexture;

    public RectTransform titleImageRect; // Title UI Image의 RectTransform
    public Canvas canvas; // UI Canvas
    public Camera uiCamera; // UI를 렌더하는 카메라

    [Range(0.01f, 0.5f)]
    public float fadeAmount = 0.05f;

    void Start()
    {
        Graphics.SetRenderTarget(maskTexture);
        GL.Clear(false, true, Color.black);
        revealMat.SetTexture("_MaskTex", maskTexture);
    }

    void Update()
    {
        // 1. 펜의 화면 좌표 (픽셀 기준)
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, pen.position);

        // 2. Title 이미지의 화면 영역 (좌표, 크기)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            titleImageRect, screenPos, uiCamera, out Vector2 localPos
        );

        Vector2 size = titleImageRect.rect.size;
        Vector2 pivot = titleImageRect.pivot;

        // 3. pivot 기준으로 localPos → (0~1) UV 변환
        Vector2 uv = new Vector2(
            (localPos.x + size.x * pivot.x) / size.x,
            (localPos.y + size.y * pivot.y) / size.y
        );

        // 4. UV 좌표 전달
        penDrawMat.SetVector("_PenUV", uv);
        //penDrawMat.SetFloat("_FadeAmount", fadeAmount);
        //Graphics.Blit(null, maskTexture, penDrawMat);

        float fadeThisFrame = fadeAmount * Time.deltaTime * 60;
        penDrawMat.SetFloat("_FadeAmount", fadeThisFrame);
        Graphics.Blit(null, maskTexture, penDrawMat);
    }
}