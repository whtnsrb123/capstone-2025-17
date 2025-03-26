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
    private RotateToMouse rotateToMouse;    // 마우스 입력 캐릭터 회전
    
    // 물에 젖은 상태 관리
    private bool isWet = false;            // 물에 젖었는지 여부
    private float originalMoveSpeed;       // 원래 걷기 속도
    private float originalSprintSpeed;     // 원래 달리기 속도
    private float originalJumpForce;       // 원래 점프 힘

    // 물 파티클 시스템 (파이프 오브젝트에서 발생하는 파티클)
    public ParticleSystem waterEffect;     // 물 파티클 시스템

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

        // 물 파티클 시스템을 비활성화하지 않음. 파이프에서 계속 흐름.
        if (waterEffect != null && !waterEffect.isPlaying)
        {
            waterEffect.Play();  // 물 파티클을 시작 (파이프에서 물 계속 나옴)
        }

        ps = waterEffect; // 파티클 시스템 컴포넌트 할당
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void Update()
    {
        MoveCharacter();  // 이동
        HandleJump();     // 점프
        CheckGrounded();  // 땅에 닿아 있는지 감지

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

    void HandleJump() //점프
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 위쪽 방향으로 힘을 가해 점프
            isGrounded = false; // 점프 후 공중 상태로 변경
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
        }
        else
        {
            isGrounded = false;
        }
    }

    // 파티클 충돌 처리
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("WaterParticle")) // 파티클 태그 확인
        {
            int numParticles = ps.GetParticles(particles);

            for (int i = 0; i < numParticles; i++)
            {
                if (particles[i].remainingLifetime < 0.1f) // 충돌한 파티클 찾기 (수명 활용)
                {
                    particles[i].remainingLifetime = 0f; // 파티클 수명을 0으로 설정하여 즉시 제거
                    ps.SetParticles(particles, numParticles); // 파티클 데이터 업데이트
                    break;
                }
            }

            Debug.Log("파티클 물방울과 충돌");

            SlowDownSpeed(); // 속도 감소 효과
        }
    }

    // 물에 젖은 효과 적용
    private void StartWetEffect()
    {
        if (isWet) return; // 이미 젖은 상태라면 중복 실행 방지

        isWet = true;
        wetTime = wetDuration; // 5초 타이머 시작

        // 속도와 점프력을 절반으로 줄임
        moveSpeed = originalMoveSpeed * 0.5f;
        sprintSpeed = originalSprintSpeed * 0.5f;
        jumpForce = originalJumpForce * 0.5f;
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
    }

    // 원래 속도로 복구
    private void ResetSpeed()
    {
        // 원래 속도로 복구
        moveSpeed = originalMoveSpeed;
        sprintSpeed = originalSprintSpeed;
        jumpForce = originalJumpForce;

        isSlowedDown = false;
    }

    // 에디터에서 땅 감지 Ray 시각
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);
    }
}