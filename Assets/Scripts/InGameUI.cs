using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private GameObject m_StartSign;
    [SerializeField] private Image m_DimBg;
    [SerializeField] private GameObject m_GameoverSign;
    [SerializeField] private Image m_TimerGauge;
    [SerializeField] private TextMeshProUGUI m_TimerValue;
    [SerializeField] private TextMeshProUGUI m_ScoreValue;
    [SerializeField] private GameObject m_Combo;
    [SerializeField] private TextMeshProUGUI m_ComboValue;

    private int m_Score = 0;

    void Awake()
    {
        m_StartSign.SetActive(false);
        m_DimBg.enabled = false;
        m_GameoverSign.SetActive(false);
        m_ScoreValue.SetText("0");
        m_Combo.SetActive(false);

        GameManager.Instance.GameStarted += OnGameStarted;
        GameManager.Instance.GameOvered += OnGameOvered;
        GameManager.Instance.TimeUpdated += OnTimeUpdated;
        GameManager.Instance.ScoreUpdated += OnScoreUpdated;
        GameManager.Instance.ComboUpdated += OnComboUpdated;
    }

    void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.GameStarted -= OnGameStarted;
            GameManager.Instance.GameOvered -= OnGameOvered;
            GameManager.Instance.TimeUpdated -= OnTimeUpdated;
            GameManager.Instance.ScoreUpdated -= OnScoreUpdated;
            GameManager.Instance.ComboUpdated -= OnComboUpdated;
        }
    }

    private async void OnGameStarted()
    {
        m_ScoreValue.SetText("0");
        m_TimerGauge.fillAmount = 1;
        m_TimerValue.SetText(Config.TIME_LIMIT.ToString());
        await UniTask.Delay(Config.START_DELAY);
        m_StartSign.SetActive(true);
        await UniTask.Delay(1000);
        m_StartSign.SetActive(false);
    }

    private async void OnGameOvered()
    {
        m_GameoverSign.SetActive(true);
        m_DimBg.enabled = true;
        m_DimBg.DOFade(0.8f, 2f).From(0).SetEase(Ease.Linear);
        await UniTask.Delay(Config.GAMEOVER_DURATION);
        m_GameoverSign.SetActive(false);
        m_DimBg.enabled = false;
    }

    private void OnTimeUpdated()
    {
        m_TimerGauge.fillAmount = GameManager.Instance.TimeLeft / Config.TIME_LIMIT;
        m_TimerValue.SetText(GameManager.Instance.TimeLeft.ToString("N0"));
    }

    private void OnScoreUpdated()
    {
        DOTween.To(() => m_Score, x => m_Score = x, GameManager.Instance.Score, 1f)
            .OnUpdate(() => m_ScoreValue.SetText(m_Score.ToString("N0")));
    }

    private void OnComboUpdated()
    {
        if (GameManager.Instance.Combo == 0)
            m_Combo.SetActive(false);
        else
        {
            m_Combo.SetActive(true);
            m_ComboValue.SetText(GameManager.Instance.Combo.ToString());
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)m_ComboValue.transform.parent);
            DOTween.Sequence()
                .Append(m_ComboValue.transform.DOScale(1.5f, 0.15f).SetEase(Ease.OutQuad))
                .Append(m_ComboValue.transform.DOScale(1, 0.15f).SetEase(Ease.InQuad));
        }
    }
}
