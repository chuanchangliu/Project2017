using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal abstract class ResourceLoader
{
    protected IDictionary<string, UnityEngine.Object> _path2Obj = new Dictionary<string, UnityEngine.Object>();
    public abstract T Load<T>(string path) where T : UnityEngine.Object;
}

internal class RawResourceLoader : ResourceLoader
{
    public override T Load<T>(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("sync load res by null path");
            return null;
        }
        path = path.ToLower();

        UnityEngine.Object obj = null;
        if (_path2Obj.TryGetValue(path, out obj))
            return obj as T;

        T ret = Resources.Load<T>(path);
        if(ret != null)
            _path2Obj.Add(path, ret);

        return ret;
    }
}


internal class AssetBundleLoader : ResourceLoader
{
    private AssetBundleManifest manifest = null;

    public AssetBundleLoader()
    {
        string bundlePath = string.Format("{0}/config/AssetBundles", Application.streamingAssetsPath);
        AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
        manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        //压缩包释放掉
        bundle.Unload(false);
        bundle = null;
    }

    public override T Load<T>(string path)
    {
        string assetName = string.Format("assets/resources/{0}.prefab", path.ToLower());
        string bundleName = string.Format("{0}.unity3d",assetName);
        string[] dependence = manifest.GetAllDependencies(bundleName);
        for(int i=0;i<dependence.Length;++i)
        {
            Debug.Log("====dependence: " + dependence[i]);
            AssetBundle.LoadFromFile(string.Format("{0}/{1}", Application.streamingAssetsPath, dependence[i]));
        }

        var bundle = AssetBundle.LoadFromFile(string.Format("{0}/{1}", Application.streamingAssetsPath, bundleName));
        var ret = bundle.LoadAsset<T>(assetName);
        if (ret != null)
            _path2Obj.Add(path, ret);
        return ret;
    }
}

/*
internal class PackageFileLoader : ResourceLoader
{
    public override T Load<T>(string path)
    {

    }
}
*/


