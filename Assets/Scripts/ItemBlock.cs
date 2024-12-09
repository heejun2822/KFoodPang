using UnityEngine;

public class ItemBlock : Block
{
    [SerializeField] private SpriteRenderer[] m_ItemSprites;

    public Config.ItemType ItemType { get; private set; }

    public void SetType(Config.ItemType type)
    {
        ItemType = type;
        for (int i = 0; i < m_ItemSprites.Length; i++)
        {
            if (i == (int)type) m_ItemSprites[i].gameObject.SetActive(true);
            else m_ItemSprites[i].gameObject.SetActive(false);
        }
    }
}
