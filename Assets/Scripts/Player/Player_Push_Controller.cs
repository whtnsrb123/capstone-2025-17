using UnityEngine;
using TMPro;

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

        if (canPush && Input.GetMouseButtonDown(0)) PushPlayer();
    }
void DetectPlayer()
{
    targetPlayerRb = null; // 감지 초기화
    canPush = false;

    RaycastHit hit;

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
        }
    }

    // 플레이어를 감지하지 못했을 경우 UI 숨김
    if (targetPlayerRb == null && pushUI != null)
    {
        pushUI.enabled = false;
    }
}


    void PushPlayer()
    {
        // 밀칠 대상이 있고 PickUp이 우선권을 갖지 않은 경우 실행
        Vector3 pushDirection = (targetPlayerRb.transform.position - transform.position).normalized; // 밀칠 방향 계산
        targetPlayerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse); // 힘 적용하여 밀치기
         Debug.Log("밀치기 성공"); // 디버그 메시지
        
    }
}
