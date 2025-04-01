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
        IInteractable interactable = target.GetComponent<IInteractable>();

        if(interactable == null)
            return;

        interactable.Interact();
    }
}