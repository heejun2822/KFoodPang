using UnityEngine;

public class Block : MonoBehaviour
{
    public float Radius { get; private set; }

    void Awake()
    {
        Radius = GetComponent<CircleCollider2D>().radius;
    }
}
