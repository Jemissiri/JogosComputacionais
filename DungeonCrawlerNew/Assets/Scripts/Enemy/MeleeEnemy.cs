using System.Collections;
using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    [Header("Melee")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackStartDelay = 0.2f;
    [SerializeField] private float attackHitDelay = 0.4f;

    private float _lastAttackTime;
    private bool _isAttacking;

    protected override void HandleBehaviour()
    {
        if (_isAttacking) return;

        if (PlayerInRange(attackRange))
            Attack();
        else if (PlayerInRange(detectionRange))
            ChasePlayer();
        else
            Idle();
    }

    private void ChasePlayer()
    {
        if (_agent.isOnNavMesh) _agent.SetDestination(_player.position);
        _animator.SetBool(HashIsMoving, true);
    }

    private void Idle()
    {
        if (_agent.isOnNavMesh) _agent.ResetPath();
        _animator.SetBool(HashIsMoving, false);
    }

    private void Attack()
    {
        if (_agent.isOnNavMesh) _agent.ResetPath();
        FacePlayer();
        _animator.SetBool(HashIsMoving, false);

        if (Time.time - _lastAttackTime < attackCooldown) return;
        _lastAttackTime = Time.time;
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        _isAttacking = true;

        yield return new WaitForSeconds(attackStartDelay);
        _animator.SetTrigger(HashIsAttacking);

        yield return new WaitForSeconds(attackHitDelay);
        if (!_isDead && PlayerInRange(attackRange) && _player.TryGetComponent(out PlayerHealth ph))
            ph.TakeDamage(attackDamage);

        yield return new WaitForSeconds(attackCooldown - attackStartDelay - attackHitDelay);
        _isAttacking = false;
    }
}