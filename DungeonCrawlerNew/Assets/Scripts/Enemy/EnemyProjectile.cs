using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{
    private float _damage;
    private float _maxRange = 20f;
    private Vector3 _startPosition;
    private Rigidbody _rb;
    private BossAgent _bossAgent;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(Vector3 direction, float speed, float damage, BossAgent bossAgent = null)
    {
        _damage = damage;
        _bossAgent = bossAgent;
        _startPosition = transform.position;
        _rb.linearVelocity = direction * speed;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _startPosition) >= _maxRange)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (other.CompareTag("Enemy")) return;

        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerHealth ph))
                ph.TakeDamage(_damage);

            if (_bossAgent != null)
                _bossAgent.AddReward(0.2f);
        }

        Destroy(gameObject);
    }
}