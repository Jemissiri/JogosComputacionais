using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => maxHealth;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;

    [Header("Death")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 2f;

    private bool _isDead = false;

    private void Start()
    {
        _currentHealth = maxHealth;
        UpdateSlider();
    }

    public void TakeDamage(float amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - amount, 0f, maxHealth);
        UpdateSlider();

        if (_currentHealth <= 0f && !_isDead)
        {
            _isDead = true;
            Die();
        }
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

    public void ResetHealth()
    {
        _currentHealth = maxHealth;
        UpdateSlider();
    }

    private void Die()
    {
        StartCoroutine(DieSequence());
    }

    private IEnumerator DieSequence()
    {
        // Disable player input
        GetComponent<PlayerController>().enabled = false;
        GetComponent<AttackController>().enabled = false;

        // Fade to black
        float elapsed = 0f;
        Color c = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeImage.color = new Color(c.r, c.g, c.b, elapsed / fadeDuration);
            yield return null;
        }

        // Load end screen
        SceneManager.LoadScene("EndScreen");
    }
}