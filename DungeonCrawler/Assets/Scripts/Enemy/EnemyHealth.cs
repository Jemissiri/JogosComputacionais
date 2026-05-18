using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }

    public event Action<float> OnHealthChanged;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);
        if (currentHealth == 0) Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);
    }

    public Color deathEffectColor = new Color(0.5f, 0.05f, 0.05f);

    void Die()
    {
        EnemyDeathEffect.Spawn(transform.position + Vector3.up, deathEffectColor);
        Destroy(gameObject);
    }
}
