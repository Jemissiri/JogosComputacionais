using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PortalInteraction : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 2f;

    private Image _fadeImage;
    private bool _transitioning = false;

    private void Start()
    {
        // Find the FadeImage automatically by name
        _fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_transitioning)
            StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        _transitioning = true;

        float elapsed = 0f;
        Color c = _fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _fadeImage.color = new Color(c.r, c.g, c.b, elapsed / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene("WinScreen");
    }
}