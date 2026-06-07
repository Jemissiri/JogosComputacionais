using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private float maxRange = 20f;

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
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore the player's own collider
        if (other.CompareTag("Player")) return;

        if (other.CompareTag("Enemy"))
        {
            // TODO: hook up to enemy health
            Destroy(gameObject);
            return;
        }

        // Destroy on anything else (walls, environment)
        if (!other.isTrigger)
            Destroy(gameObject);
    }
}