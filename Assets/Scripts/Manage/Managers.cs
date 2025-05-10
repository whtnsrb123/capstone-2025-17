using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.PlayerLoop;
[DefaultExecutionOrder(-100)] // 가능한 한 빨리 실행되게
public class Managers : MonoBehaviour
{
    private static Managers s_instance;
    public static Managers Instance
    {
        get
        {
            if (s_instance == null)
                Init();
            return s_instance;
        }
    }

    // ------------------------------ 여기서 각 매니저에 접근할 수 있도록 static 프로퍼티 제공 ------------------------------
    public static GameStateManager GameStateManager => Instance._gameState;
    public static GameTimerManager GameTimerManager => Instance._gameTimer;
    public static MissionManager MissionManager => Instance._missionManager;
    public static GimmickManager GimmickManager => Instance._gimmickManager;

    GameStateManager _gameState;
    GameTimerManager _gameTimer;
    MissionManager _missionManager;
    GimmickManager _gimmickManager;
    // ----------------------------------------------------------------------------------------------------------
    List<IManager> _managerList = new List<IManager>();
    

    private static void Init()
    {
        if (s_instance != null)
            return;

        GameObject root = GameObject.Find("@Managers");
        if (root == null)
        {
            root = new GameObject("@Managers");
            DontDestroyOnLoad(root);
        }

        s_instance = root.GetComponent<Managers>();
        if (s_instance == null)
            s_instance = root.AddComponent<Managers>();

        s_instance.CreateManagers(root);
    }

    void CreateManagers(GameObject parent)
    {
        _gameState = CreateManager<GameStateManager>(parent, "GameStateManager");
        _gameTimer = CreateManager<GameTimerManager>(parent, "GameTimerManager");
        _missionManager = CreateManager<MissionManager>(parent, "MissionManager");
        _gimmickManager = CreateManager<GimmickManager>(parent, "GimmickManager");

        _managerList.Add(_gameState);
        _managerList.Add(_gameTimer);
        _managerList.Add(_missionManager);
        _managerList.Add(_gimmickManager);

        foreach (var m in _managerList)
            m.Init();
    }

    T CreateManager<T>(GameObject parent, string name) where T : MonoBehaviour, IManager
    {
        GameObject go = null;
        
        if (typeof(T) == typeof(GameTimerManager))
        {
            GameObject prefab = Resources.Load<GameObject>("GameTimerManager");
            if (prefab == null)
            {
                Debug.LogError("GameTimerManager 프리팹을 찾을 수 없습니다! Resources/GameTimerManager 위치 확인");
                return null;
            }

            go = PhotonNetwork.Instantiate(prefab.name, prefab.transform.position, prefab.transform.rotation);
            go.name = name;
            go.transform.SetParent(parent.transform);
        }
        else if (typeof(T) == typeof(GimmickManager))
        {
            GameObject prefab = Resources.Load<GameObject>("GimmickManager");
            if (prefab == null)
            {
                Debug.LogError("GimmickManager 프리팹을 찾을 수 없습니다! Resources/GimmickManager 위치 확인");
                return null;
            }
            go = PhotonNetwork.Instantiate(prefab.name, prefab.transform.position, prefab.transform.rotation);
            go.name = name;
            go.transform.SetParent(parent.transform);
        }
        else if (typeof(T) == typeof(MissionManager))
        {
            GameObject prefab = Resources.Load<GameObject>("MissionManager");
            if (prefab == null)
            {
                Debug.LogError("MissionManager 프리팹을 찾을 수 없습니다! Resources/MissionManager 위치 확인");
                return null;
            }
            go = PhotonNetwork.Instantiate(prefab.name, prefab.transform.position, prefab.transform.rotation);
            go.name = name;
            go.transform.SetParent(parent.transform);
        }
        else if (typeof(T) == typeof(GameStateManager))
        {
            GameObject prefab = Resources.Load<GameObject>("GameStateManager");
            if (prefab == null)
            {
                Debug.LogError("GameStateManager 프리팹을 찾을 수 없습니다! Resources/GameStateManager 위치 확인");
                return null;
            }
            go = PhotonNetwork.Instantiate(prefab.name, prefab.transform.position, prefab.transform.rotation);
            go.name = name;
            go.transform.SetParent(parent.transform);
        }
        else
        {
            go = new GameObject(name);
            go.transform.SetParent(parent.transform);
            go.AddComponent<T>();
        }
        return go.GetComponent<T>();
    }
    
    


    public void Clear()
    {
        foreach (var m in _managerList)
            m.Clear();
    }
}