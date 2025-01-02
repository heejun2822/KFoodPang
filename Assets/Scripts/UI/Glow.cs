using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Glow : MonoBehaviour
{
    [SerializeField] private Image m_Glow;

    private Color[] m_ColorSequence = {Color.red, Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta};

    private Color m_Color;
    private float m_Alpha;

    private bool m_IsActive = false;
    private Tween m_ColorTween;
    private Tween m_AlphaTween;

    void Update()
    {
        if (!m_IsActive) return;
        m_Glow.color = new(m_Color.r, m_Color.g, m_Color.b, m_Alpha);
    }

    public void UpdateGlow()
    {
        if (GameManager.Instance.IsFeverTime) Activate();
        else DeActivate().Forget();
    }

    public void ResetGlow()
    {
        m_ColorTween?.Kill();
        m_AlphaTween?.Kill();
        m_IsActive = false;
        gameObject.SetActive(false);

        AudioManager.Instance.StopSubBgm(Config.AudioId.BGM_Fever);
    }

    private void Activate()
    {
        gameObject.SetActive(true);
        m_Color = m_ColorSequence[0];
        TweenColor(1);
        TweenAlpha();
        m_IsActive = true;

        AudioManager.Instance.PlaySubBgm(Config.AudioId.BGM_Fever);
    }

    private async UniTaskVoid DeActivate()
    {
        m_ColorTween.Kill();
        m_AlphaTween.Kill();
        m_IsActive = false;

        await m_Glow.DOFade(0, 0.6f);

        gameObject.SetActive(false);

        AudioManager.Instance.StopSubBgm(Config.AudioId.BGM_Fever);
    }

    private void TweenColor(int idx)
    {
        m_ColorTween = DOTween.To(() => m_Color, x => m_Color = x, m_ColorSequence[idx], 0.85f)
            .SetEase(Ease.Linear)
            .OnComplete(() => TweenColor((idx + 1) % m_ColorSequence.Length));
    }

    private void TweenAlpha()
    {
        m_AlphaTween = DOTween.To(() => m_Alpha, x => m_Alpha = x, 0.3f, 0.35f)
            .From(0.1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => TweenAlpha());
    }
}
