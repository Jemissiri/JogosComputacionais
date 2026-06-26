using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Border")]
    [SerializeField] private Image borderImage;
    [SerializeField] private float fadeDuration = 0.15f;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0.86f, 0.71f, 0.31f, 0f);
    [SerializeField] private Color hoverColor = new Color(0.86f, 0.71f, 0.31f, 1f);
    [SerializeField] private Color pressedColor = new Color(1f, 0.9f, 0.4f, 1f);

    private Coroutine _fadeCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        FadeBorder(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FadeBorder(normalColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        FadeBorder(pressedColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        FadeBorder(hoverColor);
    }

    private void FadeBorder(Color targetColor)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeCoroutine(targetColor));
    }

    private System.Collections.IEnumerator FadeCoroutine(Color targetColor)
    {
        if (borderImage == null) yield break;

        Color startColor = borderImage.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            borderImage.color = Color.Lerp(startColor, targetColor, elapsed / fadeDuration);
            yield return null;
        }

        borderImage.color = targetColor;
    }
}