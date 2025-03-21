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

    private float originalMoveSpeed; // 원래 이동 속도
    private float originalSprintSpeed; // 원래 달리기 속도
    private bool isAffectedByWind = false; // 바람의 영향을 받는지 여부
    private Vector3 currentWindDirection; // 현재 바람 방향
    private float currentWindForce; // 현재 바람 세기

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
        originalMoveSpeed = moveSpeed;
        originalSprintSpeed = sprintSpeed;
    }

    void Update()
    {
        MoveCharacter();  // 이동
        UpdateRotate();   // 캐릭터 회전
        HandleJump();     // 점프
        CheckGrounded();  // 땅에 닿아 있는지 감지
    }

    void MoveCharacter()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
        moveDirection.y = 0f;

        Vector3 targetVelocity = moveDirection * currentSpeed;
        Vector3 currentVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
        else
        {
            Vector3 newVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
        }

        if (isAffectedByWind)
        {
            rb.AddForce(currentWindDirection.normalized * currentWindForce, ForceMode.Force);
        }
    }

    void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void CheckGrounded()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, groundCheckDistance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);
    }

    public void ApplyWindEffect(Vector3 windDirection, float windForce)
    {
        // 플레이어의 현재 이동 방향
        Vector3 playerDirection = rb.velocity.normalized;

        // 바람 방향과 플레이어 방향의 각도 계산
        float angle = Vector3.Angle(playerDirection, windDirection);

        // 각도에 따른 힘의 크기 조절
        float forceMagnitude = windForce * Mathf.Cos(angle * Mathf.Deg2Rad);

        // 바람 방향으로 힘 적용
        rb.AddForce(windDirection.normalized * forceMagnitude, ForceMode.Force);

        Debug.Log("바람에 맞았다");
    }

    public void RemoveWindEffect()
    {
        isAffectedByWind = false;
        currentWindDirection = Vector3.zero;
        currentWindForce = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WindArea"))
        {
            WindController windController = other.GetComponent<WindController>();
            if (windController != null)
            {
                ApplyWindEffect(windController.windDirection, windController.windForce);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WindArea"))
        {
            RemoveWindEffect();
        }
    }
}