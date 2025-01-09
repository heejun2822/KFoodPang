using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource m_MainBgmPlayer;
    [SerializeField] private AudioSource m_SubBgmPlayer;
    [SerializeField] private AudioSource m_SfxPlayer;

    [SerializeField] private AudioData[] m_AudioDatas;

    private Dictionary<Config.AudioId, AudioData> m_AudioMap;

    private Config.AudioId m_MainBgmId;
    private Config.AudioId m_SubBgmId;

    public bool IsMute { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        m_AudioMap = m_AudioDatas.ToDictionary(data => data.audioId);
    }

    public void PlayMainBgm(Config.AudioId audioId)
    {
        m_MainBgmId = audioId;
        m_MainBgmPlayer.clip = m_AudioMap[audioId].audioClip;
        m_MainBgmPlayer.volume = m_AudioMap[audioId].volume;
        m_MainBgmPlayer.Play();
    }

    public void StopMainBgm(Config.AudioId audioId)
    {
        if (m_MainBgmId != audioId) return;

        m_MainBgmId = Config.AudioId.None;
        m_MainBgmPlayer.Stop();
    }

    public void PlaySubBgm(Config.AudioId audioId)
    {
        m_MainBgmPlayer.DOFade(0, 0.6f).OnComplete(() => m_MainBgmPlayer.Pause());

        m_SubBgmId = audioId;
        m_SubBgmPlayer.clip = m_AudioMap[audioId].audioClip;
        m_SubBgmPlayer.Play();
        m_SubBgmPlayer.DOFade(m_AudioMap[audioId].volume, 0.6f).From(0);
    }

    public void StopSubBgm(Config.AudioId audioId)
    {
        if (m_SubBgmId != audioId) return;

        m_SubBgmId = Config.AudioId.None;
        m_SubBgmPlayer.DOFade(0, 0.6f).OnComplete(() => m_SubBgmPlayer.Stop());

        if (m_MainBgmId == Config.AudioId.None) return;

        m_MainBgmPlayer.UnPause();
        m_MainBgmPlayer.DOFade(m_AudioMap[m_MainBgmId].volume, 0.6f).From(0);
    }

    public void PlaySfx(Config.AudioId audioId)
    {
        m_SfxPlayer.PlayOneShot(m_AudioMap[audioId].audioClip, m_AudioMap[audioId].volume);
    }

    private void SetMute(bool mute)
    {
        m_MainBgmPlayer.mute = mute;
        m_SubBgmPlayer.mute = mute;
        m_SfxPlayer.mute = mute;
        IsMute = mute;
    }

    public void Mute()
    {
        SetMute(true);
    }

    public void Unmute()
    {
        SetMute(false);
    }

    [Serializable] public class AudioData
    {
        public Config.AudioId audioId;
        public AudioClip audioClip;
        public float volume;
    }
}
