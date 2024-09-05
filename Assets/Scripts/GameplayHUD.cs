using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class GameplayHUD : MonoBehaviour
{

    [SerializeField] private TMP_Text timeLabel;
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private TMP_Text movesLabel;
    [SerializeField] private GameObject winPopup, losePopup;
    [SerializeField] private TMP_Text comboLabel;
    [SerializeField] private Button QuitGameBtn;

    private int timer, score, moves;

    private void Awake()
    {
        QuitGameBtn.onClick.AddListener(QuitGame);
    }
    private void OnEnable()
    {
        GameManager.UpdateScore += UpdateScore;
        GameManager.UpdateMoves += UpdateMoveCount;
        GameManager.YouLose += ShowFailPopup;
        GameManager.YouWin += ShowWinPopup;
        GameManager.ResetHUD += ResetGameplayHUD;
    }
    private void OnDisable()
    {
        GameManager.UpdateScore -= UpdateScore;
        GameManager.UpdateMoves -= UpdateMoveCount;
        GameManager.YouLose -= ShowFailPopup;
        GameManager.YouWin -= ShowWinPopup;
        GameManager.ResetHUD -= ResetGameplayHUD;
    }

    private IEnumerator GameTimer()
    {
        yield return new WaitUntil(() => GameManager.Instance.IsGameRunning() != false);//Don't start timer until game is not started

        Debug.Log("Start Timer.");

        float gameTime = GameManager.Instance.GetGameTime();
        while (gameTime > 0 && GameManager.Instance.IsGameRunning())
        {
            gameTime -= Time.deltaTime;
            timeLabel.text = "Time: " + Util.ConvertSecondsToMinutesString(Mathf.RoundToInt(gameTime));
            yield return null;
        }
        if (gameTime <= 0) StartCoroutine(GameManager.Instance.GameOver(false));
        timer = GameManager.Instance.GetGameTime() - Mathf.RoundToInt(gameTime);
        SaveGameState();
    }
    private void UpdateScore(int combo)
    {
        AudioManager.Instance.PlayComboSound(combo);
        score += combo > 1 ? combo * 2 : 1;
        PlayComboAnimation(combo);
        scoreLabel.text = "Score: " + score;
    }
    private void PlayComboAnimation(int combo)
    {
        comboLabel.DOKill();
        if (combo > 1)
        {
            comboLabel.text = $"+{combo * 2}";
            comboLabel.enabled = true;
            comboLabel.transform.DOLocalMoveY(100, 1).SetEase(Ease.Linear);
            comboLabel.DOFade(0, 1f).OnComplete(() =>
            {
                comboLabel.enabled = false;
                comboLabel.transform.DOLocalMoveY(0, 0.01f);
                comboLabel.DOFade(1, 0.01f);
            });
        }
    }
    private void UpdateMoveCount(int move)
    {
        movesLabel.text = "Moves: " + move;
        moves = move;
    }
    private void ShowFailPopup()
    {
        losePopup.SetActive(true);
        AudioManager.Instance.PlaySoundFX(SoundFX.Fail);
    }
    private void ShowWinPopup()
    {
        winPopup.SetActive(true);
        AudioManager.Instance.PlaySoundFX(SoundFX.Win);
    }
    private void ResetGameplayHUD()
    {
        scoreLabel.text = "Score: 0";
        movesLabel.text = "Moves: 0";
        timeLabel.text = "";

        losePopup.SetActive(false);
        winPopup.SetActive(false);
        StartCoroutine(GameTimer());
    }

    private void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
    private void SaveGameState()
    {
        SaveLoadManager.Instance.SaveGame(PersistentKeys.Score, score);
        SaveLoadManager.Instance.SaveGame(PersistentKeys.Timer, timer);
        SaveLoadManager.Instance.SaveGame(PersistentKeys.Moves, moves);

    }
}
