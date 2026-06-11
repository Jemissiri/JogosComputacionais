using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    [Header("Melee")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;

    private float _lastAttackTime;

    protected override void HandleBehaviour()
    {
        if (PlayerInRange(attackRange))
            Attack();
        else if (PlayerInRange(detectionRange))
            ChasePlayer();
        else
            Idle();
    }

    private void ChasePlayer()
    {
        _agent.SetDestination(_player.position);
        _animator.SetBool(HashIsMoving, true);
    }

    private void Idle()
    {
        _agent.ResetPath();
        _animator.SetBool(HashIsMoving, false);
    }

    private void Attack()
    {
        _agent.ResetPath();
        FacePlayer();
        _animator.SetBool(HashIsMoving, false);

        if (Time.time - _lastAttackTime < attackCooldown) return;
        _lastAttackTime = Time.time;
        _animator.SetTrigger(HashIsAttacking); // Trigger fires once and auto-resets
    }

    // Called by animation event on Attack clip
    public void DealDamage()
    {
        if (PlayerInRange(attackRange) && _player.TryGetComponent(out PlayerHealth ph))
            ph.TakeDamage(attackDamage);
    }
}