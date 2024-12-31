using TMPro;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ScoreValue;
    [SerializeField] private TextMeshProUGUI m_BestScoreValue;

    void Awake()
    {
        GameManager.Instance.GameExited += OnGameExited;
    }

    void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.GameExited -= OnGameExited;
        }
    }

    void OnEnable()
    {
        m_ScoreValue.SetText(GameManager.Instance.Score.ToString("N0"));
        m_BestScoreValue.SetText(GameManager.Instance.BestScore.ToString("N0"));

        AudioManager.Instance.PlayMainBgm(Config.AudioId.BGM_Title);
    }

    void OnDisable()
    {
        AudioManager.Instance.StopMainBgm(Config.AudioId.BGM_Title);
    }

    private void OnGameExited()
    {
        gameObject.SetActive(true);
    }

    public void OnClickStartBtn()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartGame().Forget();

        AudioManager.Instance.PlaySfx(Config.AudioId.SFX_ButtonClicked);
    }
}
