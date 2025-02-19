using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Timer m_TimerComponent;
    [SerializeField] private Score m_ScoreComponent;
    [SerializeField] private Combo m_ComboComponent;
    [SerializeField] private Fever m_FeverComponent;

    [SerializeField] private ImageFonts m_ImageFonts;
    [SerializeField] private Glow m_Glow;
    [SerializeField] private ScoreEffect m_ScoreEffect;

    void Awake()
    {
        ResetUI();

        GameManager.Instance.GameEntered += OnGameEntered;
        GameManager.Instance.GameStarted += OnGameStarted;
        GameManager.Instance.GameOvered += OnGameOvered;
        GameManager.Instance.GameExited += OnGameExited;
        GameManager.Instance.ScoreUpdated += OnScoreUpdated;
        GameManager.Instance.ComboUpdated += OnComboUpdated;
        GameManager.Instance.FeverTimeToggled += OnFeverTimeToggled;
    }

    void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.GameEntered -= OnGameEntered;
            GameManager.Instance.GameStarted -= OnGameStarted;
            GameManager.Instance.GameOvered -= OnGameOvered;
            GameManager.Instance.GameExited -= OnGameExited;
            GameManager.Instance.ScoreUpdated -= OnScoreUpdated;
            GameManager.Instance.ComboUpdated -= OnComboUpdated;
            GameManager.Instance.FeverTimeToggled -= OnFeverTimeToggled;
        }
    }

    private void OnGameEntered()
    {
        ResetUI();
    }

    private void OnGameStarted()
    {
        m_ImageFonts.PopUpStartSign().Forget();

        AudioManager.Instance.PlayMainBgm(Config.AudioId.BGM_InGame);
    }

    private void OnGameOvered()
    {
        m_ImageFonts.ShowGameoverSign();
    }

    private void OnGameExited()
    {
        m_ImageFonts.HideGameoverSign();
        m_Glow.ResetGlow();

        AudioManager.Instance.StopMainBgm(Config.AudioId.BGM_InGame);
    }

    private async void OnScoreUpdated(int score)
    {
        int currentScore = GameManager.Instance.Score;
        await m_ScoreEffect.DisplayScoreText(score);
        m_ScoreComponent.UpdateUI(currentScore);
    }

    private void OnComboUpdated()
    {
        m_ComboComponent.UpdateUI();
    }

    private void OnFeverTimeToggled()
    {
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
        m_ScoreEffect.ResetScoreTexts();
    }
}
