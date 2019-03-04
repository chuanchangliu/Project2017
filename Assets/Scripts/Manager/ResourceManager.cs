using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private ResourceLoader _resLoader = null;

    private ResourceManager() {}

    public void Init()
    {
        if (GameSetting.dataSource == DataSource.RawResource)
            _resLoader = new RawResourceLoader();

        else if (GameSetting.dataSource == DataSource.AssetBundle)
            _resLoader = new AssetBundleLoader();
        
    }

    public GameObject LoadPrefab(string filePath)
    {
        GameObject objTmp = _resLoader.Load<GameObject>(filePath);
        GameObject objRet = null;
        if(objTmp != null)
            objRet = GameObject.Instantiate(objTmp);
 
        return objRet;
    }

    public void LoadPrefabAsync(string filePath)
    {
        //_resLoader.LoadAsync<GameObject>(filePath);
    }

    public void LoadBundle(string name)
    {

    }

    public void LoadBundleAsync(string name)
    {

    }

    public void Tick(float deltaTime)
    {
    }
}
