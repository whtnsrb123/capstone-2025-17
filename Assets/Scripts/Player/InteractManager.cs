using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class InteractManager : MonoBehaviourPun
{
    public Transform cameraMount;
    private Transform raycastPosition;

    public float detectionRange = 10f;
    public TMP_Text descriptionText;

    [SerializeField]
    private bool isPickable;

    private GameObject detectedObject;
    private GameObject previousDetectedObject;
    private GameObject heldObject;

    private PickUpController pickUpController;
    private PlayerPushController pushController;
    private InteractController interactController;


    public Animator animator; 
    public string liftTriggerName = "IsLift";
    public string IsPutName = "IsPut";

    void Start()
    {
        pickUpController = GetComponent<PickUpController>();
        interactController = GetComponent<InteractController>();


        if (cameraMount == null && Camera.main != null)
            cameraMount = Camera.main.transform;

        raycastPosition = cameraMount;

        if (descriptionText != null)
            descriptionText.enabled = false;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameStateManager.isServerTest && !photonView.IsMine) return;
        DetectObject();
        heldObject = pickUpController.heldObject; // PickUpController에서 들고 있는 물체 정보를 가져옴
    }
    private void DetectObject()
    {
        RaycastHit hit;
        GameObject hitObject = null;

        if (raycastPosition == null)
            return;

        if (Physics.Raycast(raycastPosition.position, raycastPosition.forward, out hit, detectionRange))
        {
            hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Pickable") && heldObject == null)
            {
                detectedObject = hitObject;
                pickUpController.detectedObject = detectedObject;
                isPickable = true;

                SetDescription("Press F to Pick Up");
            }
            else if (hitObject.CompareTag("Interactable"))
            {
                detectedObject = hitObject;
                isPickable = false;

                SetDescription("Press F to Interact");
            }
            else
            {
                detectedObject = null;
                isPickable = false;
                pickUpController.detectedObject = null;
            }
        }
        else
        {
            detectedObject = null;
            isPickable = false;
            pickUpController.detectedObject = null;
        }

        if (previousDetectedObject != detectedObject)
        {
            if (previousDetectedObject != null)
            {
                Outline prevOutline = previousDetectedObject.GetComponent<Outline>();
                if (prevOutline != null)
                    prevOutline.enabled = false;
            }

            if (detectedObject != null)
            {
                Outline newOutline = detectedObject.GetComponent<Outline>();
                if (newOutline != null)
                    newOutline.enabled = true;
            }

            previousDetectedObject = detectedObject;
        }

        // 텍스트 비활성화 조건
        if (detectedObject == null && heldObject == null && descriptionText != null)
        {
            descriptionText.enabled = false;
            descriptionText.text = "";
        }
    }


    private void SetDescription(string text)
    {
        if (descriptionText != null && heldObject == null)
        {
            descriptionText.enabled = true;
            descriptionText.text = text;
        }
    }
    public void OnInput()
    {
        if(GameStateManager.isServerTest)
        {
            photonView.RPC(nameof(RPC_OnInput), RpcTarget.All);
        }
        else
        {
            RPC_OnInput();
        }
    }
    [PunRPC]
    public void RPC_OnInput()
    {
        if (heldObject != null)
        {
            pickUpController.HandlePickUpOrDrop(); 
            animator.SetTrigger(IsPutName);
            if (FindObjectOfType<BatteryBox>() != null)
            {
                if (detectedObject.GetComponent<BatteryBox>() != null)
                {
                    detectedObject.GetComponent<BatteryBox>().Interact(gameObject);
                    return;
                }
            }
            
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
            animator.SetTrigger(liftTriggerName); 
        }
        else if (isPickable == false)
        {
            interactController.Interact(detectedObject);
        }
    }
    public void ResetDetection()
    {
        detectedObject = null;
        isPickable = false;

        if (descriptionText != null)
        {
            descriptionText.enabled = false;
            descriptionText.text = "";
        }

        Debug.Log("감지 상태 초기화됨");
    }
}