using UnityEngine;

public class TitleUI : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.GameStarted += OnGameStarted;
        GameManager.Instance.GameOvered += OnGameOvered;
    }

    void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.GameStarted -= OnGameStarted;
            GameManager.Instance.GameOvered -= OnGameOvered;
        }
    }

    private void OnGameStarted()
    {
        gameObject.SetActive(false);
    }

    private void OnGameOvered()
    {
        gameObject.SetActive(true);
    }

    public void OnClickStartBtn()
    {
        GameManager.Instance.StartGame();
    }
}
