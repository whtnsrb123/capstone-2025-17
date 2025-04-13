using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MissionSpawner : MonoBehaviourPun
{

    [Header("플레이어 프리팹")] public GameObject playerPrefab;
    [Header("스폰 포인트(4명)")] public Transform[] spawnPoints;
    
    
    void Start()
    {
        // 씬이 로드되고 연결이 유지되어 있을 때만
        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
        }
    }
    
    public void SpawnPlayer()
    {
        Debug.Log("SpawnPlayer 실행됨");
        // 핵심: 로컬에서만 Instantiate 실행
        if (PhotonNetwork.LocalPlayer == null || playerPrefab == null)
        {
            Debug.LogWarning("SpawnPlayer failed: No local player or prefab.");
            return;
        }
        
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        // if (playerIndex >= spawnPoints.Length)
        // {
        //     Debug.Log("스폰 포인트보다 플레이어수가 더 많습니다.");
        //     playerIndex = playerIndex % spawnPoints.Length; // or just set to 0
        // }
        Debug.Log(playerIndex);
        Vector3 spawnPos = spawnPoints[playerIndex].position;
        Quaternion spawnRot = spawnPoints[playerIndex].rotation;
        
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, spawnRot);
    }

    
}
