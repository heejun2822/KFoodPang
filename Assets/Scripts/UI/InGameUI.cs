using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Timer m_TimerComponent;
    [SerializeField] private Score m_ScoreComponent;
    [SerializeField] private Combo m_ComboComponent;

    [SerializeField] private ImageFonts m_ImageFonts;

    void Awake()
    {
        ResetUI();

        GameManager.Instance.GameEntered += OnGameEntered;
        GameManager.Instance.GameStarted += OnGameStarted;
        GameManager.Instance.GameOvered += OnGameOvered;
        GameManager.Instance.GameExited += OnGameExited;
        GameManager.Instance.TimeUpdated += OnTimeUpdated;
        GameManager.Instance.ScoreUpdated += OnScoreUpdated;
        GameManager.Instance.ComboUpdated += OnComboUpdated;
    }

    void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.GameEntered -= OnGameEntered;
            GameManager.Instance.GameStarted -= OnGameStarted;
            GameManager.Instance.GameOvered -= OnGameOvered;
            GameManager.Instance.GameExited -= OnGameExited;
            GameManager.Instance.TimeUpdated -= OnTimeUpdated;
            GameManager.Instance.ScoreUpdated -= OnScoreUpdated;
            GameManager.Instance.ComboUpdated -= OnComboUpdated;
        }
    }

    private void OnGameEntered()
    {
        ResetUI();
    }

    private void OnGameStarted()
    {
        m_ImageFonts.PopUpStartSign().Forget();
    }

    private void OnGameOvered()
    {
        m_ImageFonts.ShowGameoverSign();
    }

    private void OnGameExited()
    {
        m_ImageFonts.HideGameoverSign();
    }

    private void OnTimeUpdated()
    {
        m_TimerComponent.UpdateUI();
    }

    private void OnScoreUpdated()
    {
        m_ScoreComponent.UpdateUI();
    }

    private void OnComboUpdated()
    {
        m_ComboComponent.UpdateUI();
    }

    private void ResetUI()
    {
        m_TimerComponent.ResetUI();
        m_ScoreComponent.ResetUI();
        m_ComboComponent.ResetUI();
        m_ImageFonts.ResetUI();
    }
}
