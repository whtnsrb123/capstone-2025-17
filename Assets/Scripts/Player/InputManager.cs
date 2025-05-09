using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerPushController pushController;
    private PickUpController pickUpController;
    private RotateToMouse rotateToMouse;
    private InteractManager interactManager;
    public Animator animator;
    public string throwTriggerName = "IsThrow";

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정 
        Cursor.visible = false; // 마우스 커서를 숨김
        rotateToMouse = GetComponent<RotateToMouse>(); // 마우스 컨트롤 
    }

    void Start()
    {
        pushController = GetComponent<PlayerPushController>();
        pickUpController = GetComponent<PickUpController>();
        interactManager = GetComponent<InteractManager>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("lift")
        || animator.GetCurrentAnimatorStateInfo(0).IsName("lift Reverse"))
        {
            return;
        }
        UpdateRotate();   // 캐릭터 회전
        // F 키: 물체 잡기/놓기
        if (Input.GetKeyDown(KeyCode.F))
        {
            interactManager.OnInput();
        }

        // 마우스 왼쪽 클릭: 밀치기 또는 물체 던지기
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("마우스 왼쪽 클릭 감지됨"); // 디버그 로그 추가
            if (pickUpController.IsHoldingObject())
            {
                Debug.Log("물체를 들고 있음 -> 던지기 시도");
                pickUpController.ThrowObject();
                animator.SetTrigger(throwTriggerName); // 던지기 트리거 활성화
            }
            else if (pushController.CanPush())
            {
                Debug.Log("밀치기 가능 -> 푸쉬 시도");
                pushController.PushPlayer();
            }
        }

        // R 키: 물체 회전
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R 키 입력 감지됨");
            pickUpController.RotateHeldObject();
        }
    }

    void UpdateRotate() // 화면 회전 (마우스)
    {
        float mouseX = Input.GetAxis("Mouse X"); // 마우스 좌우
        float mouseY = Input.GetAxis("Mouse Y"); // 마우스 상하
        rotateToMouse.UpdateRotate(mouseX, mouseY); // 회전 적용
    }
}