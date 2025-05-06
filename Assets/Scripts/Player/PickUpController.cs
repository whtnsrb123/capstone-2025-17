using UnityEngine;
using TMPro;

public class PickUpController : MonoBehaviour
{
    private float defaultMass; // 물체의 원래 질량
    private float defaultDrag; // 물체의 원래 저항력
    private float defaultAngularDrag; // 물체의 원래 각속도 저항력



    public Transform raycastPosition;         // 레이캐스트 시작 위치 (카메라)
    public Transform pickPosition;            // 물체를 들 위치 (손 위치)
    public GameObject heldObject;             // 현재 들고 있는 물체
    private Rigidbody heldObjectRb;           // 들고 있는 물체의 Rigidbody
    public GameObject detectedObject;         // 감지된 물체

    private float detectionRange = 2f;        // 감지 거리
    private float pickUpOffset = 0.5f;        // 손과 물체 사이 거리
    [SerializeField] private TMP_Text pickUpUI; // UI 텍스트: "Press F to Drop"
    private PlayerPushController pushController;
    [SerializeField] private float throwForce = 10f; // 던지는 힘

    [SerializeField] private LineRenderer trajectoryLine; // 궤적 라인
    private int trajectoryPoints = 50;        // 궤적 점 수
    private float timeBetweenPoints = 0.03f;  // 점 간 시간
    [SerializeField] private float dashLength = 0.1f; // 점 길이
    [SerializeField] private float dashGap = 0.05f;   // 점 간 간격
    private float crosshairSize = 50f;        // 에임 크기

    [SerializeField] private float holdFollowSpeed = 60f;  // 손 위치로 빠르게 이동
    [SerializeField] private float holdRotateSpeed = 10f;  // 손 회전 따라오는 속도

    private Quaternion originalRotation; // 처음 잡았던 회전값 저장
    private bool isTouching = false;     // 다른 물체에 닿아 있는지 여부
    private Quaternion heldRotationOffset = Quaternion.identity; // R키로 회전한 상태 누적 저장
    private InteractManager interactManager;
    private GameObject recentlyThrownObject;
    private float throwCooldownTime = 1.0f; // 1초간 재감지 금지
    private float throwTimer = 0f;

    void Start()
    {
        interactManager = GetComponent<InteractManager>();
        pushController = GetComponent<PlayerPushController>();

        // 카메라 기준으로 레이 시작 위치 지정
        Camera mainCamera = Camera.main;
        raycastPosition = mainCamera.transform;

        // UI 텍스트 비활성화
        if (pickUpUI != null) pickUpUI.enabled = false;

        InitializeTrajectoryLine();
    }
    private void InitializeTrajectoryLine()
    {
        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = 0;
        trajectoryLine.startWidth = 0.05f;
        trajectoryLine.endWidth = 0.05f;
        trajectoryLine.numCornerVertices = 5;
        trajectoryLine.numCapVertices = 0;
        trajectoryLine.textureMode = LineTextureMode.Tile;

        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"))
        {
            mainTexture = CreateDashedTexture()
        };
        trajectoryLine.startColor = trajectoryLine.endColor = Color.yellow;
        trajectoryLine.material.mainTextureScale = new Vector2(1f / (dashLength + dashGap), 1f);
    }

    private Texture2D CreateDashedTexture()
    {
        Texture2D dashTexture = new Texture2D(4, 1);
        dashTexture.SetPixel(0, 0, Color.white);
        dashTexture.SetPixel(1, 0, Color.white);
        dashTexture.SetPixel(2, 0, Color.clear);
        dashTexture.SetPixel(3, 0, Color.clear);
        dashTexture.Apply();
        return dashTexture;
    }

    void Update()
    {
        if (throwTimer > 0f)
            throwTimer -= Time.deltaTime;

        Debug.DrawRay(raycastPosition.position, raycastPosition.forward * detectionRange, Color.red);

        if (heldObject != null)
        {
            if (pickUpUI != null)
            {
                pickUpUI.enabled = true;
                pickUpUI.text = "Press F to Drop";
            }
        }
        else if (trajectoryLine != null)
        {
            trajectoryLine.positionCount = 0;
        }
    }

    void FixedUpdate()
    {
        // 궤적 그리기
        DisplayTrajectory();

        // 들고 있는 물체 손 위치로 이동
        if (heldObject != null && heldObjectRb != null && pickPosition != null)
        {
            // 위치 이동
            Vector3 targetPosition = pickPosition.position + pickPosition.forward * pickUpOffset;
            Vector3 moveDirection = (targetPosition - heldObjectRb.position);
            heldObjectRb.MovePosition(heldObjectRb.position + moveDirection * holdFollowSpeed * Time.fixedDeltaTime);

            // 충돌 감지
            Vector3 checkSize = heldObject.transform.localScale * 0.5f;
            isTouching = Physics.CheckBox(heldObjectRb.position, checkSize, heldObjectRb.rotation, ~LayerMask.GetMask("HeldObject"));

            // 회전: pickPosition의 회전에 R 키 누적 회전 추가
            Quaternion targetRotation = isTouching
                ? pickPosition.rotation * heldRotationOffset
                : originalRotation * heldRotationOffset;

            heldObjectRb.MoveRotation(Quaternion.Slerp(heldObjectRb.rotation, targetRotation, holdRotateSpeed * Time.fixedDeltaTime));
        }
    }

    void OnGUI()
    {
        float screenCenterX = Screen.width / 2;
        float screenCenterY = Screen.height / 2;
        float thickness = 2f;

        GUI.color = Color.red;
        GUI.DrawTexture(new Rect(screenCenterX - crosshairSize / 2, screenCenterY - thickness / 2, crosshairSize, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(screenCenterX - thickness / 2, screenCenterY - crosshairSize / 2, thickness, crosshairSize), Texture2D.whiteTexture);
    }

    public void HandlePickUpOrDrop()
    {
        if (detectedObject != null && heldObject == null)
        {
            TryPickUp(); // 줍기
        }
        else if (heldObject != null)
        {
            DropObject(); // 놓기
        }
    }
    private void TryPickUp()
    {
        if (detectedObject == null) return;
        if (detectedObject == recentlyThrownObject && throwTimer > 0f)
        {
            Debug.Log("감지 금지 : 최근 던진 오브젝트");
            return;
        }

        heldObject = detectedObject;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();
        if (heldObjectRb != null)
        {
            // 원래 물리 속성 저장
            defaultMass = heldObjectRb.mass;
            defaultDrag = heldObjectRb.drag;
            defaultAngularDrag = heldObjectRb.angularDrag;

            // 들고 있을 때 가벼운 속성 적용
            heldObjectRb.mass = 0.1f; // 매우 가벼운 질량
            heldObjectRb.drag = 5f;   // 높은 저항력
            heldObjectRb.angularDrag = 10f; // 높은 회전 저항력

            heldObjectRb.isKinematic = false;
            heldObjectRb.useGravity = false;
            heldObject.layer = LayerMask.NameToLayer("HeldObject");
            originalRotation = heldObject.transform.rotation;
        }
    }


    public void DropObject()
    {
        if (heldObject != null)
        {
            // 원래 물리 속성 복구
            heldObjectRb.mass = defaultMass;
            heldObjectRb.drag = defaultDrag;
            heldObjectRb.angularDrag = defaultAngularDrag;

            heldObjectRb.useGravity = true;
            heldObjectRb.isKinematic = false;
            heldObject.layer = LayerMask.NameToLayer("Default");
        }
        ResetHeldObject();
    }

    public void ThrowObject()
    {
        if (heldObject != null && heldObjectRb != null)
        {
            heldObjectRb.mass = defaultMass;
            heldObjectRb.drag = defaultDrag;
            heldObjectRb.angularDrag = defaultAngularDrag;

            heldObjectRb.useGravity = true;
            heldObjectRb.isKinematic = false;
            heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            Vector3 throwDirection = (pickPosition.forward + Vector3.up * 0.5f).normalized;
            heldObjectRb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);

            heldObject.transform.position += Vector3.up * 0.1f;
            Debug.Log("물체 던지기");
            recentlyThrownObject = heldObject;
            throwTimer = throwCooldownTime;
        }

        ResetHeldObject();
    }
    private void ResetHeldObject()
    {
        if (heldObject != null)
            heldObject.layer = LayerMask.NameToLayer("Default");

        heldObject = null;
        heldObjectRb = null;

        if (TryGetComponent(out InteractManager interact))
            interact.ResetDetection();
    }

    private void DisplayTrajectory()
    {
        if (heldObject == null || heldObjectRb == null || pickPosition == null) return;

        trajectoryLine.positionCount = trajectoryPoints;

        Vector3 currentPosition = heldObject.transform.position;
        Vector3 initialVelocity = (pickPosition.forward + Vector3.up * 0.5f).normalized * throwForce;

        float totalLength = 0f;
        Vector3 previousPoint = currentPosition;

        // 들고 있는 물체 콜라이더
        Collider heldCollider = heldObject.GetComponent<Collider>();

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeBetweenPoints;
            Vector3 displacement = (initialVelocity * t) + (0.5f * Physics.gravity * t * t);
            Vector3 pointPosition = currentPosition + displacement;

            trajectoryLine.SetPosition(i, pointPosition);

            // Ray를 현재 궤적 구간 방향으로 발사
            Ray ray = new Ray(previousPoint, (pointPosition - previousPoint).normalized);
            float distance = Vector3.Distance(previousPoint, pointPosition) * 0.9f;

            // 모든 충돌체 감지
            RaycastHit[] hits = Physics.RaycastAll(ray, distance, ~0, QueryTriggerInteraction.Ignore);

            foreach (var hit in hits)
            {
                // 들고 있는 물체는 무시
                if (heldCollider != null && hit.collider == heldCollider)
                    continue;

                // 다른 물체와 충돌 시 궤적 종료
                trajectoryLine.positionCount = i + 1;
                return;
            }

            if (i > 0)
            {
                totalLength += Vector3.Distance(previousPoint, pointPosition);
            }
            previousPoint = pointPosition;
        }

        // 궤적 길이에 따라 텍스처 스케일 조정
        trajectoryLine.material.mainTextureScale = new Vector2(totalLength / (dashLength + dashGap), 1f);
    }

    public void RotateHeldObject()
    {
        heldRotationOffset *= Quaternion.Euler(0, 0, -90);
        Debug.Log("물체 회전");
    }

    public bool IsHoldingObject()
    {
        return heldObject != null;
    }
}