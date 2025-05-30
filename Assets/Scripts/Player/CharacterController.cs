using Photon.Pun;
using UnityEngine;
using System.Collections;
using Photon.Pun;


public class CharacterController : MonoBehaviourPun
{
    [SerializeField] private float moveSpeed = 2f;              // 걷기 속도
    [SerializeField] private float sprintSpeed = 3f;             // 달리기 속도
    [SerializeField] private float jumpForce = 3f;                // 점프 힘
    private float groundCheckDistance = 0.4f;                  // 땅 감지 거리
    private float acceleration = 20f;                                       // 이동 가속도

    private bool isGrounded;                                // 캐릭터가 땅에 닿아 있는지 여부
    private Rigidbody rb;
    private RotateToMouse rotateToMouse;
    private Transform cameraTransform;

    // 물에 젖은 상태 관리
    private bool isWet = false;            // 물에 젖었는지
    private float originalMoveSpeed;       // 원래 걷기 속도
    private float originalSprintSpeed;     // 원래 달리기 속도
    private float originalJumpForce;       // 원래 점프 힘

    // 물 파티클 시스템 (파이프 오브젝트에서 발생하는 파티클)
    [SerializeField] private ParticleSystem waterEffect;     // 물 파티클 시스템

    // 물에 닿은 시간
    private float wetTime = 0f;            // 물에 닿은 시간을 기록
    private const float wetDuration = 5f;  // 물에 닿은 효과 지속 시간 (5초)

    // 속도 감소 관련 변수
    private float slowDownFactor = 0.5f; // 속도 감소 비율 (0.5 = 절반으로 감소)
    private float slowDownDuration = 3f;  // 속도 감소 지속 시간 (3초)
    private bool isSlowedDown = false;    // 속도 감소 상태 여부
    private float slowDownTimer = 0f;     // 속도 감소 타이머

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    public Animator animator; // 애니메이터 컴포넌트 연결
    public int MoveType = 0; // MoveType 변수 선언 및 초기화
    public string jumpTriggerName = "IsJump"; // 점프 트리거 이름
    public const string carryJumpTriggerName = "IsCarryJump"; // 점프 트리거 이름
    public string fallTriggerName = "IsFall";
    public string fallingImpactTriggerName = "IsFallingImpact";

    private float jumpStartTime = -0.5f; // 점프 시작 시간 저장
    private float jumpTimeout = 0.5f; // 1초 이내에 착지

    private void Awake()
    {
        if (GameStateManager.isServerTest)
        {
            if (!photonView.IsMine) return;
        }
        // 마우스 커서를 잠그고 보이지 않게 설정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 마우스 회전 관련 컴포넌트 가져오기
        rotateToMouse = GetComponent<RotateToMouse>();
    }


    void Start()
    {
        if (GameStateManager.isServerTest)
        {
            if (!photonView.IsMine) return;
        }

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 캐릭터 회전이 물리적으로 영향을 받지 않도록 설정
        cameraTransform = Camera.main.transform; // 메인 카메라의 Transform 가져오기
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 충돌 감지 모드 설정

        // 원래 속도와 점프력 저장
        originalMoveSpeed = moveSpeed;
        originalSprintSpeed = sprintSpeed;
        originalJumpForce = jumpForce;

        // 물 파티클 시스템을 비활성화하지 않음. 파이프에서 계속 흐름.
        if (waterEffect != null && !waterEffect.isPlaying)
        {
            waterEffect.Play();  // 물 파티클을 시작 (파이프에서 물 계속 나옴)
        }

        ps = waterEffect; // 파티클 시스템 컴포넌트 할당
        particles = new ParticleSystem.Particle[ps.main.maxParticles];

        // 애니메이터 컴포넌트 가져오기
        animator = GetComponent<Animator>();

        lastYPosition = transform.position.y; // y값 초기화
    }

    private bool wasGroundedLastFrame = true;
    private bool isFallingAnimPlayed = false;
    private bool isJumping = false;
    private float airborneStartTime = 0f; // 공중에 떠있는 시작 시간 저장

    private float lastYPosition = 0f; // 이전 프레임 y값 저장
    private float yPositionThreshold = 0.01f; // y값 변화 허용 오차

    void Update()
    {
        if (animator == null)
        {
            Debug.LogError("Animator가 할당되지 않았습니다.");
            animator = GetComponent<Animator>();
            return;
        }
        CheckGrounded();  // 땅에 닿아 있는지 감지

        // 공중에 처음 뜨면 시간 저장
        if (!isGrounded && wasGroundedLastFrame) airborneStartTime = Time.time;
        // 공중에 떠있는 시간 계산
        float timeInAir = Time.time - airborneStartTime;

        // y값 변화량 계산
        float deltaY = Mathf.Abs(transform.position.y - lastYPosition);

        // 낙하 애니메이션 트리거 0.7초 이상 공중에 떠있고, y속도가 음수이며, y위치가 충분히 변할 때만 낙하모션
        if (!isGrounded && timeInAir > 0.7f && rb.velocity.y < 0
            && deltaY > yPositionThreshold
            && !isJumping && !isFallingAnimPlayed)
        {
            animator.SetTrigger(fallTriggerName);
            isFallingAnimPlayed = true;
        }

        // 중력에 의해 떨어지고 있지 않으면 낙하 애니메이션 종료
        if (isFallingAnimPlayed && rb.velocity.y >= 0)
        {
            animator.ResetTrigger(fallTriggerName);
            isFallingAnimPlayed = false;
        }

        // 트리거 초기화
        if (isGrounded && !wasGroundedLastFrame)
        {
            animator.ResetTrigger(fallTriggerName);
            animator.ResetTrigger(fallingImpactTriggerName);
            if (timeInAir > 0.5f)
            {
                animator.SetTrigger(fallingImpactTriggerName);
            }
            isFallingAnimPlayed = false;
            isJumping = false;
        }
        wasGroundedLastFrame = isGrounded;

        lastYPosition = transform.position.y; // y값 저장

        if (GameStateManager.isServerTest && !photonView.IsMine) return;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("lift")
        || animator.GetCurrentAnimatorStateInfo(0).IsName("lift Reverse")
        || animator.GetCurrentAnimatorStateInfo(0).IsName("Fall Impact")
        || animator.GetCurrentAnimatorStateInfo(0).IsName("Getting Up"))
        {
            return;
        }
        MoveCharacter();  // 이동
        HandleJump();     // 점프

        // 물에 닿은 상태일 때 계속 효과가 적용되도록 유지
        if (isWet)
        {
            wetTime -= Time.deltaTime; // 1초마다 타이머 감소
            if (wetTime <= 0f)
            {
                ResetWetEffect();  // 효과 종료
            }
        }

        // 속도 감소 상태일 때 타이머 감소 및 효과 종료 처리
        if (isSlowedDown)
        {
            slowDownTimer -= Time.deltaTime;
            if (slowDownTimer <= 0f)
            {
                ResetSpeed(); // 원래 속도로 복구
            }
        }
    }

    // 캐릭터 이동 처리
    void MoveCharacter()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A, D 또는 좌우 방향키 입력 값
        float vertical = Input.GetAxis("Vertical");     // W, S 또는 상하 방향키 입력 값

        // 왼쪽 Shift 키를 누르면 달리기 속도로 이동
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        // 이동 방향 계산 (앞뒤 + 좌우 이동)
        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
        moveDirection.y = 0f; // 수직 이동 제거 (땅에 붙어 있도록 유지)

        // 목표 속도 계산
        Vector3 targetVelocity = moveDirection * currentSpeed;
        Vector3 currentVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // 이동 입력이 없으면 즉시 속도를 0으로 설정 (부드러운 정지)
        if (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0); // Y 속도 유지 (중력/점프 영향)
            animator.SetInteger("MoveType", 0); // 멈춤 상태
        }
        else
        {
            Vector3 newVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetInteger("MoveType", 2); // 달리기 상태
            }
            else
            {
                animator.SetInteger("MoveType", 1); // 걷기 상태
            }
        }
        if (!isGrounded)
        {
            animator.SetInteger("MoveType", 0); // 낙하 중엔 이동 애니메이션 무력화
            return;
        }
    }


    void HandleJump() //점프
    {
        // 스페이스바를 누르고 땅에 닿아 있을 때 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 위쪽 방향으로 힘을 가해 점프
            isGrounded = false; // 점프 후 공중 상태로 변경
            isJumping = true;
            jumpStartTime = Time.time; // 점프한 시간 기록
            bool isHolding = GetComponent<PickUpController>().IsHoldingObject();
            if (isHolding)
            {
                animator.SetTrigger(carryJumpTriggerName); // carry 점프 트리거 활성화
            }
            else
            {
                animator.SetTrigger(jumpTriggerName); // 점프 트리거 활성화
            }
        }
    }

    void CheckGrounded() // 바닥에 닿아있는지 (점프 관련)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // 캐릭터 위치에서 살짝 위쪽에서 Ray 시작
        Ray ray = new Ray(rayOrigin, Vector3.down); // 아래 방향으로 Ray 발사
        RaycastHit hit;

        // Ray가 groundCheckDistance 범위 내에서 충돌하면 땅에 있는 것으로 판단
        if (Physics.Raycast(ray, out hit, groundCheckDistance))
        {
            isGrounded = true;
            isJumping = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    // 파티클 충돌 처리
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("OnParticleCollision 호출됨: " + other.name);

        if (other.CompareTag("WaterParticle"))
        {
            Debug.Log("WaterParticle 태그 감지됨!");

            StartWetEffect(); // 젖은 상태 효과 시작
            SlowDownSpeed();  // 속도 감소 효과 적용
        }
    }

    // 물에 젖은 효과 적용
    private void StartWetEffect()
    {
        if (isWet)
        {
            wetTime = wetDuration;
            Debug.Log("계속 물에 맞고 있음");
            return;
        }

        isWet = true;
        wetTime = wetDuration;

        // 속도와 점프력을 절반으로 줄임
        moveSpeed = originalMoveSpeed * 0.5f;
        sprintSpeed = originalSprintSpeed * 0.5f;
        jumpForce = originalJumpForce * 0.5f;

        Debug.Log("처음 물에 맞음");
    }

    // 물에 젖은 효과 종료
    private void ResetWetEffect()
    {
        // 원래 값으로 복구
        moveSpeed = originalMoveSpeed;
        sprintSpeed = originalSprintSpeed;
        jumpForce = originalJumpForce;

        isWet = false;
        Debug.Log("물에 젖은 효과 종료 - 속도와 점프력 복구");
    }

    // 속도 감소 효과 적용
    private void SlowDownSpeed()
    {
        if (isSlowedDown) return; // 이미 속도 감소 상태라면 중복 실행 방지

        isSlowedDown = true;
        slowDownTimer = slowDownDuration;

        // 속도 감소
        moveSpeed *= slowDownFactor;
        sprintSpeed *= slowDownFactor;
        jumpForce *= slowDownFactor;

        if (animator != null)
            animator.speed = 0.5f;
    }

    // 원래 속도로 복구
    private void ResetSpeed()
    {
        // 원래 속도로 복구
        moveSpeed = originalMoveSpeed;
        sprintSpeed = originalSprintSpeed;
        jumpForce = originalJumpForce;

        isSlowedDown = false;
        if (animator != null)
            animator.speed = 1f;
    }

    // 에디터에서 땅 감지 Ray 시각
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);
    }
}
