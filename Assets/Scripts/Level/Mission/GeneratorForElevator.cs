using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorForElevator : MonoBehaviour
{
    public Generator firstGenerator;
    public Generator secondGenerator;
    public Elevator elevator;

    void Update()
    {
        if(firstGenerator.isOn && secondGenerator.isOn)
        {
            firstGenerator.FinishGenerate();
            secondGenerator.FinishGenerate();

            elevator.gameObject.tag = "Interactable";
            elevator.Interact();
            
            Destroy(this);
        }
    }
}