using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Sprinkler : MonoBehaviourPun
{
    public int count = 3;
    public GameObject waterFall;
    private bool isTriggered = false;

    private void OnCollisionEnter(Collision other)
    {
        PhotonView targetView = other.gameObject.GetComponent<PhotonView>();
        if (targetView != null && other.gameObject.CompareTag("Pickable"))
        {
            photonView.RPC(nameof(RPC_OnCollisionEnter), RpcTarget.All, targetView.ViewID);
        }
    }

    [PunRPC]
    private void RPC_OnCollisionEnter(int otherView)
    {
        if (isTriggered) return;

        PhotonView target = PhotonView.Find(otherView);
        if (target != null && target.gameObject.CompareTag("Pickable"))
        {
            isTriggered = true;
            count--;

            if (count <= 0)
            {
                Destroy(GetComponent<Outline>());
                Destroy(waterFall);
                Destroy(this);
            }
            else
            {
                StartCoroutine(RotateAndScale());
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
        float scaleRatio = Mathf.Clamp01((float)count / 3f);
        Vector3 targetScale = new Vector3(
            startScale.x * scaleRatio,
            startScale.y,
            startScale.z * scaleRatio
        );

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            float currentRotation = Mathf.Lerp(startRotation, targetRotation, t);
            transform.parent.rotation = Quaternion.Euler(
                transform.parent.rotation.eulerAngles.x,
                transform.parent.rotation.eulerAngles.y,
                currentRotation
            );

            waterFall.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        isTriggered = false;
    }
}
