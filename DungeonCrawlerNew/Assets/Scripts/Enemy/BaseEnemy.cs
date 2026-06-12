using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 50f;

    [Header("Detection")]
    [SerializeField] protected float detectionRange = 10f;

    protected float _currentHealth;
    protected bool _isDead = false;

    protected NavMeshAgent _agent;
    protected Animator _animator;
    protected Transform _player;

    protected static readonly int HashIsMoving = Animator.StringToHash("IsMoving");
    protected static readonly int HashIsAttacking = Animator.StringToHash("IsAttacking");
    protected static readonly int HashIsDead = Animator.StringToHash("IsDead");

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
        else Debug.LogWarning(name + ": No GameObject with tag 'Player' found.");
        _currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        if (_isDead) return;
        if (_player == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) _player = playerObj.transform;
            else return;
        }
        HandleBehaviour();
    }

    protected abstract void HandleBehaviour();

    public virtual void TakeDamage(float amount)
    {
        if (_isDead) return;
        _currentHealth -= amount;
        if (_currentHealth <= 0f)
            Die();
    }

    protected virtual void Die()
    {
        _isDead = true;
        if (_agent.isOnNavMesh) _agent.ResetPath();
        _agent.enabled = false;
        _animator.SetBool(HashIsDead, true);
        if (WaveManager.Instance != null) WaveManager.Instance.OnEnemyDied();
        Destroy(gameObject, 3f);
    }

    protected bool PlayerInRange(float range)
    {
        if (_player == null) return false;
        return Vector3.Distance(transform.position, _player.position) <= range;
    }

    protected void FacePlayer()
    {
        if (_player == null) return;
        Vector3 dir = (_player.position - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}