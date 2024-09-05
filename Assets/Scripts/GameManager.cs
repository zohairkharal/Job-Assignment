using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static System.Action<int> UpdateScore, UpdateMoves;
    public static System.Action YouLose, YouWin, ResetHUD;

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject cardContainer;
    [SerializeField] private Sprite cardBack;
    [SerializeField] private Sprite[] cardSprites;

    [SerializeField] private MenuManager uiHandler;

    public Sprite GetSprite(int id) => cardSprites[id];
    public Sprite GetCardBackSprite() => cardBack;
    public bool IsGameRunning() => isGameRunning;
    public int GetGameTime() => gameTime * 10;

    private Card[] cards;
    private List<GameObject> cardGameObjects = new List<GameObject>();
    private int selectedCardCount;
    private int matchedPairs;
    private int gameTime;
    private bool isGameRunning;

    private int previousCardSpriteID;
    private int previousCardID;
    private int currentCombo;
    private int moveCount;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartGame()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        selectedCardCount = 0;
        matchedPairs = 0;
        previousCardSpriteID = -1;
        previousCardID = -1;
        currentCombo = 0;
        moveCount = 0;

        ResetHUD?.Invoke();
        SetupCards();
        PeekCardsAtStart();
        StartCoroutine(HideAllCards());
    }

    private void SetupCards()
    {
        foreach (var card in cardGameObjects)
        {
            Destroy(card);
        }
        cardGameObjects = new List<GameObject>();
        Vector2 gameSize = uiHandler.GetGridSize();
        int totalCards = (int)(gameSize.x * gameSize.y);
        int pairs = totalCards / 2;
        gameTime = (int)gameSize.x;
        cards = new Card[totalCards];
        List<int> spriteIDs = new List<int>();
        for (int i = 0; i < pairs; i++)
        {
            spriteIDs.Add(i);
            spriteIDs.Add(i);
        }

        for (int i = 0; i < totalCards; i++)
        {
            int index = Random.Range(0, spriteIDs.Count);
            int spriteID = spriteIDs[index];
            spriteIDs.RemoveAt(index);

            GameObject newCard = Instantiate(cardPrefab, cardContainer.transform);
            cardGameObjects.Add(newCard);
            cards[i] = newCard.GetComponent<Card>();
            cards[i].ID = i;
            cards[i].SpriteID = spriteID;
        }
    }

    private void PeekCardsAtStart()
    {
        foreach (var item in cards)
        {
            item.FlipCard();
        }
    }
    private IEnumerator HideAllCards()
    {

        yield return new WaitForSeconds(1f);
        foreach (Card card in cards)
        {
            card.FlipCard();
        }
        isGameRunning = true;
    }

    private async Task CallWithDelayAsync(float delayInSeconds)
    {
        await Task.Delay((int)(delayInSeconds * 1000));
    }

    public async Task OnCardSelectedAsync(Card card)
    {
        await CallWithDelayAsync(1f);
        if (selectedCardCount == 0)
        {
            previousCardSpriteID = card.SpriteID;
            previousCardID = card.ID;
            selectedCardCount++;
        }
        else if (selectedCardCount == 1)
        {
            if (card.SpriteID == previousCardSpriteID)
            {
                cards[previousCardID].SetInactive();
                card.SetInactive();
                matchedPairs++;
                UpdateScore?.Invoke(++currentCombo);
                if (matchedPairs == cards.Length / 2) StartCoroutine(GameOver(true));

            }
            else
            {
                cards[previousCardID].FlipCard();
                card.FlipCard();
                currentCombo = 0;
                AudioManager.Instance.PlaySoundFX(SoundFX.Unmatch);
            }
            UpdateMoves?.Invoke(++moveCount);
            selectedCardCount = 0;
        }
    }


    public IEnumerator GameOver(bool isWinner)
    {
        if (!isGameRunning) yield break;

        isGameRunning = false;
        if (isWinner) YouWin?.Invoke();
        else YouLose?.Invoke();

        yield return new WaitForSeconds(3f);
        uiHandler.SwitchScreens(UIScreen.Menu);
    }
    public void QuitGame()
    {
        if (!isGameRunning) return;

        isGameRunning = false;
        uiHandler.SwitchScreens(UIScreen.Menu);
        AudioManager.Instance.PlaySoundFX(SoundFX.Click);
    }
    public bool CanSelectCard()
    {
        return isGameRunning && selectedCardCount < 2;
    }
}
