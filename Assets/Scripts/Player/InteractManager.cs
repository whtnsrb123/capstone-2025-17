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

    void Start()
    {
        pickUpController = GetComponent<PickUpController>();
        interactController = GetComponent<InteractController>();

        Camera mainCamera = Camera.main;
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
        }

        if (detectedObject == null && heldObject == null && descriptionText != null)
            descriptionText.enabled = false;
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
