using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class AttackController : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform spawnPointL;
    [SerializeField] private Transform spawnPointR;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 15f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.5f;

    private Animator _animator;
    private Camera _camera;
    private float _lastAttackTime;
    private bool _nextIsLeft = true;
    private Vector3 _pendingDirection;

    private static readonly int HashAttackL = Animator.StringToHash("AttackL");
    private static readonly int HashAttackR = Animator.StringToHash("AttackR");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _camera   = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryAttack();
    }

    private void TryAttack()
    {
        if (Time.time - _lastAttackTime < attackCooldown) return;
        _lastAttackTime = Time.time;

        // Compute and store direction now so the animation event can use it
        _pendingDirection = GetMouseDirection();

        _animator.SetTrigger(_nextIsLeft ? HashAttackL : HashAttackR);
        _nextIsLeft = !_nextIsLeft;
    }

    // Called by Animation Event on Attack_L clip
    public void SpawnProjectileLeft()  => SpawnProjectile(spawnPointL);

    // Called by Animation Event on Attack_R clip
    public void SpawnProjectileRight() => SpawnProjectile(spawnPointR);

    private void SpawnProjectile(Transform spawnPoint)
    {
        if (projectilePrefab == null || spawnPoint == null) return;
        if (_pendingDirection == Vector3.zero) return;

        GameObject proj = Instantiate(projectilePrefab,
                                      spawnPoint.position,
                                      Quaternion.LookRotation(_pendingDirection));
        if (proj.TryGetComponent(out Projectile projectile))
            projectile.Init(_pendingDirection, projectileSpeed);
        else
            Destroy(proj);
    }

    private Vector3 GetMouseDirection()
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 target = ray.GetPoint(distance);
            Vector3 dir = target - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
                return dir.normalized;
        }
        return transform.forward;
    }
}
