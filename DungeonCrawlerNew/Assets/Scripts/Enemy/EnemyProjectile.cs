using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 _direction;
    private float _speed;
    private float _damage;
    private float _maxRange = 20f;
    private Vector3 _startPosition;

    public void Init(Vector3 direction, float speed, float damage)
    {
        _direction = direction;
        _speed = speed;
        _damage = damage;
        _startPosition = transform.position;
    }

    private void Update()
    {
        transform.position += _direction * _speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _startPosition) >= _maxRange)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerHealth ph))
                ph.TakeDamage(_damage);
            Destroy(gameObject);
        }

        if (other.CompareTag("Wall"))
            Destroy(gameObject);
    }
}