using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 2f;            // 걷기 속도
    public float sprintSpeed = 3f;         // 달리기 속도
    public float jumpForce = 3f;           // 점프 힘
    public float mouseSensitivity = 300f;  // 마우스 감도
    public float groundCheckDistance = 0.4f; // 땅 감지 거리
    public float acceleration = 20f;       // 이동 가속도

    private bool isGrounded;               // 캐릭터가 땅에 닿아 있는지 여부
    private Rigidbody rb;                   // 캐릭터의 Rigidbody 컴포넌트
    private Transform cameraTransform;      // 메인 카메라의 Transform
    private rotateToMouse rotateToMouse;    // 마우스 입력 캐릭터 회전

    // 물에 젖은 상태 관리
    private bool isWet = false;            // 물에 젖었는지 여부
    private float originalMoveSpeed;       // 원래 걷기 속도
    private float originalSprintSpeed;     // 원래 달리기 속도
    private float originalJumpForce;       // 원래 점프 힘

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정 
        Cursor.visible = false; // 마우스 커서를 숨김

        // 마우스 컨트롤 
        rotateToMouse = GetComponent<rotateToMouse>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 캐릭터 회전이 물리적으로 영향을 받지 않도록 설정
        cameraTransform = Camera.main.transform;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 충돌 감지 모드 설정

        // 원래 속도와 점프력 저장
        originalMoveSpeed = moveSpeed;
        originalSprintSpeed = sprintSpeed;
        originalJumpForce = jumpForce;
    }

    void Update()
    {
        MoveCharacter();  // 이동
        UpdateRotate();   // 캐릭터 회전
        HandleJump();     // 점프
        CheckGrounded();  // 땅에 닿아 있는지 감지
    }

    void MoveCharacter() // 캐릭터 이동
    {
        float horizontal = Input.GetAxis("Horizontal"); // A, D
        float vertical = Input.GetAxis("Vertical");     // W, S
        // 달리기
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        // 이동 방향 계산
        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
        moveDirection.y = 0f; // 땅에 붙어 있도록 유지

        // 목표 속도 계산
        Vector3 targetVelocity = moveDirection * currentSpeed;
        Vector3 currentVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // 이동 입력이 없으면 즉시 속도를 0으로 설정
        if (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0); // Y 속도 유지 (중력/점프 영향)
        }
        else
        {
            // 목표 속도까지 부드럽게 변경
            Vector3 newVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
        }
    }

    void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X"); // 마우스 좌우
        float mouseY = Input.GetAxis("Mouse Y"); // 마우스 상하
        rotateToMouse.UpdateRotate(mouseX, mouseY); // 회전 적용
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 위쪽 방향으로 힘을 가해 점프
            isGrounded = false; // 점프 후 공중 상태로 변경
        }
    }

    void CheckGrounded()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // 캐릭터 위치에서 살짝 위쪽에서 Ray 시작
        Ray ray = new Ray(rayOrigin, Vector3.down); // 아래 방향으로 Ray 발사
        RaycastHit hit;

        // Ray가 groundCheckDistance 범위 내에서 충돌하면 땅에 있는 것으로 판단
        if (Physics.Raycast(ray, out hit, groundCheckDistance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    // 물에 닿았을 때 호출
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            Debug.Log("물에 닿음 - 속도와 점프력 감소 시작");
            StartWetEffect();
        }
    }

    // 물에 젖은 효과 적용
    private void StartWetEffect()
    {
        if (isWet) return; // 이미 젖은 상태라면 중복 실행 방지

        isWet = true;

        // 속도와 점프력을 절반으로 줄임
        moveSpeed = originalMoveSpeed * 0.5f;
        sprintSpeed = originalSprintSpeed * 0.5f;
        jumpForce = originalJumpForce * 0.5f;

        // 5초 후 원래 값으로 복구
        StartCoroutine(ResetWetEffect());
    }

    // 5초 후 원래 속도와 점프력으로 복구
    private IEnumerator ResetWetEffect()
    {
        yield return new WaitForSeconds(5f);

        // 원래 값으로 복구
        moveSpeed = originalMoveSpeed;
        sprintSpeed = originalSprintSpeed;
        jumpForce = originalJumpForce;

        isWet = false;
        Debug.Log("물에 젖은 효과 종료 - 속도와 점프력 복구");
    }

    // 에디터에서 땅 감지 Ray 시각적으로 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);
    }
}