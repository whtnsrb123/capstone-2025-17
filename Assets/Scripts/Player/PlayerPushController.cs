using UnityEngine;
using TMPro;
using System.Collections;
using Photon.Pun;

public class PlayerPushController : MonoBehaviourPun
{
    private float pushForce = 3f;
    private Rigidbody targetPlayerRb;

    private bool canPush = false;
    [SerializeField] private Transform cameraMount;
    [SerializeField] private float detectionRange = 0.5f; // 감지 거리
    [SerializeField] private TMP_Text pushUI;

    private bool isPushing = false;
    [SerializeField] private float pushDelay = 1f; // 밀치기 쿨타임

    void Start()
    {
        if (pushUI != null) pushUI.enabled = false;
        if (cameraMount == null && Camera.main != null)
            cameraMount = Camera.main.transform;
    }

    void Update()
    {
        if (GameStateManager.isServerTest && !photonView.IsMine) return;
        DetectPlayer();
    }

    void DetectPlayer()
    {
        targetPlayerRb = null;
        canPush = false;

        RaycastHit hit;
        if (Physics.Raycast(cameraMount.position, cameraMount.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player") && hit.collider.gameObject != gameObject)
            {
                targetPlayerRb = hit.collider.GetComponent<Rigidbody>();
                if (targetPlayerRb != null)
                {
                    canPush = true;
                    pushUI.enabled = true;
                    pushUI.text = "Left Click to Push";
                    Debug.Log("플레이어 감지됨: " + hit.collider.gameObject.name);
                }
            }
        }

        if (!canPush && pushUI != null)
        {
            pushUI.enabled = false;
        }
    }

    public void PushPlayer()
    {
        if (!canPush || isPushing || targetPlayerRb == null) return;

        PhotonView targetView = targetPlayerRb.GetComponent<PhotonView>();
        if (targetView == null) return;

        Vector3 pushDir = (targetPlayerRb.transform.position - transform.position).normalized;

        // 대상에게 자기 자신을 밀도록 요청 (소유자에게만 RPC)
        targetView.RPC(nameof(RPC_ReceivePush), targetView.Owner, pushDir);

        isPushing = true;
        StartCoroutine(ResetPushing());

        Debug.Log("밀치기 RPC 전송 완료: " + targetView.name);
    }

    [PunRPC]
    public void RPC_ReceivePush(Vector3 pushDirection)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            Debug.Log("밀림: " + gameObject.name);
        }
    }

    public bool CanPush()
    {
        return canPush && !isPushing;
    }

    IEnumerator ResetPushing()
    {
        yield return new WaitForSeconds(pushDelay);
        isPushing = false;
    }
}
