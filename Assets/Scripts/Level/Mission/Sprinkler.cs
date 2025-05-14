using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Sprinkler : MonoBehaviourPun
{
    public int count = 3;
    public GameObject waterFall;

    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        photonView.RPC(nameof(RPC_OnCollisionEnter), RpcTarget.All, other);
    }

    [PunRPC]
    private void RPC_OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Pickable")) {
            StartCoroutine(RotateAndScale());
            count--;
            if(count <= 0)
            {
                Destroy(GetComponent<Outline>());
                Destroy(waterFall);
                Destroy(this);
            }
        }
    }

    private IEnumerator RotateAndScale()
    {
        float elapsedTime = 0f;
        float duration = 2f;
        float startRotation = transform.parent.rotation.eulerAngles.z;
        float targetRotation = startRotation + 60f;
        Vector3 startScale = waterFall.transform.localScale;
        Vector3 targetScale = new Vector3(startScale.x * (count - 1) / 3f, startScale.y, startScale.z * (count - 1) / 3f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentRotation = Mathf.Lerp(startRotation, targetRotation, t);
            transform.parent.rotation = Quaternion.Euler(transform.parent.rotation.eulerAngles.x, transform.parent.rotation.eulerAngles.y, currentRotation);
            waterFall.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
    }
}