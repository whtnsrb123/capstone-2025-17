using UnityEngine;
using TMPro;

public class InteractManager : MonoBehaviour
{
    public Transform raycastPosition;
    public float detectionRange = 10f;
    public TMP_Text descriptionText;
    private bool canpush = false;
    [SerializeField]
    private bool isPickable;

    private GameObject detectedObject;
    private GameObject heldObject;

    private PickUpController pickUpController;
    private PlayerPushController pushController;
    private InteractController interactController;

    // 애니메이터 관련 변수
    public Animator animator; // 애니메이터 컴포넌트 연결
    public string liftTriggerName = "IsLift"; // 애니메이터 트리거 이름

    void Start()
    {
        pickUpController = GetComponent<PickUpController>();
        interactController = GetComponent<InteractController>();

        Camera mainCamera = Camera.main;
        raycastPosition = mainCamera.transform;

        if (descriptionText != null)
            descriptionText.enabled = false;

        // 애니메이터 컴포넌트 가져오기
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        DetectObject();
        heldObject = pickUpController.heldObject; // PickUpController에서 들고 있는 물체 정보를 가져옴
    }

    private void DetectObject()
    {
        canpush = false;
        detectedObject = null;
        RaycastHit hit;

        if (raycastPosition == null)
            return;

        if (Physics.Raycast(raycastPosition.position, raycastPosition.forward, out hit, detectionRange))
        {
            detectedObject = hit.collider.gameObject;

            if (hit.collider.CompareTag("Pickable"))
            {
                Debug.Log("Pickable 감지: " + hit.collider.gameObject.name);
                if (descriptionText != null && heldObject == null)
                {
                    descriptionText.enabled = true;
                    descriptionText.text = "Press F to Pick Up";
                    isPickable = true;
                    pickUpController.detectedObject = detectedObject;
                }
            }
            else if (hit.collider.CompareTag("Interactable"))
            {
                Debug.Log("Interactable 감지: " + hit.collider.gameObject.name);
                if (descriptionText != null)
                {
                    descriptionText.enabled = true;
                    descriptionText.text = "Press F to Interact";
                    isPickable = false;
                }
            }
        }

        if (detectedObject == null && heldObject == null && descriptionText != null)
            descriptionText.enabled = false;
    }

    public void OnInput()
    {
        if (heldObject != null) // 들고 있는 물체가 있다면
        {
            pickUpController.HandlePickUpOrDrop(); // 물체를 내려놓습니다.
            if (isPickable) // 들 수 있는 물체였다면
            {
                animator.SetTrigger("IsPut"); // IsPut 트리거 활성화
            }
            return;
        }

        if (detectedObject == null)
            return;

        if (isPickable)
        {
            pickUpController.HandlePickUpOrDrop();
            animator.SetTrigger(liftTriggerName); // PickUp 시 애니메이터 트리거 활성화
        }
        else if (isPickable == false)
        {
            interactController.Interact(detectedObject);
        }
    }
}