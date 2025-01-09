using DG.Tweening;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ScoreValue;

    private int m_Score = 0;
    private Tween m_TextTween;

    public void UpdateUI(int currentScore)
    {
        m_TextTween?.Kill();

        m_TextTween = DOTween.To(() => m_Score, x => m_Score = x, currentScore, 0.6f)
            .OnUpdate(() => m_ScoreValue.SetText(m_Score.ToString("N0")))
            .OnComplete(() => m_TextTween = null);
    }

    public void ResetUI()
    {
        m_ScoreValue.SetText("0");
        m_Score = 0;
    }
}
