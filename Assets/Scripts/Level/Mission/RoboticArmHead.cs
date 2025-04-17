using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboticArmHead : MonoBehaviour
{
    public Transform target;
    public Transform pickPoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            PickPlayer(other.transform);
    }

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
        target = null;
    }
}