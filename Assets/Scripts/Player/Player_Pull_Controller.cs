using UnityEngine;

public class Player_Pull_Controller : MonoBehaviour
{
    public Transform holdPosition; // 손 위치 (선택 사항)
    public float rayDistance = 10f; // 레이캐스트 사정거리
    public float pushForce = 5f; // 밀치는 힘의 세기
    private GameObject PlayerObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 감지
        {
            Debug.Log("마우스 왼쪽 클릭 감지됨"); // 클릭 시 로그 출력
            PushPlayer();
        }
    }

    void PushPlayer()
    {
        // 카메라에서 마우스 위치로 레이캐스트 생성
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 레이캐스트가 rayDistance 거리 내에서 오브젝트에 닿는지 확인
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // 충돌한 오브젝트가 "Player" 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag("Player"))
            {
                PlayerObject = hit.collider.gameObject;
                
                // 플레이어에게 Rigidbody가 있는지 확인하고 밀치기
                Rigidbody rb = PlayerObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // 레이 방향으로 힘을 가해 밀침
                    Vector3 pushDirection = hit.point - transform.position;
                    pushDirection.Normalize(); // 방향 정규화
                    rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                    Debug.Log("Player가 밀렸습니다: " + PlayerObject.name); // 밀침 성공 시 로그 출력
                }
                else
                {
                    Debug.LogWarning("Player 오브젝트에 Rigidbody가 없습니다!");
                }
            }
        }
    }

    // 디버깅을 위해 레이캐스트를 시각적으로 확인 (선택 사항)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.DrawRay(ray.origin, ray.direction * rayDistance);
    }
}