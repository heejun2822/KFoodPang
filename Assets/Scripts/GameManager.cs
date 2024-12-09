using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action GameStarted;
    public event Action GameOvered;

    public void StartGame()
    {
        GameStarted?.Invoke();
    }

    public void Gameover()
    {
        GameOvered?.Invoke();
    }
}
