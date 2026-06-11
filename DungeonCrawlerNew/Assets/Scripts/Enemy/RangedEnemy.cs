using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    [Header("Ranged")]
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private float preferredDistance = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileSpeed = 10f;

    private float _lastAttackTime;

    protected override void HandleBehaviour()
    {
        if (!PlayerInRange(detectionRange))
        {
            Idle();
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, _player.position);

        // Too close — back away
        if (distToPlayer < preferredDistance)
            BackAway();
        // In attack range — attack
        else if (distToPlayer <= attackRange)
            Attack();
        // Too far — move closer
        else
            MoveCloser();
    }

    private void Idle()
    {
        _agent.ResetPath();
        _animator.SetBool(HashIsMoving, false);
        _animator.SetBool(HashIsAttacking, false);
    }

    private void MoveCloser()
    {
        _agent.SetDestination(_player.position);
        _animator.SetBool(HashIsMoving, true);
        _animator.SetBool(HashIsAttacking, false);
    }

    private void BackAway()
    {
        Vector3 dir = (transform.position - _player.position).normalized;
        Vector3 backPos = transform.position + dir * preferredDistance;
        _agent.SetDestination(backPos);
        _animator.SetBool(HashIsMoving, true);
        _animator.SetBool(HashIsAttacking, false);
    }

    private void Attack()
    {
        _agent.ResetPath();
        FacePlayer();
        _animator.SetBool(HashIsMoving, false);

        if (Time.time - _lastAttackTime < attackCooldown) return;
        _lastAttackTime = Time.time;
        _animator.SetBool(HashIsAttacking, true);
    }

    // Called by animation event on Attack clip
    public void FireProjectile()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null) return;

        Vector3 dir = (_player.position - projectileSpawnPoint.position);
        dir.y = 0f;
        dir.Normalize();

        GameObject proj = Instantiate(projectilePrefab,
                                      projectileSpawnPoint.position,
                                      Quaternion.LookRotation(dir));

        if (proj.TryGetComponent(out EnemyProjectile ep))
            ep.Init(dir, projectileSpeed, attackDamage);
        else
            Destroy(proj);
    }
}