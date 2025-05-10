using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Autodoor : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;

    public GameObject clearArea;

    void Start()
    {
        
    }

    public void OpenDoor()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(leftDoor.DOMove(leftDoor.position + Vector3.left * 1.2f, 5f));
        seq.Join(rightDoor.DOMove(rightDoor.position + Vector3.right * 1.2f, 5f));
        seq.OnComplete(() => clearArea.SetActive(true));
    }

}