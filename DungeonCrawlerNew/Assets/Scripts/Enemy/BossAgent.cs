using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;

public class BossAgent : Agent
{
    [Header("Boss Stats")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float meleeAttackDamage = 25f;
    [SerializeField] private float meleeCooldown = 1.5f;
    [SerializeField] private float rangedRange = 10f;
    [SerializeField] private float rangedDamage = 15f;
    [SerializeField] private float rangedCooldown = 2f;
    [SerializeField] private float rangedFireDelay = 0.35f;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float meleeHitDelay = 0.4f;
    [SerializeField] private float maxHealth = 300f;

    [Header("Phase 2")]
    [SerializeField] private float phase2Threshold = 0.5f;
    [SerializeField] private float phase2SpeedBonus = 1.5f;
    [SerializeField] private float phase2DamageBonus = 1.3f;

    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform playerTransform;

    private NavMeshAgent _agent;
    private Animator _animator;
    private float _currentHealth;
    private float _lastMeleeTime;
    private float _lastRangedTime;
    private bool _isPhase2;
    private BossRoomManager _roomManager;
    private Vector3 _arenaOrigin = Vector3.zero;
    private bool _isDead = false;

    private static readonly int HashIsMoving     = Animator.StringToHash("IsMoving");
    private static readonly int HashIsAttacking   = Animator.StringToHash("IsAttacking");
    private static readonly int HashIsAttacking2  = Animator.StringToHash("IsAttacking2");
    private static readonly int HashIsDead        = Animator.StringToHash("IsDead");

    public override void Initialize()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (playerTransform == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }
    }

    public void SetRoomManager(BossRoomManager manager)
    {
        _roomManager = manager;
        _arenaOrigin = transform.position;
    }

    public void ActivateBoss()
    {
        // Called by BossRoomManager when the player enters the room.
        // In inference mode the agent is already running; this is a hook for
        // any future activation logic (e.g. intro cutscene).
    }

    public override void OnEpisodeBegin()
    {
        _currentHealth = maxHealth;
        _isPhase2 = false;
        _agent.speed = moveSpeed;
        _lastMeleeTime  = -99f;
        _lastRangedTime = -99f;

        // In game mode (_roomManager set) the boss was already placed by BossRoomManager —
        // skip random repositioning so it doesn't teleport after the spawn rise.
        if (_roomManager != null) return;

        Vector3 bossSpawn = new Vector3(Random.Range(-8f, 8f), 0f, Random.Range(-8f, 8f));
        _agent.Warp(bossSpawn);
        transform.position = bossSpawn;

        if (playerTransform != null)
            playerTransform.position = new Vector3(Random.Range(-8f, 8f), 1f, Random.Range(-8f, 8f));

        if (playerTransform != null && playerTransform.TryGetComponent(out PlayerHealth ph))
            ph.ResetHealth();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Offset absolute positions by _arenaOrigin so the model always sees
        // coordinates in the same range it was trained on (~±8 units near zero).
        sensor.AddObservation(transform.position - _arenaOrigin);
        sensor.AddObservation(transform.forward);

        if (playerTransform != null)
        {
            sensor.AddObservation(playerTransform.position - _arenaOrigin);
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            sensor.AddObservation(dirToPlayer);
            sensor.AddObservation(Vector3.Distance(transform.position, playerTransform.position));
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
        }

        sensor.AddObservation(_currentHealth / maxHealth);
        sensor.AddObservation(_isPhase2 ? 1f : 0f);
        sensor.AddObservation(Mathf.Clamp01((Time.time - _lastMeleeTime) / meleeCooldown));
        sensor.AddObservation(Mathf.Clamp01((Time.time - _lastRangedTime) / rangedCooldown));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (_isDead) return;

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 move = new Vector3(moveX, 0f, moveZ).normalized;
        if (_agent.isOnNavMesh)
            _agent.SetDestination(transform.position + move * moveSpeed);

        _animator.SetBool(HashIsMoving, move.sqrMagnitude > 0.1f);

        if (playerTransform != null)
        {
            Vector3 dir = (playerTransform.position - transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        int action = actions.DiscreteActions[0];
        switch (action)
        {
            case 0: break;
            case 1: TryMeleeAttack(); break;
            case 2: TryRangedAttack(); break;
        }

        // Check if player died this step
        if (playerTransform != null && playerTransform.TryGetComponent(out PlayerHealth playerHealth))
        {
            if (playerHealth.CurrentHealth <= 0f)
            {
                OnPlayerDied();
                return;
            }
        }

        // Step penalty to encourage efficiency
        AddReward(-0.001f);

        // Reward for being in ranged sweet spot
        if (playerTransform != null)
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position);
            // Strongly reward staying at ranged distance
            if (dist >= meleeRange * 3f && dist <= rangedRange)
                AddReward(0.003f);
            // Penalize being too close
            else if (dist < meleeRange)
                AddReward(-0.005f);
            // Penalize being too far
            else if (dist > rangedRange)
                AddReward(-0.003f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb == null) return;

        var continuous = actionsOut.ContinuousActions;
        continuous[0] = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        continuous[1] = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);

        var discrete = actionsOut.DiscreteActions;
        discrete[0] = 0;
        if (kb.jKey.isPressed) discrete[0] = 1;
        if (kb.kKey.isPressed) discrete[0] = 2;
    }

    private void TryMeleeAttack()
    {
        if (Time.time - _lastMeleeTime < meleeCooldown) return;
        if (playerTransform == null) return;
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > meleeRange) return;

        _lastMeleeTime = Time.time;
        _animator.SetTrigger(HashIsAttacking2);
        StartCoroutine(MeleeAfterDelay());
    }

    private void TryRangedAttack()
    {
        if (Time.time - _lastRangedTime < rangedCooldown) return;
        if (playerTransform == null) return;
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > rangedRange) return;

        _lastRangedTime = Time.time;
        _animator.SetTrigger(HashIsAttacking);
        StartCoroutine(FireAfterDelay());
        AddReward(0.4f);
    }

    private IEnumerator MeleeAfterDelay()
    {
        yield return new WaitForSeconds(meleeHitDelay);
        if (playerTransform != null && playerTransform.TryGetComponent(out PlayerHealth ph))
        {
            float damage = _isPhase2 ? meleeAttackDamage * phase2DamageBonus : meleeAttackDamage;
            ph.TakeDamage(damage);
            AddReward(0.3f);
        }
    }

    private IEnumerator FireAfterDelay()
    {
        yield return new WaitForSeconds(rangedFireDelay);
        FireProjectile();
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;
        Transform spawnFrom = projectileSpawnPoint != null ? projectileSpawnPoint : transform;

        Vector3 dir = (playerTransform.position - spawnFrom.position);
        dir.y = 0f;
        dir.Normalize();

        GameObject proj = Instantiate(projectilePrefab, spawnFrom.position,
                                      Quaternion.LookRotation(dir));
        if (proj.TryGetComponent(out EnemyProjectile ep))
            ep.Init(dir, projectileSpeed, _isPhase2 ? rangedDamage * phase2DamageBonus : rangedDamage, this);
        else
            Destroy(proj);
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        AddReward(-0.2f);

        if (!_isPhase2 && _currentHealth / maxHealth <= phase2Threshold)
        {
            _isPhase2 = true;
            _agent.speed = moveSpeed * phase2SpeedBonus;
            AddReward(0.1f);
        }

        if (_currentHealth <= 0f)
        {
            _isDead = true;
            _animator.SetBool(HashIsDead, true);
            _roomManager?.OnBossDefeated();

            if (_roomManager != null)
            {
                _agent.isStopped = true;
                _agent.enabled = false;
                StartCoroutine(DieAndDestroy());
            }
            else
            {
                AddReward(-1f);
                EndEpisode();
            }
        }
    }

    public void OnPlayerDied()
    {
        AddReward(1f);
        EndEpisode();
    }

    private IEnumerator DieAndDestroy()
    {
        yield return null; // wait one frame for IsDead to trigger the transition
        yield return new WaitUntil(() =>
        {
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName("Death") && state.normalizedTime >= 0.95f;
        });
        Destroy(gameObject);
    }
}