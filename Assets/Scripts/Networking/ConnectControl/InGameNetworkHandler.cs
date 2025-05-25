using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameNetworkHandler : MonoBehaviourPunCallbacks
{
    PhotonView[] allViews;
    PhotonView myView;

    private void Start()
    {
        allViews = FindObjectsOfType<PhotonView>();
        myView = GetComponent<PhotonView>();
        Debug.Assert(myView != null, "cannot find my view");
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            DisableLeftPlayer(otherPlayer);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("On Player Entered Room");
            EnableRejoinPlayer(newPlayer);
        }
    }

    public void DisableLeftPlayer(Player player)
    {
        myView.RPC("RPC_DisableLeftPlayer", RpcTarget.All, player.ActorNumber);
        Debug.Log("마스터 클라이언트만 실행합니다.");
    }

    [PunRPC]
    public void RPC_DisableLeftPlayer(int leftPlayerActorNumber)
    {
        foreach (PhotonView view in allViews)
        {
            if (view.Owner != null && view.Owner.ActorNumber == leftPlayerActorNumber)
            {
                view.gameObject.SetActive(false);
            }
        }
    }

    public void EnableRejoinPlayer(Player player)
    {
        myView.RPC("RPC_EnableRejoinedPlayer", RpcTarget.All, player.ActorNumber);
        Debug.Log(player.ActorNumber + "마스터 클라이언트만 실행합니다.");
    }

    [PunRPC]
    public void RPC_EnableRejoinedPlayer(int enteredPlayerActorNumber)
    {
        foreach (PhotonView view in allViews)
        {
            if (view.Owner != null && view.Owner.ActorNumber == enteredPlayerActorNumber)
            {
                view.gameObject.SetActive(true);
                Debug.Log("각자 한 번씩 실행합니다.");
                return;
            }
        }
        Debug.Log("can't find Rejoined Player");

    }

}
