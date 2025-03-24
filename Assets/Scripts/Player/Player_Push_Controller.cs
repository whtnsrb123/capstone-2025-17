using UnityEngine;
using TMPro;
using System.Collections;

public class Player_Push_Controller : MonoBehaviour
{
    public float pushForce = 3f; // 밀치는 힘
    private bool canPush = false; // 밀칠 수 있는 상태 여부
    private Rigidbody targetPlayerRb; // 밀칠 대상 플레이어의 Rigidbody

    public Transform holdPosition; // 레이캐스트 발사 위치
    public float detectionRange = 0.5f; // 감지 거리
    public float fieldOfView = 120f; // 감지할 시야각
    public int rayCount = 20; // 감지를 위한 레이의 개수
    public TMP_Text pushUI; // 밀칠 수 있을 때 표시할 UI

    private bool isPushing = false; // 밀치는 중인지 여부
    public float pushDelay = 1f; // 밀치기 딜레이 (초)

    void Start()
    {
        // UI가 존재하면 비활성화
        if (pushUI != null) pushUI.enabled = false;
        
        // holdPosition이 설정되지 않았을 경우 기본값을 transform으로 설정
        if (holdPosition == null) holdPosition = transform;
    }

    void Update()
    {
        DetectPlayer(); // 플레이어 감지 함수 호출
    }

    void DetectPlayer()
    {
        targetPlayerRb = null; // 감지 초기화
        canPush = false;

        RaycastHit hit;

        // holdPosition이 null인지 확인
        if (holdPosition == null)
        {
            Debug.LogError("holdPosition이 설정되지 않았습니다.");
            return;
        }

        // 정면으로 하나의 레이캐스트를 쏘아 플레이어 감지
        if (Physics.Raycast(holdPosition.position, holdPosition.forward, out hit, detectionRange))
        {
            // Player 태그 감지
            if (hit.collider.CompareTag("Player") && hit.collider.gameObject != gameObject)
            {
                targetPlayerRb = hit.collider.GetComponent<Rigidbody>();
                if (targetPlayerRb != null)
                {
                    canPush = true;
                    pushUI.enabled = true;
                    pushUI.text = "Left Click to Push"; // UI 메시지 설정
                    
                    Debug.Log("플레이어 감지됨: " + hit.collider.gameObject.name);
                }
                else
                {
                    Debug.LogWarning("감지된 오브젝트에 Rigidbody가 없습니다: " + hit.collider.gameObject.name);
                }
            }
            else
            {
                Debug.Log("감지된 오브젝트가 Player 태그가 아님: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("레이캐스트로 아무것도 감지되지 않음");
        }

        // 플레이어를 감지하지 못했을 경우 UI 숨김
        if (targetPlayerRb == null && pushUI != null)
        {
            pushUI.enabled = false;
        }
    }

    public void PushPlayer()
    {
        if (!canPush || isPushing)
        {
            Debug.Log("밀치기 불가능: canPush = " + canPush + ", isPushing = " + isPushing);
            return;
        }

        isPushing = true; // 밀치는 중으로 설정

        // targetPlayerRb가 null인지 확인
        if (targetPlayerRb == null)
        {
            Debug.LogError("targetPlayerRb가 null입니다. 밀칠 대상이 없습니다.");
            isPushing = false;
            return;
        }

        // 밀칠 방향 계산
        Vector3 pushDirection = (targetPlayerRb.transform.position - transform.position).normalized;
        targetPlayerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse); // 힘 적용하여 밀치기
        Debug.Log("밀치기 성공: " + targetPlayerRb.gameObject.name); // 디버그 메시지

        StartCoroutine(ResetPushing()); // 딜레이 코루틴 시작
    }

    public bool CanPush()
    {
        return canPush && !isPushing;
    }

    IEnumerator ResetPushing()
    {
        yield return new WaitForSeconds(pushDelay); // 딜레이 시간만큼 대기
        isPushing = false; // 밀치기 가능 상태로 변경
    }
}