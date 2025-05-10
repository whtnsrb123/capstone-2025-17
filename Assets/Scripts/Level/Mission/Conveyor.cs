using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public bool isOn = false;
    [SerializeField]
    private float speed;
    private Vector3 pos;
    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void On()
    {
        StartCoroutine(OnConveyor());
    }

    private IEnumerator OnConveyor()
    {
        isOn = true;
        float time = Random.Range(5f, 10f);
        yield return new WaitForSeconds(time);
        isOn = false;
    }

    private void FixedUpdate()
    {
        if(!isOn)
            return;

        pos = rigidbody.position;
        rigidbody.position += (transform.forward * -1) * speed * Time.fixedDeltaTime;
        rigidbody.MovePosition(pos);
    }
}