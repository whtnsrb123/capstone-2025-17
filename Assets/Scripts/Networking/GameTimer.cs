using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance {get; private set;}
    public float timeLimit = 300.0f;
    private float currentTime;

    
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
    private void Start()
    {
        currentTime = timeLimit;
        StartCoroutine(TimeCountdown());
    }

    private IEnumerator TimeCountdown()
    {
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            currentTime--;
        }
        
        //시간 초과되었으면 게임 종료 체크
        GameStateManager.Instance.CheckGameEnd();
    }

    public bool IsTimeOver()
    {
        return currentTime <= 0.0f;
    }
}
