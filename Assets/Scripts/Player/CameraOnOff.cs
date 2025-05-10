using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CameraOnOff : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Awake()
    {
        if (photonView.IsMine)
        {
            gameObject.transform.Find("Main Camera").gameObject.SetActive(true);
        }
    }
}
