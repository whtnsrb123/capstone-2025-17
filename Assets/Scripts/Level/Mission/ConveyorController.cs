using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ConveyorController : MonoBehaviourPun
{
    private Conveyor[] conveyors;
    public static float time;
    public static int randIdx;

    void Start()
    {
        conveyors = FindObjectsOfType<Conveyor>();
        photonView.RPC(nameof(SetRandomTime), RpcTarget.All);
    }
    
    [PunRPC]
    private void SetRandomTime()
    {
        time = Random.Range(5f, 15f);
        randIdx = Random.Range(0, conveyors.Length);
        StartCoroutine(nameof(SetRandomConveyor));
    }
    private IEnumerator SetRandomConveyor()
    {
        while(true)
        {
            yield return new WaitForSeconds(time);
            
            conveyors[randIdx].On();
        }
    }
}