using UnityEngine;
using TMPro;

public class InteractManager : MonoBehaviour
{
    public Transform cameraMount;
    private Transform raycastPosition;

    public float detectionRange = 10f;
    public TMP_Text descriptionText;

    [SerializeField]
    private bool isPickable;

    private GameObject detectedObject;
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
        DetectObject();
        heldObject = pickUpController.heldObject; // PickUpController에서 들고 있는 물체 정보를 가져옴
    }
    private void DetectObject()
    {
        detectedObject = null;
        RaycastHit hit;

        if (raycastPosition == null)
            return;

        if (Physics.Raycast(raycastPosition.position, raycastPosition.forward, out hit, detectionRange))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Pickable"))
            {
                if (heldObject == null)
                {
                    detectedObject = hitObject;
                    pickUpController.detectedObject = detectedObject;
                    isPickable = true;

                    if (descriptionText != null)
                    {
                        descriptionText.enabled = true;
                        descriptionText.text = "Press F to Pick Up";
                    }
                }
            }
            else if (hitObject.CompareTag("Interactable"))
            {
                detectedObject = hitObject;
                isPickable = false;

                if (descriptionText != null)
                {
                    descriptionText.enabled = true;
                    descriptionText.text = "Press F to Interact";
                }
            }
        }
        else
        {
            detectedObject = null;
            pickUpController.detectedObject = null;
            isPickable = false;

            if (descriptionText != null && heldObject == null)
            {
                descriptionText.enabled = false;
                descriptionText.text = "";
            }

            Debug.Log("감지 X : 상태 초기화 완료");
        }
        if (detectedObject == null && heldObject == null)
        {
            if (descriptionText != null)
            {
                descriptionText.enabled = false;
                descriptionText.text = "";
            }
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
        if (heldObject != null)
        {
            pickUpController.HandlePickUpOrDrop(); 
            animator.SetTrigger(IsPutName);
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