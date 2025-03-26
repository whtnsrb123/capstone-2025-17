using UnityEngine;
using TMPro;

public class PickUpController : MonoBehaviour
{
    public Transform raycastPosition;  // 레이캐스트를 발사하는 위치
    public Transform pickPosition;  // 물체를 잡을 위치
    private GameObject heldObject; // 들고 있는 물체
    private Rigidbody heldObjectRb; // 들고 있는 물체의 Rigidbody
    public GameObject detectedObject; // 감지된 물체

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
        // 밀기 컨트롤러 초기화
        pushController = GetComponent<Player_Push_Controller>();

        // 궤적 라인 렌더러 설정
        trajectoryLine = gameObject.AddComponent<LineRenderer>();
        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = 0;
        trajectoryLine.startWidth = 0.025f; // 기존 0.05f의 절반
        trajectoryLine.endWidth = 0.025f; // 기존 0.05f의 절반
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = new Color(1f, 1f, 0f, 0.5f); // 노란색, 투명도 0.5
        trajectoryLine.endColor = new Color(1f, 1f, 0f, 0.5f); // 노란색, 투명도 0.5
        trajectoryLine.numCornerVertices = 5; // 꺾임을 부드럽게 처리
        trajectoryLine.numCapVertices = 5; // 끝 부분 둥글게
    }

    void Update()
    {
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
            trajectoryLine.positionCount = 0;
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
        float thickness = 5f; // 굵기

        GUI.color = Color.red; // 색상
        GUI.DrawTexture(new Rect(screenCenterX - crosshairSize / 2, screenCenterY - thickness / 2, crosshairSize, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(screenCenterX - thickness / 2, screenCenterY - crosshairSize / 2, thickness, crosshairSize), Texture2D.whiteTexture);
    }

    public void HandlePickUpOrDrop()
    {
        if (detectedObject != null && heldObject == null)
        {
            Debug.Log("물체 잡기 시도: " + detectedObject.name);
            TryPickUp();
        }
        else if (heldObject != null)
        {
            Debug.Log("물체 놓기 시도: " + heldObject.name);
            DropObject();
        }
        else
        {
            Debug.Log("잡을 물체 없음: detectedObject = " + (detectedObject == null ? "null" : detectedObject.name));
        }
    }

    private void TryPickUp()
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
                Debug.Log("물체 잡기 성공: " + heldObject.name);
            }
            else
            {
                Debug.LogError("pickPosition이 설정되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogWarning("감지된 오브젝트에 Rigidbody가 없습니다: " + detectedObject.name);
            heldObject = null;
        }
    }

    private void DropObject()
    {
        if (heldObject != null)
        {
            heldObjectRb.isKinematic = false;  // 물체에 물리 적용
            heldObjectRb.useGravity = true;    // 중력 활성화
            heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            heldObject.transform.parent = null; // 부모 관계 해제

            // Collider 설정 되돌리기
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();
            if (heldObjectCollider != null)
            {
                heldObjectCollider.isTrigger = false; // 충돌 감지 활성화
            }

            heldObject = null;
            Debug.Log("물체 놓기 성공");
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
                Debug.Log("물체 던지기 성공");
            }
            else
            {
                Debug.LogError("pickPosition이 설정되지 않았습니다.");
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
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeBetweenPoints;
            Vector3 displacement = (initialVelocity * t) + (0.5f * Physics.gravity * t * t);
            Vector3 pointPosition = currentPosition + displacement;
            trajectoryLine.SetPosition(i, pointPosition);
        }
    }

    public void RotateHeldObject()
    {
        if (heldObject != null)
        {
            Quaternion currentRotation = heldObject.transform.rotation;
            Quaternion newRotation = currentRotation * Quaternion.Euler(0, 0, -90);
            heldObject.transform.rotation = newRotation;
            Debug.Log("물체 회전 성공");
        }
    }

    public bool IsHoldingObject()
    {
        return heldObject != null;
    }
}