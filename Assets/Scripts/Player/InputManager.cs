using Photon.Pun;
using UnityEngine;

public class InputManager : MonoBehaviourPun
{
    private PlayerPushController pushController;
    private PickUpController pickUpController;
    private RotateToMouse rotateToMouse;
    private InteractManager interactManager;
    public Animator animator;
    public string throwTriggerName = "IsThrow";
    public string PushTriggerName = "IsPush";
    private GameObject MiniMap;

    private void Awake()
    {
        if (GameStateManager.isServerTest && !photonView.IsMine) return;
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
        MiniMap = GameObject.Find("MiniMap"); //Find 함수는 활성 오브젝트만 찾을 수 있다.
        if (MiniMap != null)
        {
            MiniMap.SetActive(false);
        }
    }

    void Update()
    {
        if (GameStateManager.isServerTest && !photonView.IsMine) return;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("lift")
        || animator.GetCurrentAnimatorStateInfo(0).IsName("lift Reverse")
        || animator.GetCurrentAnimatorStateInfo(0).IsName("Falling")
        || animator.GetCurrentAnimatorStateInfo(0).IsName("Fall Impact")
        || animator.GetCurrentAnimatorStateInfo(0).IsName("Getting Up"))
        {
            return;
        }
        UpdateRotate();
        if (Input.GetKeyDown(KeyCode.F)) interactManager.OnInput();

        if (Input.GetMouseButtonDown(0))
        {
            if (pickUpController.IsHoldingObject())
            {
                pickUpController.ThrowObject();
                animator.SetTrigger(throwTriggerName);
            }
            else if (pushController.CanPush())
            {
                pushController.PushPlayer();
                animator.SetTrigger(PushTriggerName); 
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) pickUpController.RotateHeldObject();

        // Tab 키 : 미니맵 On Off
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (MiniMap.activeSelf)
            {
                MiniMap.SetActive(false);
            }
            else
            {
                MiniMap.SetActive(true);
            }
        }
    }

    void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }
}