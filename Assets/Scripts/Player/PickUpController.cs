using UnityEngine;
using TMPro;

public class PickUpController : MonoBehaviour
{
<<<<<<< Updated upstream
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
=======
    // 물체를 들 위치 (손 위치 등)
    public Transform holdPosition;  // 레이캐스트를 발사하는 위치
    public Transform pickPosition;  // 물체를 잡을 위치
    // 들고 있는 물체
    private GameObject heldObject;
    // 들고 있는 물체의 Rigidbody
    private Rigidbody heldObjectRb;
    // 감지된 물체
    private GameObject detectedObject;

    // 물체 감지 범위
    public float detectionRange = 10f;
    // 물체를 들 때의 오프셋 거리
    public float pickUpOffset = 0.5f;
    // UI 텍스트 (PickUp 메시지)
    public TMP_Text pickUpUI;

    // 물체를 밀 수 있는지 확인하는 컨트롤러
    private Player_Push_Controller pushController;
    // 물체가 pickable 상태로 감지되었는지 여부
    private bool isPickableDetected;

    // 던질 힘
    public float throwForce = 2f;

    // 던진 물체의 궤적을 그릴 라인 렌더러
    private LineRenderer trajectoryLine;
    // 던진 물체 궤적 점 개수
    public int trajectoryPoints = 30;
    // 궤적 점들 간 시간 간격
    public float timeBetweenPoints = 0.1f;

    // 화면 상의 십자가 크기 (픽셀)
    public float crosshairSize = 50f; 

    void Start()
    {
        // UI 텍스트 비활성화
        if (pickUpUI != null) pickUpUI.enabled = false;
        // 밀기 컨트롤러 초기화
        pushController = GetComponent<Player_Push_Controller>();

        // 궤적 라인 렌더러 설정
        trajectoryLine = gameObject.AddComponent<LineRenderer>();
        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = 0;
        trajectoryLine.startWidth = 0.05f;
        trajectoryLine.endWidth = 0.05f;
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = Color.yellow;
        trajectoryLine.endColor = Color.yellow;
>>>>>>> Stashed changes
    }

    void Update()
    {
<<<<<<< Updated upstream
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
=======
        // 물체 감지
        DetectPickableObject();

        // 'F' 키로 물체를 잡거나 놓기
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (detectedObject != null && heldObject == null)
            {
                TryPickUp();
            }
            else if (heldObject != null)
            {
                DropObject();
            }
        }

        // 물체를 들고 있을 경우
        if (heldObject != null)
        {
            // 물체 위치 업데이트 - pickPosition을 사용
            heldObject.transform.position = pickPosition.position + pickPosition.forward * pickUpOffset;

            // UI 표시
            if (pickUpUI != null)
            {
                pickUpUI.enabled = true;
                pickUpUI.text = "Press F to Drop";
            }

            // 'R' 키로 물체 회전
            if (Input.GetKeyDown(KeyCode.R)) RotateHeldObject();

            // 물체 궤적 표시
            DisplayTrajectory();

            // 마우스 왼쪽 버튼으로 물체 던지기
            if (Input.GetMouseButtonDown(0))
>>>>>>> Stashed changes
            {
                RotateHeldObject();
            }
        }
        else
        {
            trajectoryLine.positionCount = 0;
        }
    }

<<<<<<< Updated upstream
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
=======
    // 화면 가운데에 십자가 표시
    void OnGUI()
    {
        float screenCenterX = Screen.width / 2;
        float screenCenterY = Screen.height / 2;
        float thickness = 5f;

        GUI.color = Color.red;
        GUI.DrawTexture(new Rect(screenCenterX - crosshairSize / 2, screenCenterY - thickness / 2, crosshairSize, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(screenCenterX - thickness / 2, screenCenterY - crosshairSize / 2, thickness, crosshairSize), Texture2D.whiteTexture);
    }

    // 물체 감지 및 UI 업데이트
    void DetectPickableObject()
    {
        detectedObject = null;
        isPickableDetected = false;
        RaycastHit hit;

        // 레이캐스트로 앞쪽 물체 감지 (여전히 holdPosition에서 나옴)
        if (Physics.Raycast(holdPosition.position, holdPosition.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                detectedObject = hit.collider.gameObject;
                isPickableDetected = true;

                // UI 업데이트
                if (pickUpUI != null && heldObject == null)
                {
                    pickUpUI.enabled = true;
                    pickUpUI.text = "Press F to Pick Up";
                }

                Debug.Log("물건 감지중: " + detectedObject.name);
            }
        }

        // 레이캐스트 디버그
        Debug.DrawRay(holdPosition.position, holdPosition.forward * detectionRange, Color.red);

        // 감지된 물체 없으면 UI 숨기기
        if (detectedObject == null && heldObject == null && pickUpUI != null)
        {
            pickUpUI.enabled = false;
            isPickableDetected = false;
        }
    }

    // 물체를 잡기 시도
    void TryPickUp()
    {
        if (detectedObject == null) return;

        heldObject = detectedObject;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();

        if (heldObjectRb != null)
        {
            heldObjectRb.isKinematic = true;
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();
            if (heldObjectCollider != null && heldObject.CompareTag("Pickable"))
            {
                heldObjectCollider.isTrigger = true;
            }

            // 물체 위치와 회전 설정 (pickPosition을 사용)
            heldObject.transform.position = pickPosition.position + pickPosition.forward * pickUpOffset;
            heldObject.transform.rotation = pickPosition.rotation;
            heldObject.transform.parent = pickPosition;  // 물체의 부모를 pickPosition으로 설정
        }
    }

    // 물체 놓기
>>>>>>> Stashed changes
    void DropObject()
    {
        if (heldObject != null)
        {
<<<<<<< Updated upstream
            heldObjectRb.isKinematic = false; // 다시 물리 적용
            heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            heldObject.transform.parent = null; // 부모 해제

            // 다시 충돌 감지 활성화
=======
            heldObjectRb.isKinematic = false;
            heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            heldObject.transform.parent = null;

            // Collider 재설정
>>>>>>> Stashed changes
            if (heldObject.CompareTag("Pickable"))
            {
                Collider heldObjectCollider = heldObject.GetComponent<Collider>();
                if (heldObjectCollider != null)
                {
                    heldObjectCollider.isTrigger = false;
                }
            }

<<<<<<< Updated upstream
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
=======
            heldObject.transform.position += Vector3.up * 0.1f;
            heldObject = null;
        }
    }

    // 물체 던지기
    void ThrowObject()
    {
        if (heldObject != null && heldObjectRb != null)
        {
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();
            if (heldObjectCollider != null)
            {
                heldObjectCollider.isTrigger = false;
            }

            heldObjectRb.isKinematic = false;
            heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            // 던질 방향과 힘 설정
            Vector3 throwDirection = (pickPosition.forward + Vector3.up * 0.5f).normalized;
            heldObjectRb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);

            heldObject.transform.parent = null;
            heldObject.transform.position += Vector3.up * 0.1f;

            heldObject = null;
        }
    }

    // 물체 던짐 궤적 표시
    void DisplayTrajectory()
    {
        if (heldObject == null || heldObjectRb == null) return;

        trajectoryLine.positionCount = trajectoryPoints / 2;
        Vector3 currentPosition = heldObject.transform.position;
        Vector3 initialVelocity = (pickPosition.forward + Vector3.up * 0.5f).normalized * throwForce;

        // 물체 궤적 계산
        for (int i = 0; i < trajectoryPoints; i += 2)
        {
            float t = i * timeBetweenPoints;
            Vector3 displacement = initialVelocity * t + 0.5f * Physics.gravity * t * t;
            Vector3 pointPosition = currentPosition + displacement;

            trajectoryLine.SetPosition(i / 2, pointPosition);
        }
    }

    // 물체 회전
    void RotateHeldObject()
    {
        if (heldObject != null)
        {
            Quaternion currentRotation = heldObject.transform.rotation;
            Quaternion newRotation = currentRotation * Quaternion.Euler(0, 0, -90);
            heldObject.transform.rotation = newRotation;
        }
    }

    // 물체가 감지되었는지 여부 반환
    public bool IsPickableDetected()
    {
        return isPickableDetected || heldObject != null;
    }
>>>>>>> Stashed changes
}
