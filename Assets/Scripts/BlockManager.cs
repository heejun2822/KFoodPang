using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class BlockManager : Singleton<BlockManager>
{
    [SerializeField] private FoodBlock m_FoodBlockPrefab;
    [SerializeField] private ItemBlock m_ItemBlockPrefab;
    [SerializeField] private AreaBound m_AreaBound;

    private ObjectPool<FoodBlock> m_FoodBlockPool;
    private ObjectPool<ItemBlock> m_ItemBlockPool;

    private HashSet<Block> m_ActiveBlocks = new();

    private int m_InProgressTaskCnt;

    private Vector3 m_RadialDir;
    private Vector3 m_TangentialDir;

    public Vector3 LastBlockPosition { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        m_FoodBlockPool = CreateBlockPool(m_FoodBlockPrefab, Config.BLOCK_CAPACITY, Config.BLOCK_CAPACITY);
        m_ItemBlockPool = CreateBlockPool(m_ItemBlockPrefab, 5, 10);
    }

    private ObjectPool<T> CreateBlockPool<T>(T prefab, int defaultCapacity, int maxSize) where T : Block
    {
        Func<T> createFunc = () => {
            return Instantiate(prefab, transform);
        };
        Action<T> actionOnGet = (block) => {
            block.gameObject.SetActive(true);
        };
        Action<T> actionOnRelease = (block) => {
            block.gameObject.SetActive(false);
        };
        Action<T> actionOnDestroy = (block) => {
            Destroy(block.gameObject);
        };
        return new(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, false, defaultCapacity, maxSize);
    }

    public void ForEachBlocks(Action<Block> action)
    {
        foreach (Block block in m_ActiveBlocks) action(block);
    }

    public void Initialize()
    {
        for (int _ = 0; _ < Config.BLOCK_CAPACITY; _++) AddFoodBlock();
        ForEachBlocks(block => block.SetDynamic(true));
        Block.Interactable = true;
    }

    public void Terminate()
    {
        Block.Interactable = false;
        FoodBlock.TryPop();
        ItemBlock.TryPop();
    }

    public void Clear()
    {
        foreach (Block block in m_ActiveBlocks)
        {
            if (block is FoodBlock foodBlock) m_FoodBlockPool.Release(foodBlock);
            else if (block is ItemBlock itemBlock) m_ItemBlockPool.Release(itemBlock);
        }
        m_ActiveBlocks.Clear();
    }

    private void AddFoodBlock()
    {
        FoodBlock block = m_FoodBlockPool.Get();
        block.SetRandomType();
        block.transform.position = m_AreaBound.GetRandomSpawnPosition(block.Radius);
        m_ActiveBlocks.Add(block);
    }

    private void RemoveFoodBlock(FoodBlock block, bool withSound)
    {
        block.ResetState();
        m_FoodBlockPool.Release(block);
        m_ActiveBlocks.Remove(block);

        EffectManager.Instance.PlayPopEffect(block.transform.position);

        if (!withSound) return;
        AudioManager.Instance.PlaySfx(Config.AudioId.SFX_BlockPoped);
    }

    private void AddItemBlock(Config.ItemType type)
    {
        ItemBlock block = m_ItemBlockPool.Get();
        block.SetType(type);
        if (type == Config.ItemType.Boom)
            block.transform.position = LastBlockPosition;
        else if (type == Config.ItemType.Lightning)
            block.transform.position = m_AreaBound.GetRandomSpawnPosition(block.Radius);
        m_ActiveBlocks.Add(block);
    }

    private void RemoveItemBlock(ItemBlock block)
    {
        block.ResetState();
        m_ItemBlockPool.Release(block);
        m_ActiveBlocks.Remove(block);

        if (block.OwnType == Config.ItemType.Boom)
        {
            EffectManager.Instance.PlayBoomEffect(block.transform.position);
            AudioManager.Instance.PlaySfx(Config.AudioId.SFX_Boom);
        }
        else if (block.OwnType == Config.ItemType.Lightning)
        {
            EffectManager.Instance.PlayLightningEffect(block.transform.position);
            AudioManager.Instance.PlaySfx(Config.AudioId.SFX_Lightning);
        }
    }

    public async UniTaskVoid PopFoodBlocks(FoodBlock[] blocks)
    {
        ++m_InProgressTaskCnt;
        ForEachBlocks(block => block.SetDynamic(false));

        GameManager.Instance.IsComboTimerPaused = true;
        foreach (FoodBlock block in blocks)
        {
            RemoveFoodBlock(block, true);
            await UniTask.Delay(80);
        }
        LastBlockPosition = blocks[^1].transform.position;
        GameManager.Instance.UpdateStatus(blocks.Length);
        GameManager.Instance.IsComboTimerPaused = false;

        if (GameManager.Instance.TimeLeft > 0)
        {
            int blockCnt = blocks.Length;
            if (blockCnt >= Config.CNT_TO_GET_BOOM)
            {
                AddItemBlock(Config.ItemType.Boom);
                blockCnt--;
            }
            if (GameManager.Instance.Combo % Config.COMBO_TO_GET_LIGHTNING == 0)
            {
                AddItemBlock(Config.ItemType.Lightning);
                blockCnt--;
            }
            for (int _ = 0; _ < blockCnt; _++) AddFoodBlock();
        }

        if (--m_InProgressTaskCnt == 0)
        {
            ForEachBlocks(block => block.SetDynamic(true));
        }
    }

    public async UniTaskVoid PopItemBlock(ItemBlock itemBlock, FoodBlock[] foodBlocks)
    {
        ++m_InProgressTaskCnt;
        ForEachBlocks(block => block.SetDynamic(false));

        GameManager.Instance.IsComboTimerPaused = true;
        RemoveItemBlock(itemBlock);
        await UniTask.Delay(120);
        foreach (FoodBlock block in foodBlocks) RemoveFoodBlock(block, false);
        LastBlockPosition = itemBlock.transform.position;
        GameManager.Instance.UpdateStatus(foodBlocks.Length);
        GameManager.Instance.IsComboTimerPaused = false;

        if (GameManager.Instance.TimeLeft > 0)
        {
            int blockCnt = foodBlocks.Length + 1;
            if (GameManager.Instance.Combo % Config.COMBO_TO_GET_LIGHTNING == 0)
            {
                AddItemBlock(Config.ItemType.Lightning);
                blockCnt--;
            }
            for (int _ = 0; _ < blockCnt; _++) AddFoodBlock();
        }

        if (--m_InProgressTaskCnt == 0)
        {
            ForEachBlocks(block => block.SetDynamic(true));
        }
    }

    public void ShakeBlocks()
    {
        ForEachBlocks(block => {
            m_RadialDir = block.transform.position - m_AreaBound.Center;
            m_TangentialDir = Vector3.Cross(m_RadialDir, Vector3.forward).normalized;
            block.ApplyImpulse(m_TangentialDir * Config.SHAKING_IMPULSE);
        });
    }
}
