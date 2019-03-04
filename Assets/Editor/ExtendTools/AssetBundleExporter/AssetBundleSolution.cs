using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AssetBundleEx
{

    //如果需要显示默认的Inspcetor，则加一句 DrawDefaultInspector() 就行了
    /* 编辑模式下的代码同步时可能会有问题，比如DestroyImmediate(this)删除自己的代码时，会触发引擎底层的一个错误。
     * 可以使用EditorApplication.delayCall来延迟一帧调用。
     */
    [CustomEditor(typeof(AssetBundleSolution), true)]
    //[CanEditMultipleObjects]
    public class AssetBundleSetupInspector : Editor
    {
        string selectFile;
        string exportDirectory = "/AssetBundles";
        bool _showExportProperty = true;
        bool _showPackProperty = true;
        SerializedProperty _directoryFilterProp;
        ReorderableList _directoryFilterList;

        SerializedProperty _compressMethodsProp;
        SerializedProperty _targetPlatformsProp;
        SerializedProperty _directoryExportProp;
        
        

        void OnEnable()
        {
            _directoryFilterProp = serializedObject.FindProperty("assetFileFilters");
            _directoryFilterList = new ReorderableList(serializedObject, _directoryFilterProp, true, true, true, true);
            _directoryFilterList.drawHeaderCallback = DirectoryFilterDrawHeaderCallback;
            _directoryFilterList.drawElementCallback = DirectoryFilterDrawElementCallback;
            _directoryFilterList.onAddCallback = DirectoryFilterOnAddCallback;

            _compressMethodsProp = serializedObject.FindProperty("compressMethods");
            _targetPlatformsProp = serializedObject.FindProperty("targetPlatforms");
            _directoryExportProp = serializedObject.FindProperty("directoryExport");
        }

        public override void OnInspectorGUI()
        {
            var buildAssetBundle = false;
            var compressAssetBundle = false;
            var buildAndCompress = false;

            if (_showExportProperty = EditorGUILayout.Foldout(_showExportProperty, "第一步：AssetBundle导出"))
            {
                _directoryFilterList.DoLayoutList();

                var compressMethodCompare = _compressMethodsProp.intValue;
                _compressMethodsProp.intValue = System.Convert.ToInt32(EditorGUILayout.EnumPopup("压缩算法选择:", (CompressMethods)_compressMethodsProp.intValue));
                if (_compressMethodsProp.intValue != compressMethodCompare)
                    serializedObject.ApplyModifiedProperties();

                var targetPlatformCompare = _targetPlatformsProp.intValue;
                _targetPlatformsProp.intValue = System.Convert.ToInt32(EditorGUILayout.EnumPopup("目标平台选择:", (TargetPlatforms)_targetPlatformsProp.intValue));
                if (_targetPlatformsProp.intValue != targetPlatformCompare)
                {
                    _directoryExportProp.stringValue = ((TargetPlatforms)_targetPlatformsProp.intValue).ToString()+ exportDirectory;
                    serializedObject.ApplyModifiedProperties();
                }
                var textPrefix = "导出路径选择：";
                EditorGUILayout.LabelField(textPrefix + _directoryExportProp.stringValue);
                if(GUILayout.Button("导出目录选择..."))
                {
                    var absPath = EditorUtility.OpenFolderPanel("选择导出的目录","","");
                    if (absPath != string.Empty)
                    {
                        var relPath = AssetBundleAssist.RelativePath(absPath);
                        if (relPath != null)
                        {
                            _directoryExportProp.stringValue = relPath;
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                }

                if (GUILayout.Button("导出AssetBundle"))
                    buildAssetBundle = true;


                if (buildAssetBundle || buildAndCompress)
                {
                    SaveSettings();
                    EditorApplication.delayCall += () => new AssetBundleBuilder().BuildAssetBundles(solution);
                }
            }



            if (_showPackProperty = EditorGUILayout.Foldout(_showPackProperty, "第二步：AssetBundle打包"))
            {
                if (GUILayout.Button("打包AssetBundle"))
                    compressAssetBundle = true;
            }


            if (GUILayout.Button("保存打包设置"))
                EditorApplication.delayCall += SaveSettings;



            if (GUILayout.Button("完整打包流程"))
            {

            }
        }

        AssetBundleSolution solution
        {
            get { return (AssetBundleSolution)target; }
        }

        void SaveSettings()
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void DirectoryFilterDrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "直接导出AssetBundle的文件夹");
        }

        void DirectoryFilterOnAddCallback(ReorderableList list)
        {
            var absPath = EditorUtility.OpenFolderPanel("选择要打包AssetBundle的文件夹", "", "");
            if (absPath != string.Empty)
            {
                if (absPath.Replace('\\', '/').StartsWith(AssetBundleAssist.assetsPath, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    string relativeDir = AssetBundleAssist.RelativePath(absPath);
                    var index = list.serializedProperty.arraySize;
                    list.serializedProperty.arraySize++;
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    element.FindPropertyRelative("isValid").boolValue = true;
                    element.FindPropertyRelative("relativePath").stringValue = relativeDir;
                    element.FindPropertyRelative("regularMatch").stringValue = "*.prefab";

                }
                else
                {
                    Debug.LogErrorFormat("直接导出AssetBundle的文件夹必须在{0}文件夹下", AssetBundleAssist.assetsDir);
                }
            }
        }


        void DirectoryFilterDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _directoryFilterList.serializedProperty.GetArrayElementAtIndex(index);

            Rect newRect = rect;
            newRect.height = EditorGUIUtility.singleLineHeight;

            newRect.x = rect.xMin;
            newRect.width = 15;
            var property = element.FindPropertyRelative("isValid");
            var validBefore = property.boolValue;
            property.boolValue = EditorGUI.Toggle(newRect, property.boolValue);
            if (property.boolValue != validBefore)
                serializedObject.ApplyModifiedProperties();

            newRect.x = newRect.xMax + 5;
            newRect.xMax = rect.xMax - 85;
            EditorGUI.LabelField(newRect, element.FindPropertyRelative("relativePath").stringValue);

            newRect.x = newRect.xMax + 5;
            newRect.width = 80;
            property = element.FindPropertyRelative("regularMatch");
            string matchBefore = property.stringValue;
            string str = EditorGUI.TextField(newRect, property.stringValue);
            property.stringValue = (str.Trim() == string.Empty) ? "*.prefab" : str;
            if (property.stringValue != matchBefore)
                serializedObject.ApplyModifiedProperties();
        }
    }

    [CreateAssetMenu(fileName = "AssetBundleSolution")]
    public class AssetBundleSolution : ScriptableObject
    {
        public List<AssetFileFilter> assetFileFilters;
        public CompressMethods compressMethods;
        public TargetPlatforms targetPlatforms;
        public string directoryExport;
    }

    [System.Serializable]
    public class AssetFileFilter
    {
        public bool isValid;
        public string relativePath;
        public string regularMatch;
    }

    public enum CompressMethods
    {
        LZMA,
        LZ4,
        不压缩
    }

    public enum TargetPlatforms
    {
        And,
        IOS,
        PC
    }
}