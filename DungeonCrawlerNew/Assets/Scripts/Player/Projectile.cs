using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private float maxRange = 20f;
    [SerializeField] private GameObject hitEffectPrefab;

    private Vector3 _direction;
    private float _speed;
    private Vector3 _startPosition;

    public void Init(Vector3 direction, float speed)
    {
        _direction = direction;
        _speed = speed;
        _startPosition = transform.position;
    }

    private void Update()
    {
        if (_speed == 0f) return;

        transform.position += _direction * _speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _startPosition) >= maxRange)
        {
            SpawnHitEffect();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        if (other.isTrigger) return;

        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out BaseEnemy enemy))
                enemy.TakeDamage(damage);
            else if (other.transform.parent != null && other.transform.parent.TryGetComponent(out BaseEnemy parentEnemy))
                parentEnemy.TakeDamage(damage);
            else if (other.TryGetComponent(out BossAgent boss))
                boss.TakeDamage(damage);
            else if (other.transform.parent != null && other.transform.parent.TryGetComponent(out BossAgent parentBoss))
                parentBoss.TakeDamage(damage);
        }

        SpawnHitEffect();
        Destroy(gameObject);
    }

    private void SpawnHitEffect()
    {
        if (hitEffectPrefab == null) return;

        GameObject hit = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        // Auto-destroy hit effect once all particles finish
        if (hit.TryGetComponent(out ParticleSystem ps))
            Destroy(hit, ps.main.duration + ps.main.startLifetime.constantMax);
        else
            Destroy(hit, 3f);
    }
}