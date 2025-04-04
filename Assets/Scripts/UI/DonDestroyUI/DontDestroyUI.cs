using UnityEngine;
using Photon.Pun;

public class DontDestroyUI : MonoBehaviour
{
    public GameObject optionPanel;
    public GameObject loadingPanel;

    public static DontDestroyUI Instance {  get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        optionPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }


    public static void SetUIVisibility(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }

    public static void SetUIVisibility(GameObject go, bool visible)
    {
        go.SetActive(visible);
    }
}
