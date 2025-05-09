using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RoboticArm : MonoBehaviourPun
{
    [SerializeField]
    private RoboticArmHead head;
    private Animator animator;

    public Transform pickedTarget;

    void Start()
    {
        animator = GetComponent<Animator>();
        head = GetHead()[0];

        if (GameStateManager.isServerTest && !PhotonNetwork.IsMasterClient) return;

        if (GameStateManager.isServerTest)
        {
            photonView.RPC(nameof(RunArmRPC), RpcTarget.All);
        }
        else
        {
            Invoke("RunArm", 5f);
        }
    }

    private RoboticArmHead[] GetHead()
    {
        return gameObject.GetComponentsInChildren<RoboticArmHead>();
    }

    [PunRPC]
    private void RunArmRPC()
    {
        Invoke("RunArm", 5f);
    }
    
    public void RunArm()
    {
        animator.SetTrigger("Run");
    }
    
    [PunRPC] //플레이어 드랍하는 타이밍이 언제일까요??
    public void TryDropPlayer()
    {
        if(head.target == null) return;
        head.DropPlayer();
    }
}