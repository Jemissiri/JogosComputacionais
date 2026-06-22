using UnityEngine;
using UnityEngine.AI;

public class DummyPlayer : MonoBehaviour
{
    [SerializeField] private float moveRadius = 8f;
    [SerializeField] private float repositionInterval = 3f;

    private NavMeshAgent _agent;
    private float _lastRepositionTime;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Time.time - _lastRepositionTime < repositionInterval) return;
        _lastRepositionTime = Time.time;

        // Pick a random point in the arena
        Vector3 randomDir = Random.insideUnitSphere * moveRadius;
        randomDir.y = 0f;
        Vector3 randomPos = transform.position + randomDir;

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, moveRadius, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
    }
}