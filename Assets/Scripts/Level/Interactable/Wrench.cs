using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrench : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 lastCheckedPosition;

    private float checkInterval = 10f;
    private float checkTimer = 0f;

    void Start()
    {
        initialPosition = transform.position;
        lastCheckedPosition = transform.position;
    }

    void Update()
    {
        checkTimer += Time.deltaTime;

        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;

            if (Vector3.Distance(transform.position, lastCheckedPosition) < 0.01f)
                transform.position = initialPosition;

            lastCheckedPosition = transform.position;
        }
    }
}
