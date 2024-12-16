using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ScoreValue;
    [SerializeField] private GameObject m_Combo;
    [SerializeField] private TextMeshProUGUI m_ComboValue;

    private int m_Score = 0;

    void Awake()
    {
        m_ScoreValue.SetText(string.Empty);
        m_Combo.SetActive(false);

        GameManager.Instance.ScoreUpdated += OnScoreUpdated;
        GameManager.Instance.ComboUpdated += OnComboUpdated;
    }

    void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.ScoreUpdated -= OnScoreUpdated;
            GameManager.Instance.ComboUpdated -= OnComboUpdated;
        }
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
