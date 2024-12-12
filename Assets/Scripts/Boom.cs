using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour, IItem
{
    private ItemBlock m_Block;

    void Awake()
    {
        m_Block = GetComponent<ItemBlock>();
    }

    public void Select()
    {
        return;
    }

    public bool TryConnect(FoodBlock block)
    {
        return false;
    }

    public bool GetTargets(List<FoodBlock> targets)
    {
        targets.Clear();
        BlockManager.Instance.ForEachBlocks(block => {
            FoodBlock foodBlock = block as FoodBlock;
            if (foodBlock == null) return;
            float sqrDistance = (foodBlock.transform.position - transform.position).sqrMagnitude;
            float maxSqrDistance = Mathf.Pow(m_Block.Radius * Config.BOOM_RANGE_FACTOR + foodBlock.Radius, 2);
            if (sqrDistance > maxSqrDistance) return;
            targets.Add(foodBlock);
        });
        return true;
    }

    public void ResetState()
    {
        return;
    }
}
