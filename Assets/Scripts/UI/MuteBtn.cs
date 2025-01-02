using UnityEngine;
using UnityEngine.UI;

public class MuteBtn : MonoBehaviour
{
    [SerializeField] Sprite m_SoundOnSprite;
    [SerializeField] Sprite m_SoundOffSprite;

    private Image m_BtnImg;

    void Awake()
    {
        m_BtnImg = GetComponent<Image>();

        m_BtnImg.sprite = AudioManager.Instance.IsMute ? m_SoundOffSprite : m_SoundOnSprite;
    }

    public void OnClickMuteBtn()
    {
        if (AudioManager.Instance.IsMute)
        {
            AudioManager.Instance.Unmute();
            m_BtnImg.sprite = m_SoundOnSprite;
        }
        else
        {
            AudioManager.Instance.Mute();
            m_BtnImg.sprite = m_SoundOffSprite;
        }
    }
}
