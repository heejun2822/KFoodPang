using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class ScoreEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ScoreTextPrefab;
    [SerializeField] private CanvasScaler m_CanvasScaler;
    [SerializeField] private RectTransform m_ScoreUITransform;

    private RectTransform m_RectTransform;

    private ObjectPool<TextMeshProUGUI> m_ScoreTextPool;

    private Vector3 m_ScreenPosition;
    private Vector2 m_LocalPoint;

    void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();

        CreatePool();
    }

    private void CreatePool()
    {
        Func<TextMeshProUGUI> createFunc = () => {
            return Instantiate(m_ScoreTextPrefab, transform);
        };
        Action<TextMeshProUGUI> actionOnGet = (scoreText) => {
            scoreText.gameObject.SetActive(true);
            scoreText.rectTransform.localScale = Vector3.one;
        };
        Action<TextMeshProUGUI> actionOnRelease = (scoreText) => {
            scoreText.gameObject.SetActive(false);
        };
        Action<TextMeshProUGUI> actionOnDestroy = (scoreText) => {
            Destroy(scoreText.gameObject);
        };
        m_ScoreTextPool = new(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, false, 1, 5);
    }

    public async UniTask DisplayScoreText(int score)
    {
        TextMeshProUGUI scoreText = m_ScoreTextPool.Get();
        scoreText.SetText(score.ToString("N0"));

        float textWidth = scoreText.rectTransform.rect.width;
        if (score <= 999) textWidth *= 3 / 5f;
        else if (score <= 9999) textWidth *= 4 / 5f;
        float limitX = (m_CanvasScaler.referenceResolution.x - textWidth) / 2;

        m_ScreenPosition = Camera.main.WorldToScreenPoint(BlockManager.Instance.LastBlockPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RectTransform, m_ScreenPosition, null, out m_LocalPoint);
        m_LocalPoint.x = Mathf.Clamp(m_LocalPoint.x, -limitX, limitX);
        scoreText.rectTransform.anchoredPosition = m_LocalPoint;

        await scoreText.rectTransform.DOMoveY(scoreText.rectTransform.rect.height, 0.8f)
            .SetRelative(true)
            .SetEase(Ease.OutCubic);

        await DOTween.Sequence()
            .Append(scoreText.rectTransform.DOMove(m_ScoreUITransform.position, 0.6f))
            .Join(scoreText.rectTransform.DOScale(0.5f, 0.6f));

        m_ScoreTextPool.Release(scoreText);
    }

    public void ResetScoreTexts()
    {
        m_ScoreTextPool.Clear();
    }
}
