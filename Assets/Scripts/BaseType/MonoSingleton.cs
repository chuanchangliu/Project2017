using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                string typeName = typeof(T).Name;

                if (FindObjectsOfType<T>().Length > 1)
                {
                    Debug.LogError(typeName + " More than 1!");
                    return _instance;
                }

                if (_instance == null)
                {
                    GameObject attachedGO = GameObject.Find(typeName);

                    if (attachedGO == null)
                        attachedGO = new GameObject(typeName);

                    _instance = attachedGO.AddComponent<T>();
                    DontDestroyOnLoad(attachedGO);  //保证实例不会被释放
                    Debug.Log("Create New Singleton " + typeName + " in game !");
                }
                else
                {
                    Debug.Log("Already exist: " + typeName + " in game !");
                }
            }

            return _instance;
        }

    }

    protected virtual void OnDestroy()
    {
        Debug.Log("====MonoSingleton OnDestory");
        _instance = null;
    }
}