using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;

public class PlayerEventReceiver : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private const byte SPAWN_PLAYER_EVENT = 1;

    public GameObject myPlayerObject;

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == SPAWN_PLAYER_EVENT)
        {
            object[] data = (object[])photonEvent.CustomData;

            string[] names = (string[])data[0];
            string[] clients = (string[])data[1];
            int[] viewIDs = (int[])data[2];
            
            for (int i = 0; i < viewIDs.Length; i++)
            {
                PhotonView view = PhotonView.Find(viewIDs[i]);

                if (view == null)
                {
                    Debug.LogWarning($"[OnEvent] ViewID {viewIDs[i]}에 해당하는 PhotonView를 찾지 못함");
                    continue;
                }

                GameObject playerObj = view.gameObject;

                // 내 캐릭터라면
                if (view.IsMine)
                {
                    Debug.Log($"내 캐릭터를 찾음: {names[i]}, viewID: {viewIDs[i]}");
                }
            }
        }
    }
}