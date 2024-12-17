using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action GameEntered;
    public event Action GameStarted;
    public event Action GameOvered;
    public event Action GameExited;
    public event Action TimeUpdated;
    public event Action ScoreUpdated;
    public event Action ComboUpdated;

    public float TimeLeft { get; private set; }
    public int Score { get; private set; }
    public int Combo { get; private set; }

    private readonly TimeoutController m_ComboTC = new();

    public async UniTaskVoid StartGame()
    {
        ResetGame();
        GameEntered?.Invoke();
        await UniTask.Delay(1000);

        BlockManager.Instance.Initialize();
        GameStarted?.Invoke();
        StartTimer();
    }

    public async UniTaskVoid Gameover()
    {
        BlockManager.Instance.Terminate().Forget();
        GameOvered?.Invoke();
        await UniTask.Delay(3000);

        BlockManager.Instance.Clear();
        GameExited?.Invoke();
    }

    private void StartTimer()
    {
        TimeLeft = Config.TIME_LIMIT;
        DOTween.To(() => TimeLeft, x => TimeLeft = x, 0, Config.TIME_LIMIT)
            .SetEase(Ease.Linear)
            .OnUpdate(() => TimeUpdated?.Invoke())
            .OnComplete(() => Gameover().Forget());
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

    private void ResetGame()
    {
        TimeLeft = Config.TIME_LIMIT;
        Score = 0;
        Combo = 0;
    }
}
