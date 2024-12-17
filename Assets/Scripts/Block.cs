using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Block : MonoBehaviour
{
    public static bool Interactable { get; set; } = false;

    public float Radius { get; private set; }

    private Rigidbody2D m_Rigidbody;

    protected virtual void Awake()
    {
        Radius = GetComponent<CircleCollider2D>().radius;
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetDynamic(bool dynamic)
    {
        m_Rigidbody.bodyType = dynamic ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
    }

    public void ApplyImpulse(Vector2 impulse)
    {
        m_Rigidbody.AddForce(impulse, ForceMode2D.Impulse);
    }
}

public class Block<T> : Block where T : Enum
{
    private static readonly Array Types = Enum.GetValues(typeof(T));

    [SerializeField] protected Transform m_SpritesContainer;

    public T OwnType { get; private set; }

    public virtual void SetType(T type)
    {
        OwnType = type;
        int idx = Convert.ToInt32(type);
        for (int i = 0; i < m_SpritesContainer.childCount; i++)
        {
            if (i == idx) m_SpritesContainer.GetChild(i).gameObject.SetActive(true);
            else m_SpritesContainer.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetRandomType()
    {
        SetType((T)Types.GetValue(Random.Range(0, Types.Length)));
    }
}
