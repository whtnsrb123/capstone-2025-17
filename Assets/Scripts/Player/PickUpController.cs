using UnityEngine;
using TMPro;

public class PickUpController : MonoBehaviour
{
    [SerializeField] private Transform raycastPosition;  // 레이캐스트를 발사하는 위치 (메인 카메라 Transform)
    [SerializeField] private Transform pickPosition;  // 물체를 잡을 위치 (플레이어 앞의 Transform)
    private GameObject heldObject; // 들고 있는 물체
    private Rigidbody heldObjectRb; // 들고 있는 물체의 Rigidbody
    private GameObject detectedObject; // 감지된 물체

    private float detectionRange = 10f; // 물체 감지 범위
    private float pickUpOffset = 0.5f; // 물체를 들 때의 오프셋 거리
    [SerializeField] private TMP_Text pickUpUI; // UI 텍스트 (PickUp 메시지)
    private Player_Push_Controller pushController; // 물체를 밀 수 있는지 확인
    [SerializeField] private float throwForce = 10f; // 던질 힘
    [SerializeField] private LineRenderer trajectoryLine; // 던진 물체의 궤적을 그릴 라인 렌더러 (인스펙터에서 참조)
    private int trajectoryPoints = 50; // 던진 물체 궤적 점 개수
    private float timeBetweenPoints = 0.03f; // 궤적 점들 간 시간 간격
    [SerializeField] private float dashLength = 0.1f; // 점선의 길이 (점의 길이)
    [SerializeField] private float dashGap = 0.05f; // 점선 간격 (점 사이의 빈 공간)
    private float crosshairSize = 50f; // Aim pointer size

    void Start()
    {
        // 메인 카메라를 레이캐스트 위치로 설정
        Camera mainCamera = Camera.main;
        raycastPosition = mainCamera.transform;

        // UI 텍스트 비활성화
        if (pickUpUI != null) pickUpUI.enabled = false;

        // 밀기 컨트롤러 초기화
        pushController = GetComponent<Player_Push_Controller>();

        // 궤적 라인 렌더러 설정 (인스펙터에서 이미 추가된 LineRenderer 사용)
        if (trajectoryLine == null)
        {
            Debug.LogError("LineRenderer가 인스펙터에 할당되지 않았습니다! 플레이어 오브젝트에 LineRenderer 컴포넌트를 추가하고, Trajectory Line 필드에 할당해주세요.");
            return;
        }

        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = 0;
        trajectoryLine.startWidth = 0.05f; // 라인 두께 설정
        trajectoryLine.endWidth = 0.05f;
        trajectoryLine.numCornerVertices = 5; // 꺾임을 부드럽게 처리
        trajectoryLine.numCapVertices = 0; // 끝 부분 둥글게 처리하지 않음

        // 점선 텍스처 생성 및 설정
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = new Color(1f, 1f, 0f, 1f); // 노란색
        trajectoryLine.endColor = new Color(1f, 1f, 0f, 1f);

        // 텍스처 모드를 타일로 설정
        trajectoryLine.textureMode = LineTextureMode.Tile;

        // 점선 패턴을 위한 텍스처 생성 (더 세밀한 텍스처로 변경)
        Texture2D dashTexture = new Texture2D(4, 1); // 텍스처 크기를 늘려 더 세밀한 점선 생성
        dashTexture.SetPixel(0, 0, Color.white); // 점 부분
        dashTexture.SetPixel(1, 0, Color.white); // 점 부분
        dashTexture.SetPixel(2, 0, Color.clear); // 빈 부분
        dashTexture.SetPixel(3, 0, Color.clear); // 빈 부분
        dashTexture.Apply();

        trajectoryLine.material.mainTexture = dashTexture;
        trajectoryLine.material.mainTextureScale = new Vector2(1f / (dashLength + dashGap), 1f); // 점선 간격 조정
    }

    void Update()
    {
        // 레이캐스트로 물체 감지
        DetectPickableObject();

        // 물체를 들고 있을 경우 위치 업데이트
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
        }
        else
        {
            trajectoryLine.positionCount = 0; // 물체를 들고 있지 않으면 궤적 라인 없애기
        }
    }

    void FixedUpdate()
    {
        // 물체 궤적 표시
        DisplayTrajectory();
    }

    void OnGUI()
    {
        float screenCenterX = Screen.width / 2;
        float screenCenterY = Screen.height / 2;
        float thickness = 2f; // 굵기 설정

        GUI.color = Color.red; // 색상 설정
        GUI.DrawTexture(new Rect(screenCenterX - crosshairSize / 2, screenCenterY - thickness / 2, crosshairSize, thickness), Texture2D.whiteTexture); // Aim pointer 그리기
        GUI.DrawTexture(new Rect(screenCenterX - thickness / 2, screenCenterY - crosshairSize / 2, thickness, crosshairSize), Texture2D.whiteTexture); // Aim pointer 그리기
    }

    void DetectPickableObject()
    {
        detectedObject = null;
        RaycastHit hit;

        // 레이캐스트로 물체 감지
        if (Physics.Raycast(raycastPosition.position, raycastPosition.forward, out hit, detectionRange))
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

        // 감지된 물체 없으면 UI 숨기기
        if (detectedObject == null && heldObject == null && pickUpUI != null)
        {
            pickUpUI.enabled = false;
        }
    }

    public void HandlePickUpOrDrop()
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

    private void TryPickUp()
    {
        if (detectedObject == null) return;

        heldObject = detectedObject;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();

        if (heldObjectRb != null)
        {
            heldObjectRb.isKinematic = true; // 물리 엔진 비활성화
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();

            if (heldObjectCollider != null && heldObject.CompareTag("Pickable"))
            {
                heldObjectCollider.isTrigger = true; // 충돌 감지 비활성화
            }

            Bounds objectBounds = heldObjectCollider.bounds;
            float objectRadius = Mathf.Max(objectBounds.extents.x, objectBounds.extents.z); // 가로 반경 중 큰 값 사용
            Vector3 newPickPosition = pickPosition.position + pickPosition.forward * (objectRadius + pickUpOffset);
            heldObject.transform.position = newPickPosition;
            heldObject.transform.rotation = pickPosition.rotation;
            heldObject.transform.parent = pickPosition;
            Debug.Log("물체 잡기 : " + heldObject.name);
        }
        else
        {
            heldObject = null; // 물체가 없다면 잡을 수 없음
        }
    }

    private void DropObject()
    {
        if (heldObject != null)
        {
            heldObjectRb.isKinematic = false;  // 물체에 물리 적용
            heldObjectRb.useGravity = true;    // 중력 활성화
            heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            heldObject.transform.parent = null; // 부모 객체에서 분리

            // Collider 설정 되돌리기
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();
            if (heldObjectCollider != null)
            {
                heldObjectCollider.isTrigger = false; // 충돌 감지 활성화
            }

            heldObject = null;
            Debug.Log("물체 놓기");
        }
    }

    public void ThrowObject()
    {
        if (heldObject != null && heldObjectRb != null)
        {
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();
            if (heldObjectCollider != null)
            {
                heldObjectCollider.isTrigger = false;
            }

            heldObjectRb.isKinematic = false; // 물리 적용
            heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            // 던질 방향과 힘 설정
            if (pickPosition != null)
            {
                Vector3 throwDirection = (pickPosition.forward + Vector3.up * 0.5f).normalized;
                heldObjectRb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);

                heldObject.transform.parent = null;
                heldObject.transform.position += Vector3.up * 0.1f;

                heldObject = null;
                Debug.Log("물체 던지기");
            }
        }
    }

    private void DisplayTrajectory()
    {
        if (heldObject == null || heldObjectRb == null || pickPosition == null) return;

        // 궤적 점 개수 설정
        trajectoryLine.positionCount = trajectoryPoints;
        Vector3 currentPosition = heldObject.transform.position;
        Vector3 initialVelocity = (pickPosition.forward + Vector3.up * 0.5f).normalized * throwForce;

        // 궤적 점 계산 및 설정
        float totalLength = 0f;
        Vector3 previousPoint = currentPosition;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeBetweenPoints;
            Vector3 displacement = (initialVelocity * t) + (0.5f * Physics.gravity * t * t);
            Vector3 pointPosition = currentPosition + displacement;
            trajectoryLine.SetPosition(i, pointPosition);

            // 궤적 길이 계산 (점선 스케일 조정용)
            if (i > 0)
            {
                totalLength += Vector3.Distance(previousPoint, pointPosition);
            }
            previousPoint = pointPosition;
        }

        // 점선 텍스처 스케일 조정
        trajectoryLine.material.mainTextureScale = new Vector2(totalLength / (dashLength + dashGap), 1f);
    }

    public void RotateHeldObject()
    {
        if (heldObject != null)
        {
            Quaternion currentRotation = heldObject.transform.rotation;
            Quaternion newRotation = currentRotation * Quaternion.Euler(0, 0, -90); // 90도 회전
            heldObject.transform.rotation = newRotation;
            Debug.Log("물체 회전");
        }
    }

    public bool IsHoldingObject()
    {
        return heldObject != null; // 물체를 들고 있는지 여부 반환
    }
}