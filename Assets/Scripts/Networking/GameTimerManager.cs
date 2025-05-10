using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameTimerManager : MonoBehaviourPun, IManager
{
    public TMP_Text timerText;  // UI 타이머 표시
    private float timer = 0f;
    private bool isTimerRunning = false;
    
    // 초 변경 감지를 위한 변수
    private int currentSeconds = 0;
    private int lastSeconds = 0;
    
    // 싱글톤을 유지하면 방을 나갔다가 들어가도 기존 인스턴스를 유지하는 문제가 생김.
    // 따라서 싱글톤을 제거하고, 방마다 새로운 GameTimerManager가 생성되도록 해야 함.
    private void Start()
    {
        if (GameStateManager.isServerTest && !PhotonNetwork.InRoom)
        {
            Destroy(gameObject);
        }
    }
    
    public void Init()
    {
        // photonView는 MonoBehaviourPun이 자동으로 연결해줌
        if (photonView == null)
        {
            Debug.LogError("PhotonView가 연결되지 않았습니다!");
        }
        else
        {
            Debug.Log("GameTimerManager 초기화 완료");
        }
    }

    public void Clear()
    {
        Debug.Log("GameTimerManager 클리어");
    }
    
    //미션이 시작할 때 GameTimerManager.Instance.StartTimer(300f)호출 => 5분 타이머 시작
    public void StartTimer(float duration)
    {
        if (!PhotonNetwork.IsMasterClient) // 방장만 타이머 설정 가능
        {
            return;
        }
        
        //모든 플레이어가 타이머 시작을 동기화
        photonView.RPC(nameof(RPC_StartTimer), RpcTarget.All, duration);
    }
    
    [PunRPC]
    private void RPC_StartTimer(float duration)
    {
        // 이미 타이머 텍스트가 세팅되어 있으면 다시 만들지 않음
        if (timerText == null)
        {
            // 기존 캔버스 찾기
            GameObject existingCanvas = GameObject.Find("GameTimerCanvas");

            if (existingCanvas != null)
            {
                Transform timerTransform = existingCanvas.transform.Find("gameTimer");
                if (timerTransform != null)
                {
                    timerText = timerTransform.GetComponent<TMP_Text>();
                }
                else
                {
                    Debug.LogError("기존 Canvas에 gameTimer 오브젝트가 없습니다!");
                }
            }
            else
            {
                // 프리팹 로드
                Canvas gameTimerCanvasPrefab = Resources.Load<Canvas>("GameTimerCanvas");
                if (gameTimerCanvasPrefab == null)
                {
                    Debug.LogError("GameTimerCanvas 프리팹이 Resources 폴더에 없습니다!");
                    return;
                }

                Canvas canvasInstance = Instantiate(gameTimerCanvasPrefab);
                canvasInstance.name = "GameTimerCanvas"; // 이름 강제 설정

                Transform timerTransform = canvasInstance.transform.Find("gameTimer");
                if (timerTransform != null)
                {
                    timerText = timerTransform.GetComponent<TMP_Text>();
                }
                else
                {
                    Debug.LogError("GameTimerCanvas 안에 gameTimer 오브젝트가 없습니다!");
                }
            }
        }

        // 타이머 시작
        timer = duration;
        isTimerRunning = true;
    }
    
    private void Update()
    {
        if (!isTimerRunning) return;
        
        timer -= Time.deltaTime;
        
        currentSeconds = Mathf.FloorToInt(timer);
        if (currentSeconds != lastSeconds) // 초가 변경될 때만 UI 업데이트
        {
            lastSeconds = currentSeconds;
            UpdateTimerUI();
        }
        
        
        if (timer <= 0f)
        {
            isTimerRunning = false;
            timer = 0f;
            photonView.RPC(nameof(RPC_OnTimeUp), RpcTarget.All);
        }
    }
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
    [PunRPC]
    private void RPC_OnTimeUp()
    {
        Debug.Log("타임 오버! 게임 오버 처리 필요");
        //시간 초과되었으면 게임 종료 체크
        //GameStateManager.Instance.CheckGameEnd();
        Managers.GameStateManager.CheckGameEnd();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 방장이 데이터 전송
        {
            stream.SendNext(timer);
            stream.SendNext(isTimerRunning);
        }
        else // 다른 클라이언트가 데이터 받기
        {
            timer = (float)stream.ReceiveNext();
            isTimerRunning = (bool)stream.ReceiveNext();
        }
    }

    //제한 시간 초과 여부 넘겨주는 함수
    public bool IsTimeOver()
    {
        return timer <= 0f;
    }
}
