//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundlePacker
{
    [MenuItem("自定义工具/测试功能")]
    static void MenuItem_Unknown()
    {
        Debug.Log("====适合直接点的扩展====");
    }

    [MenuItem("Assets/自定义工具/测试功能")]
    static void AssetsItem_Unknown()
    {
        if (Selection.activeObject == null)
        {
            Debug.Log("并未选取任何内容");
            return;
        }
        Debug.Log("====适合Assets相关的操作====: " + Selection.activeObject.name);
        Debug.Log(AssetDatabase.GetAssetPath(Selection.activeObject));
    }

    [MenuItem("GameObject/自定义工具/测试功能", priority = 0)]
    static void HierarchyItem_Pack()
    {
        if (Selection.activeObject == null)
        {
            Debug.Log("并未选取任何内容");
            return;
        }
        Debug.Log("选取的是Hierarchy中的: " + Selection.activeObject.name);
    }
}
