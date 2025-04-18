using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public static Dictionary<UIType, UIBase> startingUIDictionary = new Dictionary<UIType, UIBase>();

    public Stack<PopupUI> activatedPopups = new Stack<PopupUI>();


    Transform dontDestroyCanvas; // UI Object가 표시될 전용 캔버스 
    GameObject clickBlocker; // TODO : UI 활성화 시 클릭 막는 로직 추가 

    private void Awake()
    {
        #region Singleton

        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
        #endregion

        dontDestroyCanvas = GameObject.Find("DontDestroyCanvas").transform;

        RegisterUIObjects();
    }
    private void RegisterUIObjects()
    {
        UIBase[] startingUIs = dontDestroyCanvas.GetComponentsInChildren<UIBase>(true);

        foreach (UIBase ui in startingUIs)
        {
            if (!startingUIDictionary.ContainsKey(ui.type))
            {
                startingUIDictionary.Add(ui.type, ui);
            }
        }
    }


    /// <summary>
    /// UIBase로 반환되므로 캐스팅이 필요하다 
    /// </summary>
    public UIBase GetUI(UIType type)
    {
        return startingUIDictionary[type];
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            EscPressed();
        }
    }

    public void EscPressed()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "StartScene") return;

        if (sceneName == "LobbyScene")
        {
            if (activatedPopups.Count > 0)
            {
                Debug.Log($"activated popup : {activatedPopups.Count}");
                activatedPopups.Peek().HideUI();
            }
            else
            {
                startingUIDictionary[UIType.OptionPopup].ShowUI();
            }
        }
        else
        {
            if (activatedPopups.Count > 0)
            {
                Debug.Log($"activated popup : {activatedPopups.Count}");
                activatedPopups.Peek().HideUI();
            }
            else
            {
                startingUIDictionary[UIType.IngameOption].ShowUI();
            }
        }

    }

}
