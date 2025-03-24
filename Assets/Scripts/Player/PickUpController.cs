using UnityEngine;
using TMPro;

public class PickUpController : MonoBehaviour
{
    public Transform raycastPosition;  // 레이캐스트를 발사하는 위치 (holdPosition에서 변경)
    public Transform pickPosition;  // 물체를 잡을 위치
    private GameObject heldObject; // 들고 있는 물체
    private Rigidbody heldObjectRb; // 들고 있는 물체의 Rigidbody
    private GameObject detectedObject; // 감지된 물체

    public float detectionRange = 10f; // 물체 감지 범위
    public float pickUpOffset = 0.5f; // 물체를 들 때의 오프셋 거리
    public TMP_Text pickUpUI; // UI 텍스트 (PickUp 메시지)
    private Player_Push_Controller pushController; // 물체를 밀 수 있는지 확인
    public float throwForce = 2f; // 던질 힘
    private LineRenderer trajectoryLine; // 던진 물체의 궤적을 그릴 라인 렌더러
    public int trajectoryPoints = 50; // 던진 물체 궤적 점 개수
    public float timeBetweenPoints = 0.03f; // 궤적 점들 간 시간 간격
    public float crosshairSize = 50f; // Aim pointer size
    void Start()
    {
        Camera mainCamera = Camera.main;
        raycastPosition = mainCamera.transform; // holdPosition을 raycastPosition으로 변경

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
        trajectoryLine.numCornerVertices = 5; // 꺾임을 부드럽게 처리
        trajectoryLine.numCapVertices = 5; // 끝 부분 둥글게
    }

    void Update()
    {
        // 래이캐스트로 물체 감지
        DetectPickableObject();

        // F 키
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (detectedObject != null && heldObject == null) TryPickUp();
            else if (heldObject != null) DropObject();
        }

        // 물체를 들고 있을 경우
        if (heldObject != null)
        {
            // pickPosition이 할당되었는지 확인
            if (pickPosition != null)
            {
                // 물체 위치 업데이트 - pickPosition을 사용
                heldObject.transform.position = pickPosition.position + pickPosition.forward * pickUpOffset;
            }

            // UI 표시
            if (pickUpUI != null)
            {
                pickUpUI.enabled = true;
                pickUpUI.text = "Press F to Drop";
            }
            // 물체 회전
            if (Input.GetKeyDown(KeyCode.R)) RotateHeldObject();
            
            // 물체 던지기
            if (Input.GetMouseButtonDown(0)) ThrowObject();
        }
        else trajectoryLine.positionCount = 0;
    }

        void FixedUpdate()
        {
                // 물체 궤적 표시
            DisplayTrajectory();
        }

        // Aim Pointer
        void OnGUI()
    {
        float screenCenterX = Screen.width / 2;
        float screenCenterY = Screen.height / 2;
        float thickness = 5f; // 굵기

        GUI.color = Color.red; // 색상
        GUI.DrawTexture(new Rect(screenCenterX - crosshairSize / 2, screenCenterY - thickness / 2, crosshairSize, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(screenCenterX - thickness / 2, screenCenterY - crosshairSize / 2, thickness, crosshairSize), Texture2D.whiteTexture);
    }

    // 물체 감지, UI 업데이트
    void DetectPickableObject()
    {
        detectedObject = null;
        RaycastHit hit;

        // raycastPosition이 할당되었는지 확인 (holdPosition에서 변경)
        if (raycastPosition == null)
        {
            Debug.LogError("raycastPosition 변수가 할당되지 않았습니다.");
            return;
        }

        // 레이캐스트로 물체감지
        if (Physics.Raycast(raycastPosition.position, raycastPosition.forward, out hit, detectionRange)) // holdPosition을 raycastPosition으로 변경
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                detectedObject = hit.collider.gameObject;
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
        Debug.DrawRay(raycastPosition.position, raycastPosition.forward * detectionRange, Color.red); // holdPosition을 raycastPosition으로 변경

        // 감지된 물체 없으면 UI 숨기기
        if (detectedObject == null && heldObject == null && pickUpUI != null)
        {
            pickUpUI.enabled = false;
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


            Bounds objectBounds = heldObjectCollider.bounds;
            float objectRadius = Mathf.Max(objectBounds.extents.x, objectBounds.extents.z); // 가로 반경 중 큰 값 사용

            // pickPosition이 할당되었는지 확인
            if (pickPosition != null)
            {
                Vector3 newPickPosition = pickPosition.position + pickPosition.forward * (objectRadius + pickUpOffset);
                heldObject.transform.position = newPickPosition; // 조정된 위치에 배치
                heldObject.transform.rotation = pickPosition.rotation;
                heldObject.transform.parent = pickPosition;
            }
        }
    }


    // 물체 놓기// 물체 놓기
    void DropObject()
    {
        if (heldObject != null)
        {
            heldObjectRb.isKinematic = false;  //  물체에 물리 적용
            heldObjectRb.useGravity = true;    //  중력 활성화
            heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            heldObject.transform.parent = null; // 부모 관계 해제

            // Collider 설정 되돌리기
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();
            if (heldObjectCollider != null)
            {
                heldObjectCollider.isTrigger = false; //  충돌 감지 활성화
            }

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
            if (pickPosition != null)
            {
                Vector3 throwDirection = (pickPosition.forward + Vector3.up * 0.5f).normalized;
                heldObjectRb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);

                heldObject.transform.parent = null;
                heldObject.transform.position += Vector3.up * 0.1f;

                heldObject = null;
            }
        }
    }

    // 물체 던짐 궤적 표시
    void DisplayTrajectory()
    {
        if (heldObject == null || heldObjectRb == null) return;

        trajectoryLine.positionCount = trajectoryPoints / 2; // 점 개수 절반으로 줄이기
        Vector3 currentPosition = heldObject.transform.position;
        Vector3 initialVelocity = (pickPosition.forward + Vector3.up * 0.5f).normalized * throwForce;

        bool drawPoint = true;
        int index = 0;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeBetweenPoints;
            Vector3 displacement = (initialVelocity * t) + (0.5f * Physics.gravity * t * t);
            Vector3 pointPosition = currentPosition + displacement;

            // 홀수 번째 점만 추가하여 점선처럼 보이게 만들기
            if (drawPoint && index < trajectoryLine.positionCount)
            {
                trajectoryLine.SetPosition(index, pointPosition);
                index++;
            }

            drawPoint = !drawPoint; // 번갈아가며 점 추가
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
}