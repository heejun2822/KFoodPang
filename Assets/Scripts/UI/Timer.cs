using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image m_TimerGauge;
    [SerializeField] private TextMeshProUGUI m_TimerValue;

    void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;
        UpdateUI();
    }

    private void UpdateUI()
    {
        m_TimerGauge.fillAmount = GameManager.Instance.TimeLeft / Config.TIME_LIMIT;
        m_TimerValue.SetText(GameManager.Instance.TimeLeft.ToString("N0"));
    }

    public void ResetUI()
    {
        m_TimerGauge.fillAmount = 1;
        m_TimerValue.SetText(Config.TIME_LIMIT.ToString());
    }
}
