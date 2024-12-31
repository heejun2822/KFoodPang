using DG.Tweening;
using UnityEngine;

public class SceneBg : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_Background;

    private Color DEFAULT_COLOR = new(0.8588235f, 0.509804f, 0.03529412f);  // #DB8209
    private Color FEVER_COLOR = new(0.2352941f, 0.1411765f, 0.01176471f);  // #3C2403

    void Awake()
    {
        GameManager.Instance.FeverTimeToggled += OnFeverTimeToggled;
    }

    void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.FeverTimeToggled -= OnFeverTimeToggled;
        }
    }

    private void OnFeverTimeToggled()
    {
        if (GameManager.Instance.IsFeverTime)
            m_Background.DOColor(FEVER_COLOR, 0.5f);
        else
            m_Background.DOColor(DEFAULT_COLOR, 0.5f);
    }
}
