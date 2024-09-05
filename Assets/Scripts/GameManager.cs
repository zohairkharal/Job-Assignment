using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject cardContainer;
    [SerializeField] private Sprite cardBack;
    [SerializeField] private Sprite[] cardSprites;
    [SerializeField] private TMP_Text timeLabel;
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private TMP_Text movesLabel;
    [SerializeField] private MenuManager uiHandler;

    private Card[] cards;
    private int selectedCardCount;
    private int matchedPairs;
    private float gameTime;
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
        isGameRunning = true;
        selectedCardCount = 0;
        matchedPairs = 0;
        previousCardSpriteID = -1;
        previousCardID = -1;
        currentCombo = 0;
        scoreLabel.text = "Score: " + 0;

        SetupCards();
        PeekCardsAtStart();
        StartCoroutine(HideAllCards());
        StartTimer();
    }

    private void SetupCards()
    {
        Vector2 gameSize = uiHandler.GetGridSize();
        int totalCards = (int)(gameSize.x * gameSize.y);
        int pairs = totalCards / 2;

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
            //card.ResetCard();
            card.FlipCard();
        }
    }

    public Sprite GetSprite(int id) => cardSprites[id];
    public Sprite GetCardBackSprite() => cardBack;

    private void StartTimer()
    {
        StartCoroutine(GameTimer());
    }

    private IEnumerator GameTimer()
    {
        gameTime = 0f;
        while (isGameRunning)
        {
            gameTime += Time.deltaTime;
            timeLabel.text = "Time: " + Mathf.RoundToInt(gameTime);
            yield return null;
        }
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
                UpdateScore();
                if (matchedPairs == cards.Length / 2) EndGame();
            }
            else
            {
                cards[previousCardID].FlipCard();
                card.FlipCard();
                currentCombo = 0;
            }
            UpdateMoveCount();
            selectedCardCount = 0;
        }
    }

    private void UpdateScore()
    {
        currentCombo++;
        int points = currentCombo > 1 ? currentCombo * 2 : 1;
        scoreLabel.text = "Score: " + points;
    }
    private void UpdateMoveCount()
    {
        moveCount++;
        scoreLabel.text = "Total Moves: " + moveCount;
    }
    private void EndGame()
    {
        isGameRunning = false;
        uiHandler.SwitchScreens(UIScreen.Menu);
    }

    public bool CanSelectCard()
    {
        return isGameRunning && selectedCardCount < 2;
    }
}
