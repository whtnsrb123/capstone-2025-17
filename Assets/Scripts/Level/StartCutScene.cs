using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class StartCutScene : MonoBehaviour
{
    public enum SceneState { Camera, Fade }

    [System.Serializable]
    public class Cut
    {
        public SceneState state;
        public float time;
        public GameObject cam; // 해당 컷에서 켜질 카메라나 오브젝트
    }

    public Image fadePanel;
    public UnityEvent[] afterEvents;
    public List<Cut> cuts = new List<Cut>();

    private Cut lastActiveCameraCut = null;
    private GameObject pendingCameraToDestroy = null;

    private void Start()
    {
        PlayCutscene();
    }

    public void PlayCutscene()
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < cuts.Count; i++)
        {
            int currentIndex = i;
            Cut currentCut = cuts[currentIndex];
            Cut nextCut = (currentIndex + 1 < cuts.Count) ? cuts[currentIndex + 1] : null;

            seq.AppendCallback(() =>
            {
                HandleCutStart(currentCut);
            });

            seq.AppendInterval(currentCut.time);

            seq.AppendCallback(() =>
            {
                HandleCutEnd(currentCut, nextCut);
            });
        }

        seq.OnComplete(() => OnComplete());
    }

    private void HandleCutStart(Cut cut)
    {
        switch (cut.state)
        {
            case SceneState.Camera:
                if (cut.cam != null)
                {
                    cut.cam.SetActive(true);
                    lastActiveCameraCut = cut;
                }
                break;

            case SceneState.Fade:
                // Fade 시작 전에 이전 카메라 기억해 두기
                if (lastActiveCameraCut != null && lastActiveCameraCut.cam != null)
                {
                    pendingCameraToDestroy = lastActiveCameraCut.cam;
                    lastActiveCameraCut = null;
                }

                if (fadePanel != null)
                {
                    fadePanel.gameObject.SetActive(true);
                    fadePanel.color = new Color(0, 0, 0, 0);
                    fadePanel.DOFade(1f, 1f); // 어두워지기
                }
                break;
        }
    }

    private void HandleCutEnd(Cut currentCut, Cut nextCut)
    {
        switch (currentCut.state)
        {
            case SceneState.Camera:
                if (nextCut == null || nextCut.state != SceneState.Fade)
                {
                    if (currentCut.cam != null)
                        Destroy(currentCut.cam);
                }
                break;

            case SceneState.Fade:
                if (fadePanel != null)
                {
                    fadePanel.DOFade(0f, 1f).OnComplete(() =>
                    {
                        fadePanel.gameObject.SetActive(false);

                        // 페이드가 끝나면 이전 카메라를 안전하게 제거
                        if (pendingCameraToDestroy != null)
                        {
                            Destroy(pendingCameraToDestroy);
                            pendingCameraToDestroy = null;
                        }
                    });
                }
                break;
        }
    }

    private void OnComplete()
    {
        fadePanel.gameObject.SetActive(false);

        foreach (var e in afterEvents)
            e?.Invoke();
    }

    public void Off()
    {
        this.gameObject.SetActive(false);
    }
}
