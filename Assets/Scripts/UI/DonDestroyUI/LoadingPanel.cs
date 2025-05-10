using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviourPun
{
    public static LoadingPanel Instance { get; private set; }   

    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressTMP;

    private bool loaded;
    private float currentTick;

    PopupUI popup;

    private void Awake()
    {
        #region Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        #endregion

        Transform canvas = GameObject.Find("DontDestroyCanvas").transform;
        gameObject.transform.SetParent(canvas);

        popup = GetComponent<PopupUI>();
    }

    IEnumerator LoadingProgress()
    {
        // 초기화
        loaded = false;
        currentTick = 0;
        progressBar.value = 0;
        progressTMP.text = "Loading...";

        while (true)
        {
            if (PhotonNetwork.LevelLoadingProgress >= 0.95f && PhotonNetwork.IsMasterClient)
            {
                this.loaded = true;
            }

            if (progressBar.value < 0.5f)
            {
                currentTick += Time.deltaTime * 0.01f;
                progressBar.value += currentTick;
            }
            else if (progressBar.value < 0.94f )
            {
                currentTick += Time.deltaTime * 0.5f;
                progressBar.value += currentTick;
            }
            else
            {
                if ( this.loaded)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        // 마스터 클라이언트
                        SendISLoadedLevel(this.loaded);
                    }
                    SetUIVisibility(false);
                    GameObject.FindObjectOfType<StartCutScene>().PlayCutscene();
                    yield break;
                }
            }

            progressTMP.text = $"Loading... {progressBar.value * 100: 0.0}%";
            yield return null;
        }
    }

    public void SetLoadingPanelVisibility(bool visibility)
    {
        photonView.RPC("SetUIVisibility", RpcTarget.All, visibility);
    }

    [PunRPC]
    public void SetUIVisibility(bool visibility)
    {
        Debug.Log("PunRPC SetUIVIsibility : " + visibility) ;

        if (visibility)
        {
            popup.ShowUI();
            StartCoroutine(nameof(LoadingProgress));
        }
        else
        {
            popup.HideUI();
        }
    }

    public void SendISLoadedLevel(bool loaded)
    {
        photonView.RPC("SetLoaded", RpcTarget.All, loaded);
    }

    [PunRPC]
    public void SetLoaded(bool loaded)
    {
        this.loaded = loaded;
    }

    public void BeforeLoadedLobbyScene()
    {
        Destroy(gameObject);
    }

}
