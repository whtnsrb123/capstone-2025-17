using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCollider : MonoBehaviour
{
    public Vector3 spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // 사망 애니메이션 추가하는지?

            Transform target = other.transform;
            target.position = spawnPoint;
        }
    }
}