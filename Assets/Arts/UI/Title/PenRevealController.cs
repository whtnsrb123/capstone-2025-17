using UnityEngine;

public class PenRevealController : MonoBehaviour
{
    public Transform pen; // �����̴� �� ������Ʈ
    public Material penDrawMat;
    public Material revealMat;
    public RenderTexture maskTexture;

    public RectTransform titleImageRect; // Title UI Image�� RectTransform
    public Canvas canvas; // UI Canvas
    public Camera uiCamera; // UI�� �����ϴ� ī�޶�

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
        // 1. ���� ȭ�� ��ǥ (�ȼ� ����)
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, pen.position);

        // 2. Title �̹����� ȭ�� ���� (��ǥ, ũ��)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            titleImageRect, screenPos, uiCamera, out Vector2 localPos
        );

        Vector2 size = titleImageRect.rect.size;
        Vector2 pivot = titleImageRect.pivot;

        // 3. pivot �������� localPos �� (0~1) UV ��ȯ
        Vector2 uv = new Vector2(
            (localPos.x + size.x * pivot.x) / size.x,
            (localPos.y + size.y * pivot.y) / size.y
        );

        // 4. UV ��ǥ ����
        penDrawMat.SetVector("_PenUV", uv);
        //penDrawMat.SetFloat("_FadeAmount", fadeAmount);
        //Graphics.Blit(null, maskTexture, penDrawMat);

        float fadeThisFrame = fadeAmount * Time.deltaTime * 60;
        penDrawMat.SetFloat("_FadeAmount", fadeThisFrame);
        Graphics.Blit(null, maskTexture, penDrawMat);
    }
}