using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveProgressUI : MonoBehaviour
{
    [Header("Wave")]
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Enemies")]
    [SerializeField] private Slider enemyProgressBar;
    [SerializeField] private TextMeshProUGUI enemiesText;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateWave(int currentWave, int totalWaves)
    {
        waveText.text = $"Wave {currentWave} / {totalWaves}";
    }

    public void UpdateEnemies(int enemiesAlive, int totalEnemies)
    {
        enemiesText.text = $"{enemiesAlive} enemies remaining";
        enemyProgressBar.value = totalEnemies > 0
            ? (float)enemiesAlive / totalEnemies
            : 0f;
    }
}