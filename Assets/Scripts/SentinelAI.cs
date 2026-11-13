using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SentinelAI : MonoBehaviour
{
    private enum State { Patrol, Chase }

    private NavMeshAgent agent;
    private PlayerMovement playerMovement;
    private Transform player;
    private PlayerHealth playerHealth;

    private State state;
    private Vector3 homePosition;
    private bool warnedNoNavMesh;

    private float patrolRadius;
    private float patrolSpeed;
    private float waitAtPoint;
    private float waitTimer;


    private float detectionRadius;
    private float loseRadius;
    private float fieldOfView;
    private float eyeHeight;
    private float chaseSpeed;
    private float repathInterval;
    private float repathTimer;


    private int contactDamage;
    private float attackRange;

    private void Start()
    {
        patrolRadius = 12f;
        patrolSpeed = 2f;
        waitAtPoint = 1.5f;

        detectionRadius = 20f;  
        loseRadius = 28f;        
        fieldOfView = 160f;
        eyeHeight = 1f;
        chaseSpeed = 6f;
        repathInterval = 0.2f;

        contactDamage = 10;
        attackRange = 1.8f;

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 1.2f;


        if (!agent.isOnNavMesh)
        {
            NavMeshHit placeHit;
            if (NavMesh.SamplePosition(transform.position, out placeHit, 10f, NavMesh.AllAreas))
                agent.Warp(placeHit.position);
        }

        playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            player = playerMovement.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        homePosition = transform.position;
        StartPatrol();
    }

    private void Update()
    {
        if (player == null) return;

        if (!agent.isOnNavMesh)
        {
            if (!warnedNoNavMesh)
            {
                warnedNoNavMesh = true;
                Debug.LogWarning("SentinelAI: el dron " + name + " no está sobre el NavMesh. Hornear el NavMesh o moverlo a una zona celeste.");
            }
            return;
        }

        if (state == State.Patrol) UpdatePatrol();
        else UpdateChase();
    }


    private void StartPatrol()
    {
        state = State.Patrol;
        agent.speed = patrolSpeed;
        waitTimer = 0f;
        GoToRandomPoint();
    }

    private void UpdatePatrol()
    {
        if (CanSeePlayer())
        {
            StartChase();
            return;
        }


        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitAtPoint)
            {
                waitTimer = 0f;
                GoToRandomPoint();
            }
        }
    }

    private void GoToRandomPoint()
    {
        if (!agent.isOnNavMesh) return;

        Vector2 circle = Random.insideUnitCircle * patrolRadius;
        Vector3 candidate = homePosition + new Vector3(circle.x, 0f, circle.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(candidate, out hit, 5f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }


    private void StartChase()
    {
        state = State.Chase;
        agent.speed = chaseSpeed;
        repathTimer = 0f;
    }

    private void UpdateChase()
    {
        float distance = Vector3.Distance(transform.position, player.position);


        if (distance > loseRadius || playerMovement.Stats.Invisible)
        {
            StartPatrol();
            return;
        }


        if (distance <= attackRange && playerHealth != null)
            playerHealth.TakeDamage(contactDamage);


        repathTimer += Time.deltaTime;
        if (repathTimer >= repathInterval)
        {
            repathTimer = 0f;
            agent.SetDestination(player.position);
        }
    }


    private bool CanSeePlayer()
    {
        if (playerMovement.Stats.Invisible) return false;

        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 toPlayer = (player.position + Vector3.up * 0.5f) - eye;

        if (toPlayer.magnitude > detectionRadius) return false;
        if (Vector3.Angle(transform.forward, toPlayer) > fieldOfView * 0.5f) return false;

        RaycastHit[] hits = Physics.RaycastAll(eye, toPlayer.normalized, detectionRadius, ~0, QueryTriggerInteraction.Ignore);

        float closest = float.MaxValue;
        bool playerIsFirst = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.IsChildOf(transform)) continue;

            if (hit.distance < closest)
            {
                closest = hit.distance;
                playerIsFirst = hit.transform.IsChildOf(player);
            }
        }

        return playerIsFirst;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (playerHealth == null) return;

        if (collision.collider.transform.IsChildOf(player))
            playerHealth.TakeDamage(contactDamage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 14f);
    }
}