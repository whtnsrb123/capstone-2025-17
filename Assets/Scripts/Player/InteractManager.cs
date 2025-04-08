using UnityEngine;
using TMPro;

public class InteractManager : MonoBehaviour
{
    public Transform raycastPosition;
    public float detectionRange = 10f;
    public TMP_Text descriptionText;

    [SerializeField]
    private bool isPickable;

    private GameObject detectedObject;
    private GameObject heldObject;

    private PickUpController pickUpController;
    private InteractController interactController;

    private Outline targetOutline;

    void Start()
    {
        pickUpController = GetComponent<PickUpController>();
        interactController = GetComponent<InteractController>();

        Camera mainCamera = gameObject.GetComponentInChildren<Camera>();
        raycastPosition = mainCamera.transform;

        if(descriptionText != null)
            descriptionText.enabled = false;
    }

    void Update()
    {
        DetectObject();
    }

    private void DetectObject()
    {
        detectedObject = null;
        RaycastHit hit;

        if(raycastPosition == null)
            return;

        if(Physics.Raycast(raycastPosition.position, raycastPosition.forward, out hit, detectionRange))
        {
            detectedObject = hit.collider.gameObject;

            if(hit.collider.CompareTag("Pickable"))
            {
                if (descriptionText != null && heldObject == null)
                {
                    descriptionText.enabled = true;
                    descriptionText.text = "Press F to Pick Up";
                    isPickable = true;
                    pickUpController.detectedObject = detectedObject;
                }
            }
            else if(hit.collider.CompareTag("Interactable"))
            {
                if(descriptionText != null)
                {
                    descriptionText.enabled = true;
                    descriptionText.text = "Press F to Interact";
                    isPickable = false;
                }
            }

            Outline outline = hit.collider.gameObject.GetComponent<Outline>();
            if(outline != null)
            {
                if(targetOutline != outline)
                {
                    ClearOutline();
                    targetOutline = outline;
                    targetOutline.enabled = true;
                }
            } else {
                ClearOutline();
            }
        } else {
            ClearOutline();
        }
        
        if (detectedObject == null && heldObject == null && descriptionText != null)
            descriptionText.enabled = false;
    }

    private void ClearOutline()
    {
        if(targetOutline != null)
        {
            targetOutline.enabled = false;
            targetOutline = null;
        }
    }

    public void OnInput()
    {
        if(detectedObject == null)
            return;

        if(isPickable)
            pickUpController.HandlePickUpOrDrop();
        else
            interactController.Interact(detectedObject);
    }
}
