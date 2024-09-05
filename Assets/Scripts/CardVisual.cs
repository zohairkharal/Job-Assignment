using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class CardVisual : MonoBehaviour
{
    private Image cardImage;

    private void Awake()
    {
        cardImage = GetComponent<Image>();
    }

    public void UpdateCardSprite(int spriteID, bool isFlipped)
    {
        if (spriteID == -1) return;
        cardImage.sprite = isFlipped ? GameManager.Instance.GetSprite(spriteID) : GameManager.Instance.GetCardBackSprite();
    }

    public IEnumerator FlipAnimation(Action onComplete)
    {
        float duration = 0.05f;
        Quaternion startRotation = transform.rotation;
        Quaternion halfRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, halfRotation, elapsed / duration);
            yield return null;
        }

        onComplete?.Invoke();
        elapsed = 0f;
        startRotation = transform.rotation;
        halfRotation = transform.rotation * Quaternion.Euler(0, 90, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, halfRotation, elapsed / duration);
            yield return null;
        }
    }

    public void DisableImage()
    {
        cardImage.enabled = false;
    }
    public void FadeOut(Action onComplete)
    {
        float duration = 0.5f;
        Color startColor = cardImage.color;
        cardImage.DOColor(Color.clear, duration).OnComplete(() => { onComplete?.Invoke(); });

    }

    public void ResetVisual()
    {
        cardImage.color = Color.white;
        transform.rotation = Quaternion.Euler(0, 180, 0);
        //UpdateCardSprite(-1, false);
    }
}
