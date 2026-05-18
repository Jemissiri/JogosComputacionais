using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public Image fillImage;

    public Color fillColor       = new Color(0.85f, 0.15f, 0.15f);
    public Color backgroundColor = new Color(0.15f, 0.15f, 0.15f);
    public Image backgroundImage;

    Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (backgroundImage != null) backgroundImage.color = backgroundColor;
        if (fillImage != null)       fillImage.color = fillColor;

        enemyHealth.OnHealthChanged += UpdateBar;
        UpdateBar(1f);
    }

    void OnDestroy()
    {
        enemyHealth.OnHealthChanged -= UpdateBar;
    }

    void LateUpdate()
    {
        transform.forward = cam.transform.forward;
    }

    void UpdateBar(float normalizedHealth)
    {
        if (fillImage != null)
            fillImage.fillAmount = normalizedHealth;
    }
}
