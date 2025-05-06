using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class AstronautWanderer : MonoBehaviour
{
    public float wanderRadius = 60f;
    public float waitTimeMin = 0.5f;
    public float waitTimeMax = 2f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(WanderLoop());
    }

    private IEnumerator WanderLoop()
    {
        while (true)
        {
            Vector3 center = transform.parent.position;
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection.y = 0f;
            randomDirection += center;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            // Hedefe varmayı bekle
            while (agent.pathPending || agent.remainingDistance > 0.5f)
                yield return null;

            yield return new WaitForSeconds(Random.Range(waitTimeMin, waitTimeMax));
        }
    }
}
