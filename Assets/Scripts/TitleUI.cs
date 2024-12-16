using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.GameOvered += OnGameOvered;
    }

    void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.GameOvered -= OnGameOvered;
        }
    }

    private async void OnGameOvered()
    {
        await UniTask.Delay(Config.GAMEOVER_DURATION);
        gameObject.SetActive(true);
    }

    public void OnClickStartBtn()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }
}
