using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RoboticArmHead : MonoBehaviourPun
{
    public Transform target;
    public Transform pickPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (GameStateManager.isServerTest && !PhotonNetwork.IsMasterClient) return;

        if (other.gameObject.CompareTag("Player"))
        {
            if (GameStateManager.isServerTest)
            {
                photonView.RPC(nameof(PickPlayer), RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
            }
            // else
            // {
            //     PickPlayer(other.transform);
            // }
        }
        
    }
    
    [PunRPC]
    private void PickPlayer(int viewID)
    {
        target = PhotonView.Find(viewID).transform;
        target.GetComponent<CharacterController>().enabled = false;
        target.GetComponent<Rigidbody>().useGravity = false;
        target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        target.SetParent(pickPoint);
        target.localPosition = Vector3.zero;
    }

    public void DropPlayer()
    {
        target.GetComponent<CharacterController>().enabled = true;
        target.GetComponent<Rigidbody>().useGravity = true;
        target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        target.SetParent(null);
        target = null;
    }
}