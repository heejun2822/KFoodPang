using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ComboValue;

    private Tween m_BouncingTween;

    public void UpdateUI()
    {
        if (GameManager.Instance.Combo == 0)
            ResetUI();
        else
        {
            gameObject.SetActive(true);
            m_ComboValue.SetText(GameManager.Instance.Combo.ToString());
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)m_ComboValue.transform.parent);
            BounceText();
        }
    }

    public void ResetUI()
    {
        gameObject.SetActive(false);
        m_ComboValue.SetText(string.Empty);
    }

    private void BounceText()
    {
        m_BouncingTween ??= DOTween.Sequence()
            .Append(m_ComboValue.transform.DOScale(1.5f, 0.15f).SetEase(Ease.OutQuad))
            .Append(m_ComboValue.transform.DOScale(1, 0.15f).SetEase(Ease.InQuad))
            .SetAutoKill(false);
        m_BouncingTween.Restart();
    }
}
