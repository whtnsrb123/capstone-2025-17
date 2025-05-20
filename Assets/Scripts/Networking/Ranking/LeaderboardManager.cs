using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Linq;
using System;


public class LeaderboardManager : MonoBehaviour
{
    private static LeaderboardManager instance;
    public static LeaderboardManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LeaderboardManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("LeaderboardManager");
                    instance = go.AddComponent<LeaderboardManager>();
                }
            }
            return instance;
        }
    }

    private FirebaseFirestore db;
    private const string COLLECTION_NAME = "leaderboard";
    private bool isInitialized = false;

    // 초기화 완료 이벤트
    public delegate void InitializationCompleteHandler();
    public event InitializationCompleteHandler OnInitializationComplete;

    private const int DisplayCount = 5;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
#if UNITY_EDITOR
        InitializeFirebase();
#endif
    }

    private async void InitializeFirebase()
    {
        try
        {
            // Firebase 초기화
            await FirebaseApp.CheckAndFixDependenciesAsync();

            // Firestore 초기화
            db = FirebaseFirestore.DefaultInstance;
            isInitialized = true;
            Debug.Log("Firebase Firestore 초기화 완료");

            // 초기화 완료 이벤트 발생
            Debug.Log("초기화 완료");
            OnInitializationComplete?.Invoke();

            // 테스트 데이터 저장
            // await SaveTestData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Firebase 초기화 실패: {e.Message}");
        }
    }

    // 리더보드 데이터 저장 함수
    public async Task SaveLeaderboardData(float time, string[] players)
    {
        if (!isInitialized)
        {
            Debug.LogError("Firebase가 초기화되지 않았습니다.");
            return;
        }

        try
        {
            Dictionary<string, object> leaderboardData = new Dictionary<string, object>
            {
                { "time", time },
                { "players", players },
                { "timestamp", FieldValue.ServerTimestamp }
            };

            DocumentReference docRef = await db.Collection(COLLECTION_NAME).AddAsync(leaderboardData);
            Debug.Log($"리더보드 데이터 저장 성공: {docRef.Id}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"리더보드 데이터 저장 실패: {e.Message}");
        }
    }

    // 모든 리더보드 데이터를 시간 순으로 조회하는 함수
    public async Task<List<Dictionary<string, object>>> GetAllLeaderboardData()
    {
        if (!isInitialized)
        {
            Debug.LogError("Firebase가 초기화되지 않았습니다.");
            return new List<Dictionary<string, object>>();
        }

        try
        {
            QuerySnapshot querySnapshot = await db.Collection(COLLECTION_NAME).GetSnapshotAsync();
            List<Dictionary<string, object>> allData = new List<Dictionary<string, object>>();

            foreach (DocumentSnapshot document in querySnapshot.Documents)
            {
                Dictionary<string, object> data = document.ToDictionary();
                data["documentId"] = document.Id;
                allData.Add(data);
            }

            // time 기준으로 오름차순 정렬
            allData = allData.OrderBy(x => Convert.ToSingle(x["time"])).ToList();
            List<Dictionary<string, object>> topFive = allData.Take(5).ToList();

            // 정렬된 데이터 출력
            Debug.Log("=== 리더보드 데이터 (시간 순) ===");
            foreach (var data in allData)
            {
                float time = Convert.ToSingle(data["time"]);
                string[] players = ((List<object>)data["players"]).Select(p => p.ToString()).ToArray();
                // Debug.Log($"시간: {time:F2}초, 플레이어: {string.Join(", ", players)}");
            }

            return topFive;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"리더보드 데이터 조회 실패: {e.Message}");
            return new List<Dictionary<string, object>>();
        }
    }

    #region Test Code
    // 테스트 데이터 저장
    private async Task SaveTestData()
    {
        if (!isInitialized)
        {
            Debug.LogError("Firebase가 초기화되지 않았습니다.");
            return;
        }

        try
        {
            // 테스트 데이터 생성
            var testData = new List<(float time, string[] players)>
            {
                (78.5f, new string[] { "플레이어1", "플레이어6", "플레이어3", "플레이어7" }),
                (98.5f, new string[] { "플레이어1", "플레이어2", "플레이어3", "플레이어4" }),
                (120.3f, new string[] { "플레이어2", "플레이어3", "플레이어4", "플레이어5" }),
                (150.7f, new string[] { "플레이어1", "플레이어3", "플레이어4", "플레이어5" }),
                (180.2f, new string[] { "플레이어1", "플레이어2", "플레이어4", "플레이어5" }),
                (200.0f, new string[] { "플레이어1", "플레이어2", "플레이어3", "플레이어5" })
            };

            foreach (var data in testData)
            {
                await SaveLeaderboardData(data.time, data.players);
            }

            Debug.Log("테스트 데이터 저장 완료");

            // 저장된 데이터 확인
            await GetAllLeaderboardData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"테스트 데이터 저장 실패: {e.Message}");
        }
    }
    #endregion

}