using UnityEngine;
using UnityEngine.AI;

public class PersonAgent : MonoBehaviour
{
    public float wanderRadius = 10f;
    public Vector2 wanderIntervalRange = new Vector2(3f, 7f);

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;
    private float currentWanderTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        SetNewDestination();
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        if (timer >= currentWanderTimer)
            SetNewDestination();
    }

    private void SetNewDestination()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);

        SetRandomTimer();
        timer = 0;
    }

    private void SetRandomTimer()
    {
        currentWanderTimer = Random.Range(wanderIntervalRange.x, wanderIntervalRange.y);
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection.y = 0f;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}