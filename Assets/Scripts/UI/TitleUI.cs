using UnityEngine;

public class TitleUI : MonoBehaviour
{
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

    private void OnGameExited()
    {
        gameObject.SetActive(true);
    }

    public void OnClickStartBtn()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartGame().Forget();
    }
}
