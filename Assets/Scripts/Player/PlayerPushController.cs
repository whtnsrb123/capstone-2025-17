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

    private bool isPushing = false; // 밀치는 중인지 여부
    [SerializeField] private float pushDelay = 1f; // 밀치기 딜레이
    public Animator animator;
    public const string PushTriggerName = "IsPush";
    

    void Start()
    {
        if (pushUI != null) pushUI.enabled = false;
        if (cameraMount == null && Camera.main != null)
            cameraMount = Camera.main.transform;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        targetPlayerRb = null; // 감지 초기화
        canPush = false;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, detectionRange))
        {
            // Player 태그 감지
            if (hit.collider.CompareTag("Player") && hit.collider.gameObject != gameObject)
            {
                targetPlayerRb = hit.collider.GetComponent<Rigidbody>();
                if (targetPlayerRb != null)
                {
                    canPush = true;
                    pushUI.enabled = true;
                    pushUI.text = "Left Click to Push"; // UI 메시지 설정

                    Debug.Log("플레이어 감지됨: " + hit.collider.gameObject.name);
                }
            }
        }


        if (targetPlayerRb == null && pushUI != null)
        {
            pushUI.enabled = false;
        }
    }

    public void PushPlayer()
    {
        if (GameStateManager.isServerTest)
        {
            photonView.RPC(nameof(RPC_PushPlayer), RpcTarget.All);
        }
        else
        {
            RPC_PushPlayer();
        }
    }
    [PunRPC]
    public void RPC_PushPlayer()
    {
        isPushing = true; // 밀치는 중


        if (targetPlayerRb == null)
        {
            isPushing = false;
            return;
        }

        // 밀칠 방향 계산
        Vector3 pushDirection = (targetPlayerRb.transform.position - transform.position).normalized;
        targetPlayerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse); // 힘 적용하여 밀치기
        Debug.Log("밀치기 성공: " + targetPlayerRb.gameObject.name); // 디버그 메시지
        animator.SetTrigger(PushTriggerName); 
        StartCoroutine(ResetPushing()); // 딜레이 코루틴 시작
    }

    public bool CanPush()
    {
        return canPush && !isPushing;
    }

    IEnumerator ResetPushing()
    {
        yield return new WaitForSeconds(pushDelay); // 딜레이 시간만큼 대기
        isPushing = false; // 밀치기 가능 상태로 변경
    }
}