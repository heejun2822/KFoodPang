using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T m_Instance;

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                T[] objects = FindObjectsOfType<T>();
                if (objects.Length > 0)
                {
                    m_Instance = objects[0];
                }
                if (objects.Length > 1)
                {
                    for (int i = 1; i < objects.Length; i++)
                        Destroy(objects[i].gameObject);
                }
                if (m_Instance == null)
                {
                    GameObject newObject = new(typeof(T).Name);
                    m_Instance = newObject.AddComponent<T>();
                }
            }
            return m_Instance;
        }
    }

    public static bool HasInstance
    {
        get { return m_Instance != null; }
    }

    protected virtual void Awake()
    {
        if (m_Instance == this)
            return;
        if (m_Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            m_Instance = (T)this;
        }
    }

    protected virtual void OnDestroy()
    {
        if (m_Instance == this)
        {
            m_Instance = null;
        }
    }
}

public class PersistentSingleton<T> : Singleton<T> where T : Singleton<T>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
