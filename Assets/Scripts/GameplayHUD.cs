using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameplayHUD : MonoBehaviour
{

    [SerializeField] private TMP_Text timeLabel;
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private TMP_Text movesLabel;

    [SerializeField] private TMP_Text comboLabel;

    private int timer, score, moves;
    private void OnEnable()
    {
        GameManager.UpdateScore += UpdateScore;
        GameManager.UpdateMoves += UpdateMoveCount;
    }
    private void OnDisable()
    {
        GameManager.UpdateScore -= UpdateScore;
        GameManager.UpdateMoves -= UpdateMoveCount;
    }
    // Start is called before the first frame update
    void Start()
    {
        scoreLabel.text = "Score: " + 0;
        movesLabel.text = "Moves: " + 0;
        timeLabel.text = "Time: 00:00";

        StartCoroutine(GameTimer());
    }

    private IEnumerator GameTimer()
    {
        yield return new WaitUntil(() => GameManager.Instance.IsGameRunning() != false);//Don't start timer until game is not started

        Debug.Log("Start Timer.");
        float gameTime = 0f;
        while (GameManager.Instance.IsGameRunning())
        {
            gameTime += Time.deltaTime;
            timeLabel.text = "Time: " + Mathf.RoundToInt(gameTime);
            yield return null;
        }
        timer = Mathf.RoundToInt(gameTime);
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
    private void SaveGameState()
    {
        SaveLoadManager.Instance.SaveGame(PersistentKeys.Score, score);
        SaveLoadManager.Instance.SaveGame(PersistentKeys.Timer, timer);
        SaveLoadManager.Instance.SaveGame(PersistentKeys.Moves, moves);

    }
}
