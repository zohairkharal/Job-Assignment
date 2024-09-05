using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private int spriteID;
    private int id;
    private bool isFlipped;
    private bool isTurning;
    private CardVisual cardVisual;
    private Button cardBtn;

    private void Awake()
    {
        cardVisual = GetComponent<CardVisual>();
        cardBtn = GetComponent<Button>();
        cardBtn?.onClick.AddListener(OnCardClickedAsync);
    }

    private void Start()
    {
        ResetCard();
    }

    public int SpriteID
    {
        get => spriteID;
        set
        {
            spriteID = value;
            cardVisual.UpdateCardSprite(spriteID, isFlipped);
        }
    }

    public int ID
    {
        get => id;
        set => id = value;
    }

    public void OnCardClickedAsync()
    {
        if (isFlipped || isTurning || !GameManager.Instance.CanSelectCard()) return;
        FlipCard();
        AudioManager.Instance.PlaySoundFX(SoundFX.Flip);
        _ = GameManager.Instance.OnCardSelectedAsync(this);
    }

    public void FlipCard()
    {
        if (!isTurning)
        {
            isTurning = true;
            StartCoroutine(cardVisual.FlipAnimation(() =>
            {
                isFlipped = !isFlipped;
                isTurning = false;
                cardVisual.UpdateCardSprite(SpriteID, isFlipped);
            }));
        }
    }

    public void SetInactive()
    {
        cardVisual.FadeOut(() => cardVisual.DisableImage());
    }

    public void ResetCard()
    {
        isFlipped = false;
        isTurning = false;
        cardVisual.ResetVisual();
    }
}
