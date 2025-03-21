using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    [SerializeField]
    GameObject fadePanel;

    CanvasGroup group;

    public static Action<bool> Fade;

    const float FadeInAlpha = 0.01f;
    const float FadeOutAlpha = 0.99f;

    private void Awake()
    {
        group = fadePanel.GetComponent<CanvasGroup>();
        Fade += FadeInOut;
    }

    void OnDestroy()
    {
        // static ������ ����� �޼ҵ��� ��ü�� Destroy �� ��� �ݵ�� ���� 
        Fade -= FadeInOut;
    }

    void FadeInOut(bool isFadeIn)
    {

        if (isFadeIn)
        {
            StartCoroutine(nameof(FadeIn));
        }
        else
        {
            StartCoroutine(nameof(FadeOut));
        }
    }
    public IEnumerator FadeIn()
    {
        Debug.Log(group.alpha);
        while (group.alpha > FadeInAlpha)
        {
            float now = group.alpha;
            now -= 0.01f;
            group.alpha = now;

            yield return null;
        }

        fadePanel.SetActive(false);
    }

    public IEnumerator FadeOut()
    {
        fadePanel.SetActive(true);

        while (group.alpha < FadeOutAlpha)
        {
            float now = group.alpha;
            now += 0.01f;
            group.alpha = now;

            yield return null;
        }
    }

}
