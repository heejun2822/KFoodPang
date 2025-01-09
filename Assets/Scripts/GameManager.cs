using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action GameEntered;
    public event Action GameStarted;
    public event Action GameOvered;
    public event Action GameExited;
    public event Action<int> ScoreUpdated;
    public event Action ComboUpdated;
    public event Action FeverTimeToggled;

    public bool IsPlaying { get; private set; }
    public float TimeLeft { get; private set; }

    public int Score { get; private set; }
    public int BestScore { get; private set; }

    public int Combo { get; private set; }
    public bool IsComboTimerPaused { get; set; }
    private float m_ComboDuration;

    private float m_TargetFeverGauge;
    public float FeverGauge { get; private set; }
    public float FeverTimeDuration { get; private set; }
    public bool IsFeverTime { get => FeverTimeDuration > 0; }

    [DllImport("__Internal")]
    private static extern void DispatchGameoverEvent(int score, int bestScore, bool isNewBestScore);

    protected override void Awake()
    {
        base.Awake();

        BestScore = PlayerPrefs.GetInt("BestScore", 0);
    }

    void Update()
    {
        if (!IsPlaying) return;

        if (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            if (TimeLeft <= 0) Gameover().Forget();
        }

        if (Combo > 0 && !IsComboTimerPaused)
        {
            m_ComboDuration -= Time.deltaTime;
            if (m_ComboDuration <= 0) ResetCombo();
        }

        if (IsFeverTime)
        {
            FeverTimeDuration -= Time.deltaTime;
            if (FeverTimeDuration <= 0) FinishFeverTime();
        }
        else if (FeverGauge < m_TargetFeverGauge)
        {
            FeverGauge += Time.deltaTime * 20;
            if (FeverGauge >= Config.MAX_FEVER_GAUGE) StartFeverTime();
        }
    }

    public async UniTaskVoid StartGame()
    {
        ResetGame();
        GameEntered?.Invoke();

        await UniTask.Delay(1000);

        BlockManager.Instance.Initialize();
        IsPlaying = true;
        TimeLeft = Config.TIME_LIMIT;
        GameStarted?.Invoke();
    }

    public async UniTaskVoid Gameover()
    {
        TimeLeft = 0;
        GameOvered?.Invoke();

        await BlockManager.Instance.Terminate();
        await UniTask.Delay(3000);

        IsPlaying = false;
        bool isBestScoreUpdated = UpdateBestScore();
        #if UNITY_WEBGL && !UNITY_EDITOR
            DispatchGameoverEvent(Score, BestScore, isBestScoreUpdated);
        #endif
        BlockManager.Instance.Clear();
        GameExited?.Invoke();
    }

    public void UpdateStatus(int blockCnt)
    {
        UpdateScore(blockCnt);
        UpdateCombo();
        UpdateFeverGauge(blockCnt);
    }

    private void UpdateScore(int blockCnt)
    {
        int blockScore = (int)((1 + (Combo > 0 ? 0.1f : 0) + Combo * 0.01f) * Config.BASIC_BLOCK_SCORE);
        int score = (int)(blockCnt - 1 + Mathf.Pow(blockCnt - Config.CNT_TO_POP + 1, 2)) * blockScore;
        if (IsFeverTime) score = (int)(score * Config.FEVER_TIME_SCORE_FACTOR);

        Score += score;
        ScoreUpdated?.Invoke(score);
    }

    private void UpdateCombo()
    {
        Combo += 1;
        m_ComboDuration = Config.COMBO_DURATION;
        ComboUpdated?.Invoke();
    }

    private void ResetCombo()
    {
        Combo = 0;
        m_ComboDuration = 0;
        ComboUpdated?.Invoke();
    }

    private void UpdateFeverGauge(int blockCnt)
    {
        if (IsFeverTime) return;
        m_TargetFeverGauge += blockCnt * Config.FEVER_GAUGE_PER_BLOCK;
    }

    private void StartFeverTime()
    {
        FeverTimeDuration = Config.FEVER_TIME_DURATION;
        m_TargetFeverGauge = 0;
        FeverTimeToggled?.Invoke();
    }

    private void FinishFeverTime()
    {
        FeverTimeDuration = 0;
        FeverGauge = 0;
        FeverTimeToggled?.Invoke();
    }

    private bool UpdateBestScore()
    {
        if (Score <= BestScore) return false;

        BestScore = Score;
        PlayerPrefs.SetInt("BestScore", BestScore);
        return true;
    }

    private void ResetGame()
    {
        IsPlaying = false;
        TimeLeft = Config.TIME_LIMIT;
        Score = 0;
        Combo = 0;
        m_ComboDuration = 0;
        m_TargetFeverGauge = 0;
        FeverGauge = 0;
        FeverTimeDuration = 0;
    }
}
