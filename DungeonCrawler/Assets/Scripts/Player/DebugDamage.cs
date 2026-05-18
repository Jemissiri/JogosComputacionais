using UnityEngine;
using UnityEngine.InputSystem;

public class DebugDamage : MonoBehaviour
{
    public int damageAmount = 10;

    PlayerHealth playerHealth;
    EnemyHealth  enemyHealth;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        enemyHealth  = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (playerHealth != null) playerHealth.TakeDamage(damageAmount);
            else if (enemyHealth != null) enemyHealth.TakeDamage(damageAmount);
        }
    }
}
