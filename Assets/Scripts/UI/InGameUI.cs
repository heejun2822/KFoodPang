using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Timer m_TimerComponent;
    [SerializeField] private Score m_ScoreComponent;
    [SerializeField] private Combo m_ComboComponent;
    [SerializeField] private Fever m_FeverComponent;

    [SerializeField] private ImageFonts m_ImageFonts;
    [SerializeField] private Glow m_Glow;

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
        GameManager.Instance.FeverUpdated += OnFeverUpdated;
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
            GameManager.Instance.FeverUpdated -= OnFeverUpdated;
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
        m_Glow.ResetGlow();
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

    private void OnFeverUpdated()
    {
        m_FeverComponent.UpdateUI();
        m_Glow.UpdateGlow();
    }

    public void OnClickShakeBtn()
    {
        BlockManager.Instance.ShakeBlocks();
    }

    private void ResetUI()
    {
        m_TimerComponent.ResetUI();
        m_ScoreComponent.ResetUI();
        m_ComboComponent.ResetUI();
        m_FeverComponent.ResetUI();
        m_ImageFonts.ResetUI();
    }
}
