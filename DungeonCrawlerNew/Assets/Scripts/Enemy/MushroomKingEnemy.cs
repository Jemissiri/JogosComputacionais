using System.Collections;
using UnityEngine;

public class MushroomKingEnemy : BaseEnemy
{
    [Header("Ranged")]
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float preferredDistance = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackFireDelay = 0.4f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float backAwayRotateSpeed = 120f;

    private float _lastAttackTime;
    private bool _isRotatingToShoot;

    protected override void Awake()
    {
        base.Awake();
        detectionRange = 20f;
        _agent.stoppingDistance = preferredDistance;
    }

    protected override void HandleBehaviour()
    {
        if (!PlayerInRange(detectionRange))
        {
            Idle();
            return;
        }

        float dist = Vector3.Distance(transform.position, _player.position);

        if (dist < preferredDistance - 1f)
            BackAway();
        else if (dist <= attackRange)
            Attack();
        else
            Chase();
    }

    private void Idle()
    {
        if (_agent.isOnNavMesh) _agent.isStopped = true;
        _animator.SetBool(HashIsMoving, false);
    }

    private void Chase()
    {
        if (_agent.isOnNavMesh)
        {
            _agent.stoppingDistance = preferredDistance;
            _agent.isStopped = false;
            _agent.SetDestination(_player.position);
        }
        _animator.SetBool(HashIsMoving, true);
    }

    private void BackAway()
    {
        // Fire once per cooldown while backing away
        if (!_isRotatingToShoot && Time.time - _lastAttackTime >= attackCooldown)
        {
            _lastAttackTime = Time.time;
            StartCoroutine(RotateAndFire());
        }

        Vector3 dir = transform.position - _player.position;
        if (dir.sqrMagnitude < 0.01f) dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();

        if (_agent.isOnNavMesh)
        {
            _agent.stoppingDistance = 0f;
            _agent.isStopped = false;
            _agent.SetDestination(_player.position + dir * preferredDistance);
        }
        _animator.SetBool(HashIsMoving, true);
    }

    private void Attack()
    {
        if (_agent.isOnNavMesh) _agent.isStopped = true;
        FacePlayer();
        _animator.SetBool(HashIsMoving, false);

        if (Time.time - _lastAttackTime < attackCooldown) return;
        _lastAttackTime = Time.time;

        _animator.SetTrigger(HashIsAttacking);
        StartCoroutine(FireAfterDelay());
    }

    private IEnumerator RotateAndFire()
    {
        _isRotatingToShoot = true;

        // Disable agent rotation so we can control it manually
        _agent.updateRotation = false;

        while (!_isDead)
        {
            Vector3 dir = (_player.position - transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f) break;

            Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, backAwayRotateSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRot) < 3f) break;
            yield return null;
        }

        if (!_isDead) FireProjectile();

        _agent.updateRotation = true;
        _isRotatingToShoot = false;
    }

    private IEnumerator FireAfterDelay()
    {
        yield return new WaitForSeconds(attackFireDelay);
        if (!_isDead) FireProjectile();
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;

        Transform spawnFrom = projectileSpawnPoint != null ? projectileSpawnPoint : transform;
        Vector3 dir = (_player.position - spawnFrom.position);
        dir.y = 0f;
        dir.Normalize();

        GameObject proj = Instantiate(projectilePrefab, spawnFrom.position, Quaternion.LookRotation(dir));
        if (proj.TryGetComponent(out EnemyProjectile ep))
            ep.Init(dir, projectileSpeed, attackDamage);
        else
            Destroy(proj);
    }
}
