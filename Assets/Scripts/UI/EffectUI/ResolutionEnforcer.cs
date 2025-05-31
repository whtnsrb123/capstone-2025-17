using UnityEngine;
public class ResolutionEnforcer : MonoBehaviour
{
    public int baseWidth = 1920;
    public int baseHeight = 1080;
    public float enforceDelay = 0.5f; // 창 조절 멈춘 뒤 적용할 대기 시간

    private float targetAspect;
    private int prevWidth;
    private int prevHeight;
    private float resizeTimer = 0f;

    private const float epslion = 0.001f;

    void Start()
    {
        targetAspect = (float)baseWidth / baseHeight;
        prevWidth = Screen.width;
        prevHeight = Screen.height;
    }

    void Update()
    {
        if (prevHeight != Screen.height || prevWidth != Screen.width)
        {
            // 창 크기가 변하고 있으면 타이머 리셋
            resizeTimer = enforceDelay;

            prevWidth = Screen.width;
            prevHeight = Screen.height;
        }

        if (resizeTimer > 0f)
        {
            resizeTimer -= Time.unscaledDeltaTime;
            if (resizeTimer <= 0f)
            {
                EnforceAspect();
            }
        }
    }

    void EnforceAspect()
    {
        int width = Screen.width;
        int height = Screen.height;
        float currentAspect = (float)width / height;

        if (Mathf.Abs(currentAspect - targetAspect) > epslion)
        {
            if (currentAspect > targetAspect)
            {
                width = Mathf.RoundToInt(height * targetAspect);
            }
            else
            {
                height = Mathf.RoundToInt(width / targetAspect);
            }

            Screen.SetResolution(width, height, Screen.fullScreen);
        }
    }
}
