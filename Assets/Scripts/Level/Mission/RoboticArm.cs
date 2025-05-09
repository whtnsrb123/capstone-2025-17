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
            photonView.RPC(nameof(RunArmRPC), RpcTarget.All);
        else
            StartCoroutine(RunArmLoop());
    }

    private RoboticArmHead[] GetHead()
    {
        return gameObject.GetComponentsInChildren<RoboticArmHead>();
    }

    [PunRPC]
    private void RunArmRPC()
    {
        StartCoroutine(RunArmLoop());
    }

    private IEnumerator RunArmLoop()
    {
        while (true)
        {
            RunArm();
            float delay = Random.Range(15f, 30f);
            yield return new WaitForSeconds(delay);
        }
    }

    public void RunArm()
    {
        animator.SetTrigger("Run");
    }

    [PunRPC]
    public void TryDropPlayer()
    {
        if (head.target == null) return;
        head.DropPlayer();
    }
}