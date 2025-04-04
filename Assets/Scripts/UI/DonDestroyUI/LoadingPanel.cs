using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingPanel : MonoBehaviourPun
{
    public static LoadingPanel Instance { get; private set; }   

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressTMP;

    [SerializeField] private float move = 0.003f;

    private int currentPlayer = 0;

    private float currentTick;
    private float epsilon = 0.1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    IEnumerator LoadingProgress()
    {
        while (progressBar.value < 0.9f)
        {

            float currentProgress = PhotonNetwork.LevelLoadingProgress;

            if (progressBar.value < 0.75f)
            {
                currentTick += Time.deltaTime * move;
                progressBar.value += Time.deltaTime;
            }
            else if (currentProgress > progressBar.value + epsilon )
            {
                progressBar.value = Mathf.MoveTowards(progressBar.value, currentProgress, move);
            }
            else 
            {
                progressBar.value = currentProgress;
            }
            progressTMP.text = $"Loading... {progressBar.value * 100: 0.0}%";
            yield return null;
        }

        NotifySceneLoaded();
    }

    public void SetLoadingPanelVisibility(bool visibility)
    {
        photonView.RPC("SetUIVisibility", RpcTarget.All, visibility);
    }

    [PunRPC]
    public void SetUIVisibility(bool visibility)
    {
        Debug.Log("PunRPC SetUIVIsibility : " + visibility) ;

        loadingPanel.SetActive(visibility);
        if (visibility)
        {
            // 초기화
            currentPlayer = 0;
            currentTick = 0;
            progressBar.value = 0;
            progressTMP.text = "Loading...";

            StartCoroutine(nameof(LoadingProgress));
        }
    }

    [PunRPC]
    public void ClientSceneLoaded()
    {
        currentPlayer++;
        if (currentPlayer == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            photonView.RPC("SetUIVisibility", RpcTarget.All, false);
        }
    }

    void NotifySceneLoaded()
    {
        photonView.RPC("ClientSceneLoaded", RpcTarget.MasterClient);
    }



}
