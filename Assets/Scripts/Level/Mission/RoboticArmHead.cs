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
        target.SetParent(pickPoint);
        target.localPosition = Vector3.zero;
    }

    private void OnTriggerExit(Collider other)
    {
        target = null;
    }
}