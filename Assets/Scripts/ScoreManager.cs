using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    [SerializeField] private TMP_Text _potsLeftText;

    public event Action<int> ScoreChanged;
    public event Action GameWon;
    public event Action GameLost;

    [SerializeField] private int _targetScore = 10;
    [SerializeField] private int _maxScore = 30;

    public int Score { get; private set; }
    public int TargetScore => _targetScore;

    private bool _hasWon;
    public int potsFired = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ResetScore()
    {
        Score = 0;
        _hasWon = false;
        ScoreChanged?.Invoke(Score);
    }

    public void AddPoint(int amount = 1)
    {
        if (_hasWon) return;

        Score += amount;
        ScoreChanged?.Invoke(Score);

        if (Score >= _targetScore)
        {
            _hasWon = true;
            GameWon?.Invoke();
        }
        else if (potsFired >= _maxScore && !_hasWon)
        {
            GameLost?.Invoke();
        }
    }

    public void OnPotGone()
    {
        if (potsFired >= _maxScore && !_hasWon)
        {
            GameLost?.Invoke();
        }
    }
    public void OnPotShot()
    {
        potsFired += 1;
        _potsLeftText.text = $"{_maxScore - potsFired} pots left.";
    }
}