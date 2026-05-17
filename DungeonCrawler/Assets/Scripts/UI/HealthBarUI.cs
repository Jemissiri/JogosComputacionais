using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public HealthSegment[] segments;

    [Header("Sprites")]
    public Sprite spriteEmpty;
    public Sprite spriteFull;
    public Sprite[] spritesGaining;
    public Sprite[] spritesLosing;

    int previousFilled;

    void Start()
    {
        foreach (var seg in segments)
        {
            seg.spriteEmpty    = spriteEmpty;
            seg.spriteFull     = spriteFull;
            seg.spritesGaining = spritesGaining;
            seg.spritesLosing  = spritesLosing;
        }

        playerHealth.OnHealthChanged += UpdateBar;

        int filled = FilledCount(playerHealth.currentHealth, playerHealth.maxHealth);
        previousFilled = filled;
        for (int i = 0; i < segments.Length; i++)
            segments[i].Init(i < filled);
    }

    void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateBar;
    }

    void UpdateBar(int current, int max)
    {
        int filled = FilledCount(current, max);
        for (int i = 0; i < segments.Length; i++)
            segments[i].SetState(i < filled);
        previousFilled = filled;
    }

    int FilledCount(int current, int max) =>
        Mathf.RoundToInt((float)current / max * segments.Length);
}
