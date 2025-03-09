using UnityEngine;
using UnityEngine.Events;

public class Switch : InteractableObject
{
    public UnityEvent onPressed;
    
    public override void Interact()
    {
        onPressed.Invoke();
    }

    public void Test()
    {
        Debug.Log("TEST MESSAGE");
    }

    void Start()
    {
        Interact();
    }
}