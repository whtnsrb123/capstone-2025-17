using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PersonHand : MonoBehaviourPun
{
    public Transform target;
    public Transform pickPoint;
    private Transform respawnPoint;

    void Start( )
    {
        respawnPoint = GameObject.Find( "Respawn Point" ).transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameStateManager.isServerTest && !PhotonNetwork.IsMasterClient) return;

        if (other.gameObject.CompareTag("Player"))
        {
            if (GameStateManager.isServerTest)
            {
                photonView.RPC(nameof(PickPlayer), RpcTarget.All, other.transform);
            }
            else
            {
                PickPlayer(other.transform);
            }
        }
        
    }
    
    [PunRPC]
    private void PickPlayer(Transform targetPlayer)
    {
        target = targetPlayer;
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
        target.transform.position = respawnPoint.position;
        target = null;
    }
}
