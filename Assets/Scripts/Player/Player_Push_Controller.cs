using UnityEngine;
using UnityEngine.UI;

public class Player_Push_Controller : MonoBehaviour
{
<<<<<<< Updated upstream
    // Start is called before the first frame update
    void Start()
    {
        
=======
    public float pushForce = 3f;
    private bool canPush = false;
    private Rigidbody targetPlayerRb;

    public Transform holdPosition;
    public float detectionRange = 0.5f;
    public TMP_Text pushUI;

    private PickUpController pickUpController;

    void Start()
    {
        if (pushUI != null) pushUI.enabled = false;
        if (holdPosition == null) holdPosition = transform;

        pickUpController = GetComponent<PickUpController>();
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        
    }
}
=======
        DetectPlayer();

        if (canPush && Input.GetMouseButtonDown(0)) PushPlayer();
    }

    void DetectPlayer()
    {
        targetPlayerRb = null;
        canPush = false;

        RaycastHit hit;

        if (Physics.Raycast(holdPosition.position, holdPosition.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player") && hit.collider.gameObject != gameObject)
            {
                targetPlayerRb = hit.collider.GetComponent<Rigidbody>();
                if (targetPlayerRb != null)
                {
                    canPush = true;
                    if (pushUI != null)
                    {
                        pushUI.enabled = true;
                        pushUI.text = "Left Click to Push";
                    }
                    Debug.Log("플레이어 감지됨: " + hit.collider.gameObject.name);
                }
            }
        }

        Debug.DrawRay(holdPosition.position, holdPosition.forward * detectionRange, Color.blue);

        if (targetPlayerRb == null && pushUI != null)
        {
            pushUI.enabled = false;
        }
    }

    void PushPlayer()
    {
        if (targetPlayerRb != null)
        {
            Vector3 pushDirection = (targetPlayerRb.transform.position - transform.position).normalized;
            targetPlayerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            Debug.Log("밀치기 성공");
        }
    }

}
>>>>>>> Stashed changes
