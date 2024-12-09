using UnityEngine;
using Random = UnityEngine.Random;

public class AreaBound : MonoBehaviour
{
    [SerializeField] private EdgeCollider2D m_Edge;

    private Bounds m_SpawnBounds;

    void Awake()
    {
        Vector3 center = new(m_Edge.bounds.center.x, (m_Edge.bounds.center.y + m_Edge.bounds.max.y) / 2, 0);
        Vector3 size = new(m_Edge.bounds.size.x, m_Edge.bounds.size.y / 2, 0);
        m_SpawnBounds = new(center, size);
    }

    public Vector3 GetRandomSpawnPosition(float offset)
    {
        return new(
            Random.Range(m_SpawnBounds.min.x + offset, m_SpawnBounds.max.x - offset),
            Random.Range(m_SpawnBounds.min.y + offset, m_SpawnBounds.max.y - offset),
            0
        );
    }
}
