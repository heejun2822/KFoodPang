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

    public async UniTask Initialize()
    {
        await UniTask.Delay(Config.START_DELAY);
        for (int _ = 0; _ < Config.BLOCK_CAPACITY; _++) AddFoodBlock();
        Block.Interactable = true;
    }

    public async UniTaskVoid Terminate()
    {
        Block.Interactable = false;
        await UniTask.Delay(Config.GAMEOVER_DURATION);
        FoodBlock.ResetSelectedBlocks();
        Lightning.Selected?.ResetState();

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

    private void RemoveFoodBlock(FoodBlock block)
    {
        m_FoodBlockPool.Release(block);
        m_ActiveBlocks.Remove(block);
    }

    private void AddItemBlock(Config.ItemType type, Vector3? position = null)
    {
        ItemBlock block = m_ItemBlockPool.Get();
        block.SetType(type);
        block.transform.position = position ?? m_AreaBound.GetRandomSpawnPosition(block.Radius);
        m_ActiveBlocks.Add(block);
    }

    private void RemoveItemBlock(ItemBlock block)
    {
        m_ItemBlockPool.Release(block);
        m_ActiveBlocks.Remove(block);
    }

    public async UniTask PopFoodBlocks(List<FoodBlock> blocks)
    {
        Block.Interactable = false;
        ForEachBlocks(block => block.SetDynamic(false));

        foreach (FoodBlock block in blocks)
        {
            RemoveFoodBlock(block);
            await UniTask.Delay(80);
        }
        GameManager.Instance.UpdateScore(blocks.Count);

        int blockCnt = blocks.Count;
        if (blockCnt >= Config.CNT_TO_GET_BOOM)
        {
            AddItemBlock(Config.ItemType.Boom, blocks[^1].transform.position);
            blockCnt--;
        }
        for (int _ = 0; _ < blockCnt; _++) AddFoodBlock();

        ForEachBlocks(block => block.SetDynamic(true));
        Block.Interactable = true;
    }

    public async UniTask PopItemBlock(ItemBlock itemBlock, List<FoodBlock> foodBlocks)
    {
        Block.Interactable = false;
        ForEachBlocks(block => block.SetDynamic(false));

        RemoveItemBlock(itemBlock);
        await UniTask.Delay(100);
        foreach (FoodBlock block in foodBlocks) RemoveFoodBlock(block);
        GameManager.Instance.UpdateScore(foodBlocks.Count);

        for (int _ = 0; _ < foodBlocks.Count + 1; _++) AddFoodBlock();

        ForEachBlocks(block => block.SetDynamic(true));
        Block.Interactable = true;
    }
}
