using UnityEngine;
using System.Collections;
using Photon.Pun;

public class Generator : MonoBehaviourPun, IInteractable
{
    public bool isOn = false;
    private Light light;
    private Coroutine generating;

    void Start()
    {
        light = transform.GetChild(0).GetComponent<Light>();    
    }

    public void Interact()
    {
        if(generating != null)
            return;

        photonView.RPC(nameof(OnGenerate), RpcTarget.All);
    }

    [PunRPC]
    private void OnGenerate()
    {
        generating = StartCoroutine(Generate());
    }
    

    private IEnumerator Generate()
    {
        isOn = true;
        light.color = Color.green;

        yield return new WaitForSeconds(5f);

        isOn = false;
        light.color = Color.red;
        generating = null;
    }

    public void FinishGenerate()
    {
        light.color = Color.green;
        if(generating != null)
            StopCoroutine(generating);
        
        Destroy(this);
    }
}