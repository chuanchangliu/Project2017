using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public delegate void CallbackVoid();
    public CallbackVoid onDestroy = null;
    public CallbackVoid onApplicationQuit = null;

    private float _deltaTime = 0;

    public void Init()
    {
        InitManagers();
        StartCoroutine(GameStartUp());
    }

    void InitManagers()
    {
        //TrackManager.Instance.Init();
        //AudioManager.Instance.Init();
        //UIManager.Instance.Init();
        //EntityManager.Instance.Init();
        //UpdateManager.Instance.Init();
        
        ResourceManager.Instance.Init();
        UIWindowManager.Instance.Init();
    }

    IEnumerator GameStartUp()
    {
        foreach (var item in UpdateManager.Instance.UpdateCoroutine())
            yield return item;

        gameObject.AddComponent<LuaEngine>();
    }

    void OnApplicationQuit()
    {
        Debug.Log("||====OnApplication Quit====||");
        if (onApplicationQuit != null)
            onApplicationQuit();
    }

    void Start()
    {
        _deltaTime = Time.deltaTime;
    }

    void Update()
    {
        ResourceManager.Instance.Tick(_deltaTime);
    }


    void OnDestory()
    {
        Debug.Log("||====OnDestroy====||");
        if (onDestroy != null)
            onDestroy();
    }
}
