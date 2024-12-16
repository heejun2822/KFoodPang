using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action GameStarted;
    public event Action GameOvered;
    public event Action ScoreUpdated;
    public event Action ComboUpdated;

    public int Score { get; private set; }
    public int Combo { get; private set; }

    private readonly TimeoutController m_ComboTC = new();

    public void StartGame()
    {
        GameStarted?.Invoke();
        BlockManager.Instance.Initialize();
    }

    public void Gameover()
    {
        GameOvered?.Invoke();
    }

    public void UpdateScore(int blockCnt)
    {
        int blockScore = (int)(Config.BASIC_BLOCK_SCORE * (1 + 0.1f * Combo));
        int score = (int)(blockCnt - 1 + Mathf.Pow(blockCnt - Config.CNT_TO_POP + 1, 2)) * blockScore;
        if (blockCnt >= Config.CNT_TO_GET_BOOM) score += Config.CNT_TO_POP * blockScore;

        Score += score;
        ScoreUpdated?.Invoke();
        UpdateCombo().Forget();
    }

    private async UniTaskVoid UpdateCombo()
    {
        Combo += 1;
        ComboUpdated?.Invoke();
        await UniTask.NextFrame();

        m_ComboTC.Reset();
        (bool comboReseted, int result) = await UniTask.WaitUntilValueChanged(
            this,
            x => x.Combo,
            cancellationToken: m_ComboTC.Timeout(Config.COMBO_DURATION)
        ).SuppressCancellationThrow();

        if (comboReseted)
        {
            Combo = 0;
            ComboUpdated?.Invoke();
        }
    }
}
