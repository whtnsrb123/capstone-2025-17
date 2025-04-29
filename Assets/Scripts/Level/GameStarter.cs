using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviourPun
{
    public static void GameStart()
    {
        if (GameStateManager.isServerTest)
        {
            PhotonNetwork.LoadLevel("Mission1");
        }
        else
        {
            SceneManager.LoadScene("Mission1");
        }
    }
}
