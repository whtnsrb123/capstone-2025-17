using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Player_Push_Controller pushController;
    private PickUpController pickUpController;
    private RotateToMouse rotateToMouse;    // 마우스 입력 캐릭터 회전
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정 
        Cursor.visible = false; // 마우스 커서를 숨김

        // 마우스 컨트롤 
        rotateToMouse = GetComponent<RotateToMouse>();
    }
    void Start()
    {
        pushController = GetComponent<Player_Push_Controller>();
        pickUpController = GetComponent<PickUpController>();
    }

    void Update()
    {
        UpdateRotate();   // 캐릭터 회전
        // F 키: 물체 잡기/놓기
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F 키 입력 감지됨"); // 디버그 로그 추가
            pickUpController.HandlePickUpOrDrop();
        }

        // 마우스 왼쪽 클릭: 밀치기 또는 물체 던지기
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("마우스 왼쪽 클릭 감지됨"); // 디버그 로그 추가
            if (pickUpController.IsHoldingObject())
            {
                Debug.Log("물체를 들고 있음 -> 던지기 시도");
                pickUpController.ThrowObject();
            }
            else if (pushController.CanPush())
            {
                Debug.Log("밀치기 가능 -> 푸쉬 시도");
                pushController.PushPlayer();
            }
            else
            {
                Debug.Log("밀치기 불가능: canPush = " + pushController.CanPush());
            }
        }
        void UpdateRotate() // 화면 회전 (마우스)
        {
            float mouseX = Input.GetAxis("Mouse X"); // 마우스 좌우
            float mouseY = Input.GetAxis("Mouse Y"); // 마우스 상하
            rotateToMouse.UpdateRotate(mouseX, mouseY); // 회전 적용
        }

        // R 키 물체 회전
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R 키 입력 감지됨");
            pickUpController.RotateHeldObject();
        }
    }
}