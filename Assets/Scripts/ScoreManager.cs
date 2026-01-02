using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event Action<int> ScoreChanged;
    public event Action GameWon;

    [SerializeField] private int _targetScore = 10;

    public int Score { get; private set; }
    public int TargetScore => _targetScore;

    private bool _hasWon;

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
    }
}