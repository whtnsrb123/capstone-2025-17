using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorForElevator : MonoBehaviour
{
    public Generator firstGenerator;
    public Generator secondGenerator;
    public Elevator elevator;
    public ShortCutSceneCamera cutSceneCamera;
    void Update()
    {
        if (firstGenerator.isOn && secondGenerator.isOn)
        {
            Managers.GimmickManager.ActivateGimmick("Generator");
        }
        if(Managers.GimmickManager.IsGimmickActivated("Generator"))
        {
            cutSceneCamera.PlayShortCutScene();


            firstGenerator.FinishGenerate();
            secondGenerator.FinishGenerate();

            elevator.gameObject.tag = "Interactable";
            elevator.Interact();

            
            Destroy(this);
        }
    }
}