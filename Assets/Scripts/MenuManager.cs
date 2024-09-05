using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UIScreen
{
    Menu, Gameplay
}
public class MenuManager : MonoBehaviour
{
    [SerializeField] private ToggleGroup layoutSelection;
    [SerializeField] private Toggle _2x2, _2x3, _4x4, _4x5, custom;
    [SerializeField] private TMP_InputField rowsInput, colsInput;
    [SerializeField] private TMP_Text selectGridText;
    [SerializeField] private TMP_Text timerLabel, movesLabel, scoreLabel;

    [Header("UI Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gameplayPanel;

    private int rows = 0, cols = 0;
    private int prevGameScore, prevGameMoves, prevGameTimer;

    private void Awake()
    {
        InitializeListeners();
    }

    private void InitializeListeners()
    {
        _2x2.onValueChanged.AddListener((isOn) => { if (isOn) SetGridSize(2, 2); AudioManager.Instance.PlaySoundFX(SoundFX.Click); });
        _2x3.onValueChanged.AddListener((isOn) => { if (isOn) SetGridSize(2, 3); AudioManager.Instance.PlaySoundFX(SoundFX.Click); });
        _4x4.onValueChanged.AddListener((isOn) => { if (isOn) SetGridSize(4, 4); AudioManager.Instance.PlaySoundFX(SoundFX.Click); });
        _4x5.onValueChanged.AddListener((isOn) => { if (isOn) SetGridSize(4, 5); AudioManager.Instance.PlaySoundFX(SoundFX.Click); });
        custom.onValueChanged.AddListener(ActivateInput);
        rowsInput.onValueChanged.AddListener(OnRowValueChanged);
        colsInput.onValueChanged.AddListener(OnColValueChanged);
    }

    private void Start()
    {
        _2x2.onValueChanged.Invoke(true);
    }
    private void OnEnable()
    {
        GetPreviousGameData();
        SetLabels();
    }
    void GetPreviousGameData()
    {
        prevGameScore = SaveLoadManager.Instance.LoadScore(PersistentKeys.Score);
        prevGameTimer = SaveLoadManager.Instance.LoadScore(PersistentKeys.Timer);
        prevGameMoves = SaveLoadManager.Instance.LoadScore(PersistentKeys.Moves);
    }
    void SetLabels()
    {
        timerLabel.text = $"Last Game Timer: {prevGameTimer}";
        scoreLabel.text = $"Last Game Scores: {prevGameScore}";
        movesLabel.text = $"Last Game Moves: {prevGameMoves}";
    }
    private void SetGridSize(int row, int col)
    {
        rows = row;
        cols = col;
        selectGridText.text = $"{rows} x {cols}";
    }

    private void ActivateInput(bool isOn)
    {
        rowsInput.interactable = isOn;
        colsInput.interactable = isOn;
        if (!isOn) ResetInputs();

        AudioManager.Instance.PlaySoundFX(SoundFX.Click);
    }

    private void ResetInputs()
    {
        rowsInput.text = "";
        colsInput.text = "";
    }

    private void OnRowValueChanged(string text)
    {
        if (int.TryParse(text, out int row))
        {
            row = Mathf.Clamp(row % 2 == 0 ? row : row + 1, 0, 10);
            rowsInput.text = row.ToString();
            SetGridSize(row, cols);
            AudioManager.Instance.PlaySoundFX(SoundFX.Write);
        }
    }

    private void OnColValueChanged(string text)
    {
        if (int.TryParse(text, out int col))
        {
            col = Mathf.Clamp(col % 2 == 0 ? col : col + 1, 0, 10);
            colsInput.text = col.ToString();
            SetGridSize(rows, col);
            AudioManager.Instance.PlaySoundFX(SoundFX.Write);
        }
    }

    public Vector2 GetGridSize() => new Vector2(rows, cols);


    public void SwitchScreens(UIScreen screen)
    {
        switch (screen)
        {
            case UIScreen.Menu:
                menuPanel.SetActive(true);
                gameplayPanel.SetActive(false);
                break;
            case UIScreen.Gameplay:
                menuPanel.SetActive(false);
                gameplayPanel.SetActive(true);
                break;
        }
    }
    public void StartGame()
    {
        SwitchScreens(UIScreen.Gameplay);
        GameManager.Instance.StartGame();
        AudioManager.Instance.PlaySoundFX(SoundFX.Click);

    }
}
