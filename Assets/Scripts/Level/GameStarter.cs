using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameStarter : MonoBehaviourPun
{
    public static void GameStart()
    {
        PhotonNetwork.LoadLevel("Mission1");
    }
}
