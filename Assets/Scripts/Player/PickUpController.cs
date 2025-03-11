using Photon.Pun;
using UnityEngine;
using TMPro;

public class PickUpController : MonoBehaviourPun
{
    public Transform holdPosition; // 물체를 들고 있을 위치
    public TMP_Text interactionText; // 상호작용 UI 텍스트
    private GameObject heldObject; // 현재 들고 있는 물체
    private Rigidbody heldObjectRb; // 들고 있는 물체의 Rigidbody
    private GameObject detectedObject; // 감지된 물체

    public float detectionRange = 0.05f; // 감지 거리
    public float fieldOfView = 120f; // 시야각
    public int rayCount = 8; // 감지 레이 개수

    void Start()
    {
        // 상호작용 텍스트가 있으면 처음에는 비활성화
        if (interactionText != null)
        {
            interactionText.enabled = false;
        }
    }

    void Update()
    {
        //서버테스트중이 아니거나 로컬캐릭터일때만 이동처리
        if( GameStateManager.isServerTest && !photonView.IsMine ) return;
        
        DetectPickableObject(); // 주변에서 집을 수 있는 물체 감지

        // 물체를 들고 있으면 상호작용 텍스트 변경
        if (heldObject != null && interactionText != null)
        {
            interactionText.text = "Press F to Drop";
            interactionText.enabled = true;
        }

        // F 키를 누르면 집거나 놓기
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldObject == null)
            {
                TryPickUp(); // 물체를 집기 시도
            }
            else
            {
                DropObject(); // 들고 있는 물체를 놓기
            }
        }

        // 들고 있는 물체를 손 위치에 고정
        if (heldObject != null)
        {
            heldObject.transform.position = holdPosition.position;

            // R 키를 누르면 들고 있는 물체 회전
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateHeldObject();
            }
        }
    }

    // 들고 있는 물체를 회전
    void RotateHeldObject()
    {
        if (heldObject != null)
        {
            Quaternion currentRotation = heldObject.transform.rotation;
            Quaternion newRotation = currentRotation * Quaternion.Euler(0, 0, -90);
            heldObject.transform.rotation = newRotation;
        }
    }

    // 주위에서 집을 수 있는 물체 감지
    void DetectPickableObject()
    {
        detectedObject = null;
        float halfFOV = fieldOfView / 2; // 시야각 절반
        float angleStep = fieldOfView / rayCount; // 각도 간격
        RaycastHit hit;
        
        // 시야각 내에서 여러 개의 Ray를 쏴서 감지
        for (int i = 0; i <= rayCount; i++)
        {
            float angle = -halfFOV + (angleStep * i);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * holdPosition.forward;
            
            if (Physics.Raycast(holdPosition.position, direction, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Pickable"))
                {
                    detectedObject = hit.collider.gameObject;
                    Debug.Log("물건 감지중: " + detectedObject.name);

                    // 상호작용 텍스트 표시
                    if (interactionText != null)
                    {
                        interactionText.text = "Press F to Pick Up";
                        interactionText.enabled = true;
                    }
                    break;
                }
            }
            Debug.DrawRay(holdPosition.position, direction * detectionRange, Color.red); // 감지 Ray 시각화
        }

        // 감지된 물체가 없으면 UI 비활성화
        if (detectedObject == null && interactionText != null)
        {
            interactionText.enabled = false;
        }
    }

    // 감지된 물체를 집기
    void TryPickUp()
    {
        if (detectedObject == null) return;

        heldObject = detectedObject;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();

        if (heldObjectRb != null)
        {
            heldObjectRb.isKinematic = true; // 물리적 영향 제거

            // 충돌 감지를 위한 설정 변경
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();
            if (heldObjectCollider != null && heldObject.CompareTag("Pickable"))
            {
                heldObjectCollider.isTrigger = true;
            }

            // 물체를 손 위치로 이동 및 고정
            heldObject.transform.position = holdPosition.position;
            heldObject.transform.rotation = holdPosition.rotation;
            heldObject.transform.parent = holdPosition;
        }
    }

    // 들고 있는 물체를 놓기
    void DropObject()
    {
        if (heldObject != null)
        {
            heldObjectRb.isKinematic = false; // 다시 물리 적용
            heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            heldObject.transform.parent = null; // 부모 해제

            // 다시 충돌 감지 활성화
            if (heldObject.CompareTag("Pickable"))
            {
                Collider heldObjectCollider = heldObject.GetComponent<Collider>();
                if (heldObjectCollider != null)
                {
                    heldObjectCollider.isTrigger = false;
                }
            }

            // 물체가 바닥에 묻히지 않도록 살짝 위로 이동
            heldObject.transform.position += Vector3.up * 0.1f;

            heldObject = null; // 들고 있는 물체 초기화

            // 상호작용 UI 숨기기
            if (interactionText != null)
            {
                interactionText.enabled = false;
            }
        }
    }
}
