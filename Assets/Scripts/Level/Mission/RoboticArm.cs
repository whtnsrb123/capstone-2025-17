using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboticArm : MonoBehaviour
{
    [SerializeField]
    private RoboticArmHead head;
    private Animator animator;

    public Transform pickedTarget;

    void Start()
    {
        animator = GetComponent<Animator>();
        head = GetHead()[0];

        Invoke("RunArm", 5f);
    }

    private RoboticArmHead[] GetHead()
    {
        return gameObject.GetComponentsInChildren<RoboticArmHead>();
    }

    public void RunArm()
    {
        animator.SetTrigger("Run");
    }

    public void TryDropPlayer()
    {
        if(head.target == null) return;
        head.DropPlayer();
    }
}