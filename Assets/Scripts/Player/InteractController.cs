using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    void Start()
    {
        
    }

    public void Interact(GameObject target)
    {
        Debug.Log(target.name);
    }
}