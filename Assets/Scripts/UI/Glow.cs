using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Glow : MonoBehaviour
{
    [SerializeField] private Image m_Glow;

    private Tween m_GlowingTween;

    public void UpdateGlow()
    {
        if (GameManager.Instance.IsFeverTime) Activate();
        else DeActivate().Forget();
    }

    public void ResetGlow()
    {
        m_GlowingTween.Kill();
        gameObject.SetActive(false);

        AudioManager.Instance.StopSubBgm(Config.AudioId.BGM_Fever);
    }

    private void Activate()
    {
        gameObject.SetActive(true);
        m_GlowingTween = m_Glow.DOFade(1, 0.5f)
            .From(0.2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        AudioManager.Instance.PlaySubBgm(Config.AudioId.BGM_Fever);
    }

    private async UniTaskVoid DeActivate()
    {
        m_GlowingTween.Kill();
        await m_Glow.DOFade(0, 0.5f);
        gameObject.SetActive(false);

        AudioManager.Instance.StopSubBgm(Config.AudioId.BGM_Fever);
    }
}
