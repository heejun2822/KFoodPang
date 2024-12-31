using DG.Tweening;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ScoreValue;

    private int m_Score = 0;

    public void UpdateUI()
    {
        DOTween.To(() => m_Score, x => m_Score = x, GameManager.Instance.Score, 1f)
            .OnUpdate(() => m_ScoreValue.SetText(m_Score.ToString("N0")));
    }

    public void ResetUI()
    {
        m_ScoreValue.SetText("0");
        m_Score = 0;
    }
}
