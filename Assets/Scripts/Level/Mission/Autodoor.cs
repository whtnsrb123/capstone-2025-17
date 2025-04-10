using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Autodoor : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;

    void Start()
    {
        
    }

    public void OpenDoor()
    {
        leftDoor.DOMove(leftDoor.position + Vector3.left * 1.2f, 5f);
        rightDoor.DOMove(rightDoor.position + Vector3.right * 1.2f, 5f);
    }
}