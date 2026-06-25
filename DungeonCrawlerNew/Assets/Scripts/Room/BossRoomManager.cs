using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossRoomManager : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Spawn Effect")]
    [SerializeField] private GameObject magicCirclePrefab;
    [SerializeField] private float circleDelay = 1f;
    [SerializeField] private float spawnRiseDistance = 2f;
    [SerializeField] private float spawnRiseDuration = 1f;

    [Header("Portal")]
    [SerializeField] private GameObject portalPrefab;

    [Header("Trigger")]
    [SerializeField] private float triggerRange = 15f;

    private bool _triggered = false;
    private bool _cleared = false;
    private Transform _player;

    private void Start()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
    }

    private void Update()
    {
        if (_triggered || _cleared) return;

        if (_player == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) _player = playerObj.transform;
            else return;
        }

        if (Vector3.Distance(transform.position, _player.position) <= triggerRange)
        {
            _triggered = true;
            StartCoroutine(SpawnBoss());
        }
    }

    private IEnumerator SpawnBoss()
    {
        if (bossPrefab == null) yield break;

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(spawnPos, out navHit, 5f, NavMesh.AllAreas))
            spawnPos = navHit.position;

        GameObject circle = null;
        if (magicCirclePrefab != null)
            circle = Instantiate(magicCirclePrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(circleDelay);

        Vector3 startPos = spawnPos + Vector3.down * spawnRiseDistance;
        GameObject bossGO = Instantiate(bossPrefab, startPos, Quaternion.identity);

        if (bossGO.TryGetComponent(out BossAgent agent))
        {
            agent.enabled = false;
            agent.SetRoomManager(this);
        }
        if (bossGO.TryGetComponent(out NavMeshAgent navAgent))
            navAgent.enabled = false;

        float elapsed = 0f;
        while (elapsed < spawnRiseDuration)
        {
            elapsed += Time.deltaTime;
            bossGO.transform.position = Vector3.Lerp(startPos, spawnPos, elapsed / spawnRiseDuration);
            yield return null;
        }
        bossGO.transform.position = spawnPos;

        if (agent != null) agent.enabled = true;
        if (navAgent != null) navAgent.enabled = true;

        if (circle != null)
            StartCoroutine(FadeOutCircle(circle));
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

    public void OnBossDefeated()
    {
        if (_cleared) return;
        _cleared = true;

        if (portalPrefab != null)
            Instantiate(portalPrefab, transform.position, Quaternion.identity);

        Debug.Log("Boss defeated — portal spawned!");
    }
}
