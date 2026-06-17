using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;

    private void Start()
    {
        _currentHealth = maxHealth;
        UpdateSlider();
    }

    public void TakeDamage(float amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - amount, 0f, maxHealth);
        UpdateSlider();

        if (_currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0f, maxHealth);
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        if (healthSlider != null)
            healthSlider.value = _currentHealth;
    }

    private void Die()
    {
        Debug.Log("Player died!");
        // hook it up to the game over screen after
    }

    public void ResetHealth()
    {
        _currentHealth = maxHealth;
        UpdateSlider();
    }

    // For testing purposes only
    // private void Update()
    // {
    //     if (Keyboard.current.spaceKey.wasPressedThisFrame)
    //     GetComponent<PlayerHealth>().TakeDamage(10f);
    // }
}