using System;

public class GameManager : Singleton<GameManager>
{
    public event Action GameStarted;
    public event Action GameOvered;

    public void StartGame()
    {
        GameStarted?.Invoke();
        BlockManager.Instance.Initialize();
    }

    public void Gameover()
    {
        GameOvered?.Invoke();
    }
}
