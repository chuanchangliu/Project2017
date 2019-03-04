using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace AssetBundleEx
{
    public static class AssetBundleAssist
    {
        static readonly string _assetsDir;
        static readonly string _assetsPath;
        static readonly string _assetsParentPath;
        static readonly string _assetsParentFold;

        static AssetBundleAssist()
        {
            _assetsDir = "Assets";
            _assetsPath = Application.dataPath;
            _assetsParentPath = Directory.GetParent(_assetsPath).FullName.Replace('\\', '/');
            _assetsParentFold = _assetsParentPath + "/";
        }

        public static string assetsDir
        {
            get { return _assetsDir; }
        }

        public static string assetsPath
        {
            get { return _assetsPath; }
        }

        public static TargetPlatforms targetPlatform
        {
            get
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                    return TargetPlatforms.IOS;
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                    return TargetPlatforms.And;

                return TargetPlatforms.PC;

            }
        }

        public static string RelativePath(string absPath)
        {
            absPath = absPath.Replace('\\', '/');
            return absPath.StartsWith(_assetsParentFold, System.StringComparison.CurrentCultureIgnoreCase) ? absPath.Substring(_assetsParentFold.Length) : null;
        }

        public static string AbsolutePath(string relPath)
        {
            relPath = relPath.Replace('\\', '/');
            return _assetsParentFold + relPath;
        }

        public static string[] FilterFiles(string absolutePath, string pattern)
        {
            var filterFiles = new List<string>();
            var directryInfo = new DirectoryInfo(absolutePath);
            if(directryInfo.Exists)
            {
                var matchFiles = directryInfo.GetFiles(pattern, SearchOption.AllDirectories);
                foreach(var absoluteFile in matchFiles)
                    filterFiles.Add(absoluteFile.FullName.Replace('\\', '/'));
            }

            return filterFiles.ToArray();
        }

        public static string[] FilterAssetFiles(string relativePath,string regularMatch)
        {
            var relativeFiles = new List<string>();
            var absolutePath = AbsolutePath(relativePath);
            var absoluteFiles = FilterFiles(absolutePath, regularMatch);
            foreach(var absoluteFile in absoluteFiles)
            {
                
                if(!absoluteFile.EndsWith(".meta",System.StringComparison.CurrentCultureIgnoreCase))
                {
                    var relativeFile = RelativePath(absoluteFile);
                    relativeFiles.Add(relativeFile);
                }
            }

            return relativeFiles.ToArray();
        }

        public static bool IsAutoReferenced(Object unityObj)
        {
            return unityObj as MonoScript || unityObj as LightingDataAsset;
        }
    }
}
