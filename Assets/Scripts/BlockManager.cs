using System;
using UnityEngine;
using UnityEngine.Pool;

public class BlockManager : Singleton<BlockManager>
{
    [SerializeField] private FoodBlock m_FoodBlockPrefab;
    [SerializeField] private ItemBlock m_ItemBlockPrefab;
    [SerializeField] private AreaBound m_AreaBound;

    private ObjectPool<FoodBlock> m_FoodBlockPool;
    private ObjectPool<ItemBlock> m_ItemBlockPool;

    protected override void Awake()
    {
        base.Awake();
        CreateFoodBlockPool();
        CreateItemBlockPool();
    }

    private void CreateFoodBlockPool()
    {
        Func<FoodBlock> createFunc = () => {
            return Instantiate(m_FoodBlockPrefab, transform);
        };
        Action<FoodBlock> actionOnGet = (block) => {
            block.gameObject.SetActive(true);
            block.SetRandomType();
        };
        Action<FoodBlock> actionOnRelease = (block) => {
            block.gameObject.SetActive(false);
        };
        Action<FoodBlock> actionOnDestroy = (block) => {
            Destroy(block.gameObject);
        };
        m_FoodBlockPool = new(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, false, Config.BLOCK_CAPACITY, Config.BLOCK_CAPACITY);
    }

    private void CreateItemBlockPool()
    {
        Func<ItemBlock> createFunc = () => {
            return Instantiate(m_ItemBlockPrefab, transform);
        };
        Action<ItemBlock> actionOnGet = (block) => {
            block.gameObject.SetActive(true);
        };
        Action<ItemBlock> actionOnRelease = (block) => {
            block.gameObject.SetActive(false);
        };
        Action<ItemBlock> actionOnDestroy = (block) => {
            Destroy(block.gameObject);
        };
        m_ItemBlockPool = new(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, false, 5, 10);
    }

    public void AddFoodBlock()
    {
        FoodBlock block = m_FoodBlockPool.Get();
        block.transform.position = m_AreaBound.GetRandomSpawnPosition(block.Radius);
    }

    public void AddItemBlock()
    {
        ItemBlock block = m_ItemBlockPool.Get();
        block.transform.position = m_AreaBound.GetRandomSpawnPosition(block.Radius);
    }
}
