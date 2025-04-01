using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLauncher : MonoBehaviour
{
    public float windForce = 10f;
    public Vector3 windDirection = Vector3.forward;
    
    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            Vector3 forceDirection = transform.TransformDirection(windDirection).normalized;
            rb.AddForce(forceDirection * windForce, ForceMode.Acceleration);
        }
    }

    private void OnDrawGizmos()
    {
        // 바람 구멍의 위치
        Vector3 startPos = transform.position;
    
        // 바람이 나가는 방향 (Transform 기준 변환)
        Vector3 direction = transform.TransformDirection(windDirection).normalized;
    
        // 화살표 그리기
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(startPos, direction * 2f); // 바람의 방향을 나타내는 선
    
        // 화살표 끝에 삼각형 모양 추가
        Vector3 arrowHead = startPos + direction * 2f;
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;
    
        Gizmos.DrawRay(arrowHead, right * 0.5f);
        Gizmos.DrawRay(arrowHead, left * 0.5f);
    }
}