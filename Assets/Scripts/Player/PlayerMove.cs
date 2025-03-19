using UnityEngine;

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

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; // 게임 시작 시 마우스 커서를 화면 중앙에 고정 
        Cursor.visible = false; // 마우스 커서를 숨김

        // 마우스 회전 관련 컴포넌트 가져오기
        rotateToMouse = GetComponent<rotateToMouse>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 캐릭터 회전이 물리적으로 영향을 받지 않도록 설정
        cameraTransform = Camera.main.transform; // 메인 카메라의 Transform 가져오기
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 충돌 감지 모드 설정
    }

    void Update()
    {
        MoveCharacter();  // 캐릭터 이동
        UpdateRotate();   // 마우스 입력 캐릭터 회전
        HandleJump();     // 점프
        CheckGrounded();  // 땅에 닿아 있는지 감지
    }

    
    void MoveCharacter()// 캐릭터 이동
    {
        float horizontal = Input.GetAxis("Horizontal"); // A, D 좌우 입력 값
        float vertical = Input.GetAxis("Vertical");     // W, S 상하 입력 값

        // 달리기
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        // 이동 방향 계산
        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
        moveDirection.y = 0f; // 수직 이동 제거 (땅에 붙어 있도록 유지)

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
            // 가속도를 적용하여 목표 속도까지 부드럽게 변경
            Vector3 newVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
        }
    }

    // 마우스 입력을 받아 캐릭터 회전 처리
    void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X"); // 마우스 좌우 이동 입력
        float mouseY = Input.GetAxis("Mouse Y"); // 마우스 상하 이동 입력
        rotateToMouse.UpdateRotate(mouseX, mouseY); // 회전 적용
    }

    // 점프 처리
    void HandleJump()
    {
        // 스페이스바를 누르고 땅에 닿아 있을 때 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 위쪽 방향으로 힘을 가해 점프
            isGrounded = false; // 점프 후 공중 상태로 변경
        }
    }

    // 땅에 닿아 있는지 감지
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

    // 에디터에서 땅 감지 Ray 시각적으로 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);
    }
}
