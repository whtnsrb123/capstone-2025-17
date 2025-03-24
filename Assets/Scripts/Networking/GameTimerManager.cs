using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameTimerManager : MonoBehaviourPun, IPunObservable
{
    public TMP_Text timerText;  // UI 타이머 표시
    private float timer = 0f;
    private bool isTimerRunning = false;
    
    // 싱글톤을 유지하면 방을 나갔다가 들어가도 기존 인스턴스를 유지하는 문제가 생김.
    // 따라서 싱글톤을 제거하고, 방마다 새로운 GameTimerManager가 생성되도록 해야 함.
    private void Start()
    {
        if (!PhotonNetwork.InRoom)
        {
            Destroy(gameObject);
        }
    }
    
    //미션이 시작할 때 GameTimerManager.Instance.StartTimer(300f)호출 => 5분 타이머 시작
    public void StartTimer(float duration)
    {
        if (!PhotonNetwork.IsMasterClient) // 방장만 타이머 설정 가능
        {
            return;
        }
        
        timer = duration;
        isTimerRunning = true;
        
        //모든 플레이어가 타이머 시작을 동기화
        photonView.RPC(nameof(RPC_StartTimer), RpcTarget.All, duration);
    }
    
    [PunRPC]
    private void RPC_StartTimer(float duration)
    {
        if (timerText != null) return;
        
        // 이전 캔버스가 있다면 삭제
        GameObject oldCanvas = GameObject.Find("GameTimerCanvas");
        if (oldCanvas != null)
        {
            Destroy(oldCanvas);
        }
        
        //------------------------------Resources 폴더에서 GameTimerCanvas 불러와서 timerText 연결------------------------------------------------------
        // 캔버스 프리팹 로드 & 인스턴스화
        Canvas gameTimerCanvasPrefab = Resources.Load<Canvas>("GameTimerCanvas");
        if (gameTimerCanvasPrefab == null)
        {
            Debug.LogError("GameTimerCanvas 프리팹이 Resources 폴더에 없습니다!");
            return;
        }

        Canvas canvasInstance = Instantiate(gameTimerCanvasPrefab);

        // 자식 오브젝트에서 "gameTimer" 이름의 TMP_Text 찾기
        Transform timerTransform = canvasInstance.transform.Find("gameTimer");
        if (timerTransform != null)
        {
            timerText = timerTransform.GetComponent<TMP_Text>();
        }
        else
        {
            Debug.LogError("GameTimerCanvas 안에 gameTimer 오브젝트가 없습니다!");
        }
        //----------------------------------------------------------------------------------------------------------------------------------------
        timer = duration;
        isTimerRunning = true;
    }
    
    private void Update()
    {
        if (!isTimerRunning) return;
        
        timer -= Time.deltaTime;
        UpdateTimerUI();
        
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
        GameStateManager.Instance.CheckGameEnd();
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
