using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AssetBundleEx
{
    public class AssetBundleSummary
    {
        public static List<AssetBundleBuild> _assetBundleBuilds = new List<AssetBundleBuild>();
        public static Dictionary<string,int> _assetBundleRefers = new Dictionary<string,int>();

        public static void ClearCache()
        {
            _assetBundleBuilds.Clear();
            _assetBundleRefers.Clear();
        }

        public static void AddAssetBundleChunk(string relativeFile, bool isScene)
        {
            Object mainObj = AssetDatabase.LoadMainAssetAtPath(relativeFile);
            if (AssetBundleAssist.IsAutoReferenced(mainObj)) return;

            int referenceCount;
            bool isRepeatChunk = false;
            var assetName = relativeFile.ToLower();
            var assetBundleName = assetName.Replace(' ', '_') + ".unity3d";

            if (_assetBundleRefers.TryGetValue(assetName, out referenceCount))
                isRepeatChunk = true;

            referenceCount += 1;
            _assetBundleRefers[assetName] = referenceCount;
            if (isRepeatChunk) return;

            var build = new AssetBundleBuild();
            build.assetBundleName = assetBundleName;
            build.assetNames = new string[] { assetName };
            _assetBundleBuilds.Add(build);

            var relativeFilesDepend = AssetDatabase.GetDependencies(relativeFile);
            foreach(var relativePath in relativeFilesDepend)
            {
                if (relativePath == relativeFile) continue;
                AddAssetBundleChunk(relativePath, isScene);
            }
        }

        public static void ReportSummary()
        {
            bool haveRepeat = false;
            foreach (var item in _assetBundleRefers)
            {
                if (item.Value > 1)
                {
                    haveRepeat = true;
                    Debug.Log("file: " + item.Key + " is reference " + item.Value.ToString() + " times.");
                }
            }

            if (!haveRepeat)
                Debug.Log("there is not file repeat reference!");
        }

        public static AssetBundleBuild[] GetAssetBundleBuild()
        {
            return _assetBundleBuilds.ToArray();
        }
    }
}
