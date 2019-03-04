using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* UpdateUIManager 管理 Update 过程中的UI。
** 此时尚未进入游戏逻辑，只能额外制作一套 UIManager 的功能
*/
public class UpdateUIManager {

    private static UpdateUIManager s_instance;
    public static UpdateUIManager Instance{
        get{
            if (s_instance == null)            
                s_instance = new UpdateUIManager();
            return s_instance;
        }
    }

    public UpdateUIManager(): this("UpdateUIRoot"){ }

    public UpdateUIManager(String rootName)
    {
        CreateUIRoot(rootName);
    }

    private GameObject m_rootObject;
    private Camera m_uiCamera;

    private void CreateUIRoot(String rootName)
    {
        /*
        m_rootObject = new GameObject(rootName);

        
        
        UIRoot root = m_rootObject.AddComponent<UIRoot>();
        //root.manualHeight = ScreenConfig.ManualHeight;
        Debug.Log("====manualHeight: " + root.manualHeight.ToString());

        m_uiCamera = NGUITools.AddChild<Camera>(m_rootObject);
        {
            m_uiCamera.allowHDR = false;
            m_uiCamera.allowMSAA = false;

            float depth = -1f;

            //int uiLayer = (int)e_Layer_Type.UI;
            //m_uiCamera.gameObject.layer = uiLayer;
            m_uiCamera.depth = depth + 1;
            m_uiCamera.backgroundColor = Color.grey;
            //m_uiCamera.cullingMask = 1 << uiLayer;

            //Use Simple2D Camera
            m_uiCamera.orthographic = true;
            m_uiCamera.orthographicSize = Convert.ToSingle(ScreenConfig.ManualHeight) / Screen.height;
            m_uiCamera.nearClipPlane = -2f;
            m_uiCamera.farClipPlane = 2f;
            m_uiCamera.gameObject.AddComponent<UICamera>();
        }
        */
    }

    private GameObject CreatePanel(String prefabName)
    {
        /*
        string assetPath = "Prefabs/" + prefabName;
        GameObject prefab = Resources.Load(assetPath, typeof(GameObject)) as GameObject;
        if (prefab == null)
        {
            Debug.LogError(string.Format("failed to load ui asset: {0}", assetPath));
            return null;
        }

        GameObject panel = NGUITools.AddChild(m_uiCamera.gameObject, prefab);
        if (panel == null) return null;

        return panel;
        */
        return null;
    }

    public TComponent CreatePanel<TComponent>(String prefabName) where TComponent : Component
    {
        GameObject gameObject = CreatePanel(prefabName);
        if (gameObject != null)
            return gameObject.GetComponent<TComponent>();

        return null;
    }

}
