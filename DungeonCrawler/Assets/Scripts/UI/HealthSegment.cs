using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthSegment : MonoBehaviour
{
    [HideInInspector] public Sprite spriteEmpty;
    [HideInInspector] public Sprite spriteFull;
    [HideInInspector] public Sprite[] spritesGaining;
    [HideInInspector] public Sprite[] spritesLosing;

    public float fullScale  = 3f;
    public float emptyScale = 2f;

    Image image;
    bool isFull;
    Coroutine anim;

    const float FrameTime = 0.06f;

    void Awake() => image = GetComponent<Image>();

    public void Init(bool startFull)
    {
        isFull = startFull;
        image.sprite = startFull ? spriteFull : spriteEmpty;
        transform.localScale = Vector3.one * (startFull ? fullScale : emptyScale);
    }

    public void SetState(bool full)
    {
        if (isFull == full) return;

        bool wasFullBefore = isFull;
        isFull = full;

        if (anim != null) StopCoroutine(anim);

        if (full && !wasFullBefore)
            anim = StartCoroutine(Animate(spritesGaining, spriteFull, fullScale));
        else
            anim = StartCoroutine(Animate(spritesLosing, spriteEmpty, emptyScale));
    }

    IEnumerator Animate(Sprite[] frames, Sprite finalSprite, float finalScale)
    {
        foreach (var frame in frames)
        {
            image.sprite = frame;
            yield return new WaitForSeconds(FrameTime);
        }
        image.sprite = finalSprite;
        transform.localScale = Vector3.one * finalScale;
        anim = null;
    }
}
