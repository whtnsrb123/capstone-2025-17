using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class PersonAgent : MonoBehaviourPun
{
    [Header("Wandering")]
    public float wanderRadius = 10f;
    public Vector2 wanderIntervalRange = new Vector2(3f, 7f);

    [Header("Detection")]
    public float viewDistance = 10f;
    public float viewAngle = 120f;
    public Transform eyePoint;
    public LayerMask playerLayer;
    public LayerMask obstructionMask;

    [Header("Combat")]
    public float chaseTimeout = 3f;
    public float attackDistance = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private GameObject targetPlayer;

    private float timer;
    private float currentWanderTimer;
    private float lastSeenTime;

    private enum State { Wander, Chase, Attack, Return }
    private State currentState = State.Wander;
    private State previousState = State.Wander;
    public bool isAttacking = false;

    private Vector3 lastDestination;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SetNewDestination();
    }

    void Update()
    {
        if (GameStateManager.isServerTest && !PhotonNetwork.IsMasterClient)
            return;

        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (currentState != previousState)
        {
            Debug.Log($"{gameObject.name} | State changed: {previousState} → {currentState}");
            previousState = currentState;
        }

        switch (currentState)
        {
            case State.Wander:
                InvokedStateMethod(nameof(HandleWander));
                break;
            case State.Chase:
                InvokedStateMethod(nameof(HandleChase));
                break;
            case State.Attack:
                InvokedStateMethod(nameof(HandleAttack));
                break;
            case State.Return:
                InvokedStateMethod(nameof(HandleReturn));
                break;
        }
    }

    void LateUpdate()
    {
        if (currentState == State.Wander || currentState == State.Chase || currentState == State.Attack)
        {
            UpdateChaseStateIfNeeded();
        }
    }

    void InvokedStateMethod(string methodName, RpcTarget target = RpcTarget.All)
    {
        if (GameStateManager.isServerTest)
            photonView.RPC(methodName, target);
        else
            SendMessage(methodName);
    }

    private void UpdateChaseStateIfNeeded()
    {
        GameObject visiblePlayer = FindFirstVisiblePlayer();
        GameObject obstructedPlayer = FindObstructedPlayer();

        if (visiblePlayer != null)
        {
            visiblePlayer.GetComponent<ChaseStateManager>()?.SetChaseState(ChaseStateManager.ChaseState.Detected);
        }
        else if (obstructedPlayer != null)
        {
            obstructedPlayer.GetComponent<ChaseStateManager>()?.SetChaseState(ChaseStateManager.ChaseState.Obstructed);
        }
    }

    [PunRPC]
    private void HandleWander()
    {
        timer += Time.deltaTime;
        if (timer >= currentWanderTimer)
        {
            Debug.Log($"{gameObject.name} | Wander: Setting new destination.");
            SetNewDestination();
        }

        GameObject seenPlayer = FindFirstVisiblePlayer();
        if (seenPlayer != null)
        {
            Debug.Log($"{gameObject.name} | Wander: Player detected! Switching to Chase.");
            targetPlayer = seenPlayer;
            currentState = State.Chase;
        }
    }

    [PunRPC]
    private void HandleChase()
    {
        if (targetPlayer == null)
        {
            currentState = State.Return;
            agent.SetDestination(lastDestination);
            return;
        }

        agent.SetDestination(targetPlayer.transform.position);
        Debug.Log($"{gameObject.name} | Chase: Chasing the player.");

        if (CanSeeTarget())
        {
            lastSeenTime = Time.time;

            if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= attackDistance)
            {
                Debug.Log($"{gameObject.name} | Chase: Player in range. Switching to Attack.");
                currentState = State.Attack;
            }
        }
        else if (Time.time - lastSeenTime > chaseTimeout)
        {
            Debug.Log($"{gameObject.name} | Chase: Lost sight of player. Returning.");
            currentState = State.Return;
            agent.SetDestination(lastDestination);
        }
    }

    [PunRPC]
    private void HandleAttack()
    {
        if (targetPlayer == null)
        {
            currentState = State.Return;
            agent.SetDestination(lastDestination);
            return;
        }

        if(isAttacking) return;

        agent.ResetPath();
        transform.LookAt(targetPlayer.transform);
        animator.SetTrigger("Attack");
        isAttacking = true;

        Debug.Log($"{gameObject.name} | Attack: Triggered attack animation.");
    }

    // 애니메이션 끝에 연결된 이벤트에서 호출됨
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        
        if (targetPlayer == null)
        {
            Debug.Log($"{gameObject.name} | Attack End: No target → Return");
            currentState = State.Return;
            agent.SetDestination(lastDestination);
            return;
        }

        float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);
        Debug.Log($"{gameObject.name} | Attack End: Distance = {distance:F2}");

        if (distance > attackDistance + 0.5f)
        {
            Debug.Log($"{gameObject.name} | Attack End: Player out of range → Chase");
            currentState = State.Chase;
        }
        else
        {
            Debug.Log($"{gameObject.name} | Attack End: Still in range → stay in Attack or re-trigger");
        }
    }

    [PunRPC]
    private void HandleReturn()
    {
        Debug.Log($"{gameObject.name} | Return: Returning to last destination.");

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            Debug.Log($"{gameObject.name} | Return: Reached last destination. Switching to Wander.");
            currentState = State.Wander;
            SetNewDestination();

            if (targetPlayer != null)
            {
                targetPlayer.GetComponent<ChaseStateManager>()?.SetChaseState(ChaseStateManager.ChaseState.Undetected);
                targetPlayer = null;
            }
        }
    }

    private void SetNewDestination()
    {
        lastDestination = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(lastDestination);
        currentWanderTimer = Random.Range(wanderIntervalRange.x, wanderIntervalRange.y);
        timer = 0;
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection.y = 0f;
        randDirection += origin;

        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }

    private GameObject FindFirstVisiblePlayer()
    {
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p == null) continue;
            Vector3 dir = (p.transform.position - eyePoint.position).normalized;
            float dist = Vector3.Distance(eyePoint.position, p.transform.position);
            float angle = Vector3.Angle(new Vector3(eyePoint.forward.x, 0, eyePoint.forward.z), new Vector3(dir.x, 0, dir.z));

            if (angle < viewAngle / 2f && dist < viewDistance &&
                !Physics.Raycast(eyePoint.position, dir, dist, obstructionMask))
                return p;
        }
        return null;
    }

    private GameObject FindObstructedPlayer()
    {
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p == null) continue;
            Vector3 dir = (p.transform.position - eyePoint.position).normalized;
            float dist = Vector3.Distance(eyePoint.position, p.transform.position);
            float angle = Vector3.Angle(new Vector3(eyePoint.forward.x, 0, eyePoint.forward.z), new Vector3(dir.x, 0, dir.z));

            if (angle < viewAngle / 2f && dist < viewDistance &&
                Physics.Raycast(eyePoint.position, dir, dist, obstructionMask))
                return p;
        }
        return null;
    }

    private bool CanSeeTarget()
    {
        if (targetPlayer == null) return false;

        Vector3 dir = (targetPlayer.transform.position - eyePoint.position).normalized;
        float dist = Vector3.Distance(eyePoint.position, targetPlayer.transform.position);
        float angle = Vector3.Angle(new Vector3(eyePoint.forward.x, 0, eyePoint.forward.z), new Vector3(dir.x, 0, dir.z));

        return angle < viewAngle / 2f && dist < viewDistance &&
               !Physics.Raycast(eyePoint.position, dir, dist, obstructionMask);
    }

    private void OnDrawGizmosSelected()
    {
        if (eyePoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyePoint.position, viewDistance);

        Vector3 left = Quaternion.Euler(0, -viewAngle / 2f, 0) * eyePoint.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2f, 0) * eyePoint.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(eyePoint.position, left * viewDistance);
        Gizmos.DrawRay(eyePoint.position, right * viewDistance);
    }
}
