using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public bool isMoving = false;
    public bool isDown = true;

    public Vector3 downPosition;
    public Vector3 upPosition;
    
    // Move 함수를 호출해 엘리베이터가 움직임
    // 움직이는 중에는 플레이어가 간섭할 수 없도록 isMoving 변수를 사용
    // 엘리베이터의 시작과 끝 위치를 직접 지정
    // 탑승, 하차, 조작의 상호작용 연결 필요
    
    void Start()
    {
        Invoke("Move", 2f);
    }

    public void Move()
    {
        if (isMoving) return;

        isMoving = true;
        StartCoroutine(MoveElevator());
    }
    
    IEnumerator MoveElevator()
    {
        Vector3 targetPosition = isDown ? upPosition : downPosition;
        Vector3 startPosition = transform.position;
        float time = 0;
        float duration = 5f;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            yield return null;
        }

        isDown = !isDown;
        isMoving = false;
    }
}