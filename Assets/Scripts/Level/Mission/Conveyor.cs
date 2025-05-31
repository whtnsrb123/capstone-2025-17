using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Conveyor : MonoBehaviourPun
{
    public bool isOn = false;
    [SerializeField]
    private float speed;
    private Vector3 pos;
    private List<PhotonView> targets;

    private void Start()
    {
        targets = new List<PhotonView>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.CompareTag("Player"))
            targets.Add(other.transform.GetComponent<PhotonView>());
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.transform.CompareTag("Player"))
            targets.Remove(other.transform.GetComponent<PhotonView>());
    }

    public void On()
    {
        StartCoroutine(OnConveyor());
    }

    private IEnumerator OnConveyor()
    {
        isOn = true;
        float time = ConveyorController.time;
        yield return new WaitForSeconds(time);
        isOn = false;
    }

    private void FixedUpdate()
    {
        if(!isOn)
            return;
        
        if (targets.Count != 0)
        {
            foreach (PhotonView pv in targets)
            {
                photonView.RPC(nameof(RPC_ReceivePush), pv.Owner, pv.ViewID);
            }
        }
    }

    [PunRPC]
    public void RPC_ReceivePush(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        Rigidbody rb = view.GetComponent<Rigidbody>();
        Vector3 move = (transform.forward * -1) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}