using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private GameObject _winPanel, _losePanel;

    private void Start()
    {
        if (_winPanel != null) _winPanel.SetActive(false);
        if (_losePanel != null) _losePanel.SetActive(false);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged += OnScoreChanged;
            ScoreManager.Instance.GameWon += OnGameWon;
            ScoreManager.Instance.GameLost += OnGameLost;

            // Initialize display
            OnScoreChanged(ScoreManager.Instance.Score);
        }
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged -= OnScoreChanged;
            ScoreManager.Instance.GameWon -= OnGameWon;
            ScoreManager.Instance.GameLost -= OnGameLost;
        }
    }

    private void OnScoreChanged(int score)
    {
        if (_scoreText == null) return;

        int target = ScoreManager.Instance != null ? ScoreManager.Instance.TargetScore : 10;
        _scoreText.text = $"{score}/{target}";
    }

    private void OnGameWon()
    {
        if (_winPanel != null) _winPanel.SetActive(true);
    }
    
    private void OnGameLost()
    {
        if (_winPanel != null) _losePanel.SetActive(true);
    }
}