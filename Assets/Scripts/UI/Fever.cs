using UnityEngine;
using UnityEngine.UI;

public class Fever : MonoBehaviour
{
    [SerializeField] private Image m_FeverGauge;

    public void UpdateUI()
    {
        if (GameManager.Instance.IsFeverTime)
            m_FeverGauge.fillAmount = GameManager.Instance.FeverTime / Config.FEVER_TIME_DURATION;
        else
            m_FeverGauge.fillAmount = GameManager.Instance.FeverGauge / Config.MAX_FEVER_GAUGE;
    }

    public void ResetUI()
    {
        m_FeverGauge.fillAmount = 0;
    }
}
