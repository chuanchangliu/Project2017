using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace AssetBundleEx
{
    public class AssetBundleBuilder
    {
        /// <summary>
        /// 打包方法会将所有设置过AssetBundleName的资源打包，所以自动打包前需要清理
        /// </summary>
        public void ClearAssetBundlesName()
        {
            string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < assetBundleNames.Length; i++)
                AssetDatabase.RemoveAssetBundleName(assetBundleNames[i], true);
        }

        public void BuildAssetBundles(AssetBundleSolution solution)
        {
            ClearAssetBundlesName();
            AssetBundleSummary.ClearCache();

            var absExportPath = AssetBundleAssist.AbsolutePath(solution.directoryExport);
            Directory.CreateDirectory(absExportPath);

            foreach(var filter in solution.assetFileFilters)
            {
                if (!filter.isValid) continue;

                var relativeFiles = AssetBundleAssist.FilterAssetFiles(filter.relativePath,filter.regularMatch);
           
                var isScene = filter.regularMatch.Contains(".unity");
                for(var i=0;i<relativeFiles.Length;++i)
                {
                    string relativeFile = relativeFiles[i];
                    EditorUtility.DisplayProgressBar("文件分析", relativeFile, (float)(i+1) / relativeFiles.Length);

                    Debug.Log("====AddAssetBundleChunk: " + relativeFile);
                    AssetBundleSummary.AddAssetBundleChunk(relativeFile,isScene);
                }
                EditorUtility.ClearProgressBar();
            }

            var compressMethod = BuildAssetBundleOptions.None;
            if (solution.compressMethods == CompressMethods.LZ4)
                compressMethod = BuildAssetBundleOptions.ChunkBasedCompression;
            else if (solution.compressMethods == CompressMethods.不压缩)
                compressMethod = BuildAssetBundleOptions.UncompressedAssetBundle;

            var buildTarget = BuildTarget.StandaloneWindows;
            if (solution.targetPlatforms == TargetPlatforms.And)
                buildTarget = BuildTarget.Android;
            else if (solution.targetPlatforms == TargetPlatforms.IOS)
                buildTarget = BuildTarget.iOS;

            AssetBundleSummary.ReportSummary();
            BuildPipeline.BuildAssetBundles(absExportPath, AssetBundleSummary.GetAssetBundleBuild(), compressMethod, buildTarget);
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("温馨提示", "打包工作已经愉快的结束了,如果您还意犹未尽,请等待下次机会!", "确定按钮,没有取消,没有谢谢惠顾,没有再来一次!");
        }
    }

}

