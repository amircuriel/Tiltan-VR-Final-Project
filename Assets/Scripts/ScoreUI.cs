using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private GameObject _winPanel;

    private void Start()
    {
        if (_winPanel != null) _winPanel.SetActive(false);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged += OnScoreChanged;
            ScoreManager.Instance.GameWon += OnGameWon;

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
        }
    }

    private void OnScoreChanged(int score)
    {
        if (_scoreText == null) return;

        int target = ScoreManager.Instance != null ? ScoreManager.Instance.TargetScore : 10;
        _scoreText.text = $"Score: {score}/{target}";
    }

    private void OnGameWon()
    {
        if (_winPanel != null) _winPanel.SetActive(true);
    }
}