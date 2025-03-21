using UnityEngine;

public class WindController : MonoBehaviour
{
    public Vector3 windDirection = Vector3.forward;
    public float windForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController playerController = other.GetComponent<CharacterController>();
            if (playerController != null)
            {
                playerController.ApplyWindEffect(windDirection, windForce);
                Debug.Log("Player has entered the wind area.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController playerController = other.GetComponent<CharacterController>();
            if (playerController != null)
            {
                playerController.RemoveWindEffect();
                Debug.Log("Player has exited the wind area.");
            }
        }
    }

    // 에디터에서 바람 방향 시각적으로 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + windDirection.normalized * 5f);
    }
}