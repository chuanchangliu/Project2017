using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EasyCall
{
    public static void Log(string str)
    {
        Debug.Log(str);
    }

    public static void LogWarning(string str)
    {
        Debug.LogWarning(str);
    }

    public static void LogError(string str)
    {
        Debug.LogError(str);
    }

    public static GameObject LoadPrefab(string str)
    {
        return ResourceManager.Instance.LoadPrefab(str);
    }

    public static GameObject CreateWindow(string name,string prefab)
    {
        return UIWindowManager.Instance.CreateWindow(name, prefab);
    }

    public static void ShowWindow(GameObject targetObj)
    {
        UIWindowManager.Instance.MoveToShow(targetObj);
    }
}
