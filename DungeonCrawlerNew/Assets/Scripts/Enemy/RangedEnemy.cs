using System.Collections;
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

    [SerializeField] private float attackFireDelay = 0.4f;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 4f;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float dashTriggerDistance = 3f;

    private float _lastAttackTime;
    private float _lastDashTime;
    private bool _isDashing;

    protected override void HandleBehaviour()
    {
        if (_isDashing) return;

        if (!PlayerInRange(detectionRange))
        {
            Idle();
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, _player.position);

        // Dash away if player gets too close
        if (distToPlayer < dashTriggerDistance && Time.time - _lastDashTime >= dashCooldown)
        {
            StartCoroutine(DashAway());
            return;
        }

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
        if (_agent.isOnNavMesh) _agent.ResetPath();
        _animator.SetBool(HashIsMoving, false);
    }

    private void MoveCloser()
    {
        if (_agent.isOnNavMesh) _agent.SetDestination(_player.position);
        _animator.SetBool(HashIsMoving, true);
    }

    private void BackAway()
    {
        Vector3 dir = (transform.position - _player.position).normalized;
        Vector3 backPos = transform.position + dir * preferredDistance;
        if (_agent.isOnNavMesh) _agent.SetDestination(backPos);
        _animator.SetBool(HashIsMoving, true);
    }

    private void Attack()
    {
        if (_agent.isOnNavMesh) _agent.ResetPath();
        FacePlayer();
        _animator.SetBool(HashIsMoving, false);

        if (Time.time - _lastAttackTime < attackCooldown) return;
        _lastAttackTime = Time.time;
        _animator.SetTrigger(HashIsAttacking);
        StartCoroutine(FireAfterDelay());
    }

    private IEnumerator FireAfterDelay()
    {
        yield return new WaitForSeconds(attackFireDelay);
        if (!_isDead) FireProjectile();
    }

    private IEnumerator DashAway()
    {
        _isDashing = true;
        _lastDashTime = Time.time;

        // Direction away from player with random angle offset
        Vector3 awayDir = (transform.position - _player.position).normalized;
        float randomAngle = Random.Range(-60f, 60f);
        awayDir = Quaternion.Euler(0f, randomAngle, 0f) * awayDir;
        awayDir.y = 0f;

        Vector3 dashTarget = transform.position + awayDir * dashDistance;

        // Disable agent and move manually
        _agent.enabled = false;
        float elapsed = 0f;
        float duration = dashDistance / dashSpeed;
        Vector3 startPos = transform.position;

        _animator.SetBool(HashIsMoving, true);

        while (elapsed < duration && !_isDead)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, dashTarget, elapsed / duration);
            yield return null;
        }

        // Re-enable agent
        _agent.enabled = true;
        if (_agent.isOnNavMesh)
            _agent.Warp(transform.position);

        _animator.SetBool(HashIsMoving, false);
        _isDashing = false;
    }

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