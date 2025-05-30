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
            SpawnPlayer();
        }
    }
    
    private IEnumerator CheckRoomFull()
    {
        while (!isReady)
        {
            Debug.Log("전원 입장 기다리는 중");
            if (PhotonNetwork.CurrentRoom.PlayerCount == ServerInfo.RequiredPlayerCount)
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
    private void SpawnPlayer()
    {
        int spawnIndex = -1;

        // ActorNubmer는 1부터 순차 증가를 보장하지 않으므로, Player의  ActorNumber가 기록된 Room의 CustomProperties를 사용한다
        int[] actorNumbers = (int[])PhotonNetwork.CurrentRoom.CustomProperties["PlayerActorNumbers"];

        for (var i = 0; i < actorNumbers.Length; i++)
        {
            if (actorNumbers[i] == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                spawnIndex = i;
            }
        }

        int meshId = (int) PhotonNetwork.LocalPlayer.CustomProperties[ClientInfo.CharacterIdKey];
        // string nickname = PhotonNetwork.LocalPlayer.CustomProperties[ClientInfo.NicknameKey].ToString();

        object[] data = new object[] { meshId }; // data[0] = meshId;

        // 입장 전 actorNumbers 정보를 갱신하지만, 만에 하나 0으로 처리한다 
        spawnIndex = spawnIndex == -1 ? 0 : spawnIndex;

        Vector3 spawnPos = spawnPoints[spawnIndex].position;
        Quaternion spawnRot = spawnPoints[spawnIndex].rotation;
        Debug.Log($"Spawn Player 호출 : {PhotonNetwork.LocalPlayer.ActorNumber}");
        
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, spawnRot, 0, data);
        PhotonView view = player.GetComponent<PhotonView>();
        Debug.Log($"[소유권 확인] {view.ViewID} → {view.OwnerActorNr}");

        //playerProperties = new Dictionary<PlayerProperty, GameObject>();

    //     string[] names = new string[PhotonNetwork.CurrentRoom.Players.Count];
    //     string[] clients = new string[PhotonNetwork.CurrentRoom.Players.Count];
    //     int[] photonViews = new int[PhotonNetwork.CurrentRoom.Players.Count];
    //     
    //     int cnt = 0;
    //
    //     foreach (KeyValuePair<int, Player> pl in PhotonNetwork.CurrentRoom.Players)
    //     {
    //         int spawnIndex = cnt % spawnPoints.Length;
    //         Vector3 spawnPos = spawnPoints[spawnIndex].position;
    //         Quaternion spawnRot = spawnPoints[spawnIndex].rotation;
    //         
    //         Debug.Log("플레이어 확인 : " + pl.Value.ActorNumber);
    //
    //         GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, spawnRot);
    //         PhotonView view = player.GetComponent<PhotonView>();
    //         
    //         view.TransferOwnership(pl.Value.ActorNumber);
    //         bool success = view.OwnerActorNr == pl.Value.ActorNumber;
    //         Debug.Log($"[소유권 확인] {view.ViewID} → {view.OwnerActorNr} / 기대값: {pl.Value.ActorNumber} / 성공여부: {success}");
    //
    //         names[cnt] = pl.Value.ActorNumber.ToString();
    //         clients[cnt] = pl.Value.ActorNumber.ToString();
    //         photonViews[cnt] = view.ViewID;
    //         cnt++;
    //     }
    //
    //     for(int i = 0; i < names.Length; i++)
    //     {
    //         Debug.Log("정보 확인 : " + names[i] + " / " + clients[i] + " / " + photonViews[i].ToString());
    //     }
    //
    //     object[] content = new object[] { names, clients, photonViews };
    //
    //     RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    //     SendOptions sendOptions = new SendOptions { Reliability = true };
    //     PhotonNetwork.RaiseEvent(1, content, options, sendOptions);
    //     Debug.Log("RaiseEvent 실행함");
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;

        int skinInfoIndex = 0;

        if (instantiationData != null && instantiationData.Length > 0)
        {
            int skinId = (int)instantiationData[skinInfoIndex];
            ApplySkin(skinId);
        }

    }

    public void ApplySkin(int skinId)
    {

    }

}
