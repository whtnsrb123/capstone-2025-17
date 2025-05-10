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
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) pickUpController.RotateHeldObject();
    }

    void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }
}