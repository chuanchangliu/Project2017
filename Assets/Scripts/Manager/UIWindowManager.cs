using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIWindowManager : Singleton<UIWindowManager>
{
    private Transform _nodeRoot;
    private Transform _nodeNormal;
    private Transform _nodeHide;
    private Transform _nodePopup;

    private UIWindowManager() {}

    public void Init()
    {
        GameObject obj = ResourceManager.Instance.LoadPrefab("UI/Canvas");
        if (obj == null) return;

        _nodeRoot = obj.transform;
        _nodeHide = _nodeRoot.Find("Hide");
        _nodeNormal = _nodeRoot.Find("Normal");
        _nodePopup = _nodeRoot.Find("Popup");
    }

    public GameObject CreateWindow(string name ,string prefab)
    {
        GameObject obj = ResourceManager.Instance.LoadPrefab(prefab);
        obj.name = name;
        obj.AddComponent<LuaFramework.LuaBehaviour>();
        return obj;
    }

    private void MoveTo(GameObject obj,Transform trans)
    {
        if (obj.transform.parent != trans)
        {
            Vector3 scale = obj.transform.localScale;
            obj.transform.parent = trans;
            obj.transform.localScale = scale;
        }
    }

    public void MoveToShow(GameObject obj)
    {
        MoveTo(obj, _nodeNormal);
    }

    public void MoveToHide(GameObject obj)
    {
        MoveTo(obj, _nodeHide);
    }

    public void MoveToPopup(GameObject obj)
    {
        MoveTo(obj, _nodePopup);
    }
}
