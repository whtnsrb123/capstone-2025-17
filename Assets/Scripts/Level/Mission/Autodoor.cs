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
        OpenDoor();
    }

    public void OpenDoor()
    {
        leftDoor.DOMove(leftDoor.position + Vector3.left * 2.5f, 5f);
        rightDoor.DOMove(rightDoor.position + Vector3.right * 2.5f, 5f);
    }
}