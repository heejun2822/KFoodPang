using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ImageFonts : MonoBehaviour
{
    [SerializeField] private GameObject m_StartSign;
    [SerializeField] private Image m_DimBg;
    [SerializeField] private GameObject m_GameoverSign;

    public async UniTaskVoid PopUpStartSign()
    {
        m_StartSign.SetActive(true);
        await UniTask.Delay(1000);
        m_StartSign.SetActive(false);
    }

    public void ShowGameoverSign()
    {
        m_GameoverSign.SetActive(true);
        m_DimBg.enabled = true;
        m_DimBg.DOFade(0.8f, 2f).From(0).SetEase(Ease.Linear);
    }

    public void HideGameoverSign()
    {
        m_GameoverSign.SetActive(false);
        m_DimBg.enabled = false;
    }

    public void ResetUI()
    {
        m_StartSign.SetActive(false);
        m_DimBg.enabled = false;
        m_GameoverSign.SetActive(false);
    }
}
