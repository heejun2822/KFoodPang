using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodBlock : Block
{
    private static readonly Array s_FoodTypes = Enum.GetValues(typeof(Config.FoodType));
    private static Config.FoodType RandomFoodType
    {
        get => (Config.FoodType)s_FoodTypes.GetValue(Random.Range(0, s_FoodTypes.Length));
    }

    [SerializeField] private SpriteRenderer[] m_FoodSprites;
    [SerializeField] private SpriteRenderer m_SelectionMark;

    public Config.FoodType FoodType { get; private set; }

    private void SetType(Config.FoodType type)
    {
        FoodType = type;
        for (int i = 0; i < m_FoodSprites.Length; i++)
        {
            if (i == (int)type) m_FoodSprites[i].gameObject.SetActive(true);
            else m_FoodSprites[i].gameObject.SetActive(false);
        }
    }

    public void SetRandomType()
    {
        SetType(RandomFoodType);
    }
}
