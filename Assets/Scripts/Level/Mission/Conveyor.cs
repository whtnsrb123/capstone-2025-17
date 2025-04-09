using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    [SerializeField]
    private bool isOn = false;
    [SerializeField]
    private float speed;
    private Vector3 pos;
    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        pos = rigidbody.position;
        rigidbody.position += (transform.forward * -1) * speed * Time.fixedDeltaTime;
        rigidbody.MovePosition(pos);
    }
}