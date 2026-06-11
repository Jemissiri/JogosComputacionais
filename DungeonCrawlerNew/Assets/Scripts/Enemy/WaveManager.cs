using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [System.Serializable]
    public class EnemyWaveEntry
    {
        public GameObject enemyPrefab;
        public int count;
    }

    [System.Serializable]
    public class Wave
    {
        public List<EnemyWaveEntry> enemies;
    }

    [Header("Waves")]
    [SerializeField] private List<Wave> waves;

    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject magicCirclePrefab;
    [SerializeField] private float timeBetweenWaves = 2f;
    [SerializeField] private float spawnRiseDistance = 2f;
    [SerializeField] private float spawnRiseDuration = 1f;
    [SerializeField] private float circleDelay = 1f;

    [Header("Door")]
    [SerializeField] private GameObject door;

    private int _currentWave = 0;
    private int _enemiesAlive = 0;
    private bool _spawning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(SpawnWave(_currentWave));
    }

    private IEnumerator SpawnWave(int waveIndex)
    {
        _spawning = true;
        Wave wave = waves[waveIndex];

        // Count total enemies in wave
        foreach (var entry in wave.enemies)
            _enemiesAlive += entry.count;

        // Spawn each enemy type
        foreach (var entry in wave.enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                StartCoroutine(SpawnEnemy(entry.enemyPrefab, spawnPoint));
                yield return new WaitForSeconds(0.5f);
            }
        }

        _spawning = false;
    }

    private IEnumerator SpawnEnemy(GameObject prefab, Transform spawnPoint)
    {
        // Spawn magic circle
        GameObject circle = null;
        if (magicCirclePrefab != null)
            circle = Instantiate(magicCirclePrefab, spawnPoint.position, Quaternion.identity);

        yield return new WaitForSeconds(circleDelay);

        // Spawn enemy below ground
        Vector3 startPos = spawnPoint.position + Vector3.down * spawnRiseDistance;
        GameObject enemy = Instantiate(prefab, startPos, spawnPoint.rotation);

        // Disable AI while rising
        if (enemy.TryGetComponent(out BaseEnemy baseEnemy))
            baseEnemy.enabled = false;
        if (enemy.TryGetComponent(out UnityEngine.AI.NavMeshAgent agent))
            agent.enabled = false;

        // Rise up
        float elapsed = 0f;
        Vector3 targetPos = spawnPoint.position;
        while (elapsed < spawnRiseDuration)
        {
            elapsed += Time.deltaTime;
            enemy.transform.position = Vector3.Lerp(startPos, targetPos, 
                                                     elapsed / spawnRiseDuration);
            yield return null;
        }
        enemy.transform.position = targetPos;

        // Enable AI
        if (baseEnemy != null) baseEnemy.enabled = true;
        if (agent != null) agent.enabled = true;

        // Fade out magic circle
        if (circle != null) StartCoroutine(FadeOutCircle(circle));
    }

    private IEnumerator FadeOutCircle(GameObject circle)
    {
        float duration = 1f;
        float elapsed = 0f;
        Renderer rend = circle.GetComponent<Renderer>();

        if (rend != null)
        {
            var mpb = new MaterialPropertyBlock();
            rend.GetPropertyBlock(mpb);
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                mpb.SetColor("_BaseColor", new Color(1f, 1f, 1f, alpha));
                rend.SetPropertyBlock(mpb);
                yield return null;
            }
        }

        Destroy(circle);
    }

    public void OnEnemyDied()
    {
        _enemiesAlive--;

        if (_enemiesAlive > 0) return;

        _currentWave++;

        if (_currentWave >= waves.Count)
        {
            // All waves cleared — unlock door
            UnlockDoor();
            return;
        }

        // Start next wave after delay
        StartCoroutine(NextWaveDelay());
    }

    private IEnumerator NextWaveDelay()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine(SpawnWave(_currentWave));
    }

    private void UnlockDoor()
    {
        Debug.Log("All waves cleared — door unlocked!");
        if (door != null)
            door.SetActive(false); // or play door open animation
    }
}