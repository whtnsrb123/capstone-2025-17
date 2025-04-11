using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    private Conveyor[] conveyors;

    void Start()
    {
        conveyors = FindObjectsOfType<Conveyor>();
    }

    private IEnumerator SetRandomConveyor()
    {
        while(true)
        {
            float time = Random.Range(5f, 15f);
            yield return new WaitForSeconds(time);

            int randIdx = Random.Range(0, conveyors.Length);
            conveyors[randIdx].On();
        }
    }
}