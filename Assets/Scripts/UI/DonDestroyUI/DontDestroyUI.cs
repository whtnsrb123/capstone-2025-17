using UnityEngine;
using Photon.Pun;

public class DontDestroyUI : MonoBehaviour
{
    public static DontDestroyUI Instance {  get; private set; }
   
    GameObject loadingUI;
    LoadingPanel loadingPanel;

    private void Awake()
    {
        #region Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
        #endregion

    }
}
