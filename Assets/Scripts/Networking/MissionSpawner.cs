using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MissionSpawner : MonoBehaviourPun
{

    [Header("플레이어 프리팹")] public GameObject playerPrefab;
    [Header("스폰 포인트(4명)")] public Transform[] spawnPoints;

    private bool isReady = false;
    
    void Start()
    {
        // 씬이 로드되고 연결이 유지되어 있을 때만
        if (PhotonNetwork.IsConnectedAndReady)
        {
            //StartCoroutine(nameof(CheckRoomFull));
            if (PhotonNetwork.IsMasterClient)
            {
                SpawnAllPlayer();
            }
        }
    }
    
    private IEnumerator CheckRoomFull()
    {
        while (!isReady)
        {
            Debug.Log("전원 입장 기다리는 중");
            if (PhotonNetwork.CurrentRoom.PlayerCount == ServerInfo.CMaxPlayer)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    //SpawnPlayer();
                }

                isReady = true;
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
    private void SpawnAllPlayer()
    {
        //playerProperties = new Dictionary<PlayerProperty, GameObject>();

        string[] names = new string[PhotonNetwork.CurrentRoom.Players.Count];
        string[] clients = new string[PhotonNetwork.CurrentRoom.Players.Count];
        int[] photonViews = new int[PhotonNetwork.CurrentRoom.Players.Count];
        
        int cnt = 0;

        foreach (KeyValuePair<int, Player> pl in PhotonNetwork.CurrentRoom.Players)
        {
            int spawnIndex = cnt % spawnPoints.Length;
            Vector3 spawnPos = spawnPoints[spawnIndex].position;
            Quaternion spawnRot = spawnPoints[spawnIndex].rotation;
            
            Debug.Log("플레이어 확인 : " + pl.Value.ActorNumber);

            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, spawnRot);
            PhotonView view = player.GetComponent<PhotonView>();
            
            view.TransferOwnership(pl.Value);
            bool success = view.OwnerActorNr == pl.Value.ActorNumber;
            Debug.Log($"[소유권 확인] {view.ViewID} → {view.OwnerActorNr} / 기대값: {pl.Value.ActorNumber} / 성공여부: {success}");

            names[cnt] = pl.Value.ActorNumber.ToString();
            clients[cnt] = pl.Value.ActorNumber.ToString();
            photonViews[cnt] = view.ViewID;
            cnt++;
        }

        for(int i = 0; i < names.Length; i++)
        {
            Debug.Log("정보 확인 : " + names[i] + " / " + clients[i] + " / " + photonViews[i].ToString());
        }

        object[] content = new object[] { names, clients, photonViews };

        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(1, content, options, sendOptions);
        Debug.Log("RaiseEvent 실행함");
    }
    
    
}
