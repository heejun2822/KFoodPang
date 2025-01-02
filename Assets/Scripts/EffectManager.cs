using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class EffectManager : Singleton<EffectManager>
{
    [SerializeField] private ParticleSystem m_PopEffectPrefab;
    [SerializeField] private ParticleSystem m_BoomEffectPrefab;
    [SerializeField] private ParticleSystem m_LightningEffectPrefab;

    private ObjectPool<ParticleSystem> m_PopEffectPool;
    private ObjectPool<ParticleSystem> m_BoomEffectPool;
    private ObjectPool<ParticleSystem> m_LightningEffectPool;

    protected override void Awake()
    {
        base.Awake();
        m_PopEffectPool = CreateEffectPool(m_PopEffectPrefab, 0.2f, 10, 20);
        m_BoomEffectPool = CreateEffectPool(m_BoomEffectPrefab, 0.5f, 1, 4);
        m_LightningEffectPool = CreateEffectPool(m_LightningEffectPrefab, 0.5f, 1, 4);
    }

    private ObjectPool<ParticleSystem> CreateEffectPool(ParticleSystem prefab, float scale, int defaultCapacity, int maxSize)
    {
        Func<ParticleSystem> createFunc = () => {
            ParticleSystem effect = Instantiate(prefab, transform);
            effect.transform.localScale = Vector3.one * scale;
            return effect;
        };
        Action<ParticleSystem> actionOnGet = (effect) => {
            effect.gameObject.SetActive(true);
        };
        Action<ParticleSystem> actionOnRelease = (effect) => {
            effect.gameObject.SetActive(false);
        };
        Action<ParticleSystem> actionOnDestroy = (effect) => {
            Destroy(effect.gameObject);
        };
        return new(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, false, defaultCapacity, maxSize);
    }

    private async UniTaskVoid PlayEffect(ObjectPool<ParticleSystem> pool, Vector3 position)
    {
        ParticleSystem effect = pool.Get();
        effect.transform.position = position;
        effect.Play();

        await UniTask.Delay((int)(effect.main.duration * 1000));

        pool.Release(effect);
    }

    public void PlayPopEffect(Vector3 position)
    {
        PlayEffect(m_PopEffectPool, position).Forget();
    }

    public void PlayBoomEffect(Vector3 position)
    {
        PlayEffect(m_BoomEffectPool, position).Forget();
    }

    public void PlayLightningEffect(Vector3 position)
    {
        PlayEffect(m_LightningEffectPool, position).Forget();
    }
}
