using UnityEngine;

public sealed class DontDestroy : MonoBehaviour
{
    static DontDestroy instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}