using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboticArm : MonoBehaviour
{
    [SerializeField]
    private RoboticArmHead collider;
    private Animator animator;

    public Transform pickedTarget;

    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetHead()[0];
    }

    private RoboticArmHead[] GetHead()
    {
        return gameObject.GetComponentsInChildren<RoboticArmHead>();
    }

    public void RunArm()
    {
        animator.SetTrigger("Run");
    }
}