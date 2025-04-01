using UnityEngine;
using System.Collections;

public class ShortCutSceneCamera : MonoBehaviour
{
    private Camera camera;

    public float playTime;

    void Start()
    {
        camera = GetComponent<Camera>();    
    }

    public void PlayShortCutScene()
    {
        camera.enabled = true;
        StartCoroutine(SetCamera());
    }

    private IEnumerator SetCamera()
    {
        yield return new WaitForSeconds(playTime);
        gameObject.SetActive(false);
    }
}