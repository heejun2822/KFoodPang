using System;
using System.Threading;
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
    public event Action FeverUpdated;

    public float TimeLeft { get; private set; }
    public int Score { get; private set; }
    public int Combo { get; private set; }
    public float FeverGauge { get; private set; }
    public float FeverTime { get; private set; }
    public bool IsFeverTime { get => FeverTime > 0; }

    private bool m_IsFeverTimeReady;

    private readonly TimeoutController m_ComboTC = new();
    private CancellationTokenSource m_CTS;

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
        m_CTS.Cancel();
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

    public void UpdateStatus(int blockCnt)
    {
        UpdateScore(blockCnt);
        UpdateCombo().Forget();
        UpdateFeverGauge(blockCnt).Forget();
    }

    private void UpdateScore(int blockCnt)
    {
        int blockScore = (int)(Config.BASIC_BLOCK_SCORE * (1 + 0.1f * Combo));
        int score = (int)(blockCnt - 1 + Mathf.Pow(blockCnt - Config.CNT_TO_POP + 1, 2)) * blockScore;
        if (blockCnt >= Config.CNT_TO_GET_BOOM) score += Config.CNT_TO_POP * blockScore;
        if (IsFeverTime) score = (int)(score * Config.FEVER_TIME_SCORE_FACTOR);

        Score += score;
        ScoreUpdated?.Invoke();
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

    private async UniTaskVoid UpdateFeverGauge(int blockCnt)
    {
        if (m_IsFeverTimeReady || IsFeverTime) return;

        float gauge = FeverGauge + blockCnt * Config.FEVER_GAUGE_PER_BLOCK;
        if (gauge >= Config.MAX_FEVER_GAUGE)
        {
            gauge = Config.MAX_FEVER_GAUGE;
            m_IsFeverTimeReady = true;
        }
        float duration = 0.2f + 0.03f * (gauge - FeverGauge);
        bool gameOvered = await DOTween.To(() => FeverGauge, x => FeverGauge = x, gauge, duration)
            .OnUpdate(() => FeverUpdated?.Invoke())
            .WithCancellation(cancellationToken: m_CTS.Token)
            .SuppressCancellationThrow();

        if (gameOvered) return;
        if (!m_IsFeverTimeReady) return;

        m_IsFeverTimeReady = false;
        FeverTime = Config.FEVER_TIME_DURATION;
        gameOvered = await DOTween.To(() => FeverTime, x => FeverTime = x, 0, Config.FEVER_TIME_DURATION)
            .SetEase(Ease.Linear)
            .OnUpdate(() => FeverUpdated?.Invoke())
            .WithCancellation(cancellationToken: m_CTS.Token)
            .SuppressCancellationThrow();

        if (gameOvered) return;
        FeverGauge = 0;
        FeverUpdated?.Invoke();
    }

    private void ResetGame()
    {
        TimeLeft = Config.TIME_LIMIT;
        Score = 0;
        Combo = 0;
        FeverGauge = 0;
        FeverTime = 0;
        m_IsFeverTimeReady = false;

        m_CTS?.Dispose();
        m_CTS = new();
    }
}
