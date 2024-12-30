using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Glow : MonoBehaviour
{
    [SerializeField] private Image m_Glow;

    private bool m_IsActive;
    private Tween m_GlowingTween;

    public void UpdateGlow()
    {
        if (!m_IsActive && GameManager.Instance.IsFeverTime) Activate();
        else if (m_IsActive && !GameManager.Instance.IsFeverTime) DeActivate().Forget();
    }

    public void ResetGlow()
    {
        m_GlowingTween.Kill();
        m_IsActive = false;
        gameObject.SetActive(false);

        AudioManager.Instance.StopSubBgm(Config.AudioId.BGM_Fever);
    }

    private void Activate()
    {
        m_IsActive = true;
        gameObject.SetActive(true);
        m_GlowingTween = m_Glow.DOFade(1, 0.5f)
            .From(0.2f)
            .SetEase(Ease.OutSine)
            .SetLoops(-1, LoopType.Yoyo);

        AudioManager.Instance.PlaySubBgm(Config.AudioId.BGM_Fever);
    }

    private async UniTaskVoid DeActivate()
    {
        m_GlowingTween.Kill();
        await m_Glow.DOFade(0, 0.5f);
        m_IsActive = false;
        gameObject.SetActive(false);

        AudioManager.Instance.StopSubBgm(Config.AudioId.BGM_Fever);
    }
}
