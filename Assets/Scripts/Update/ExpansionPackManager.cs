using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExpansionPackManager
{
    private static string LuaCallPackFinishDownload = "OnPackFinishDownload";
    private static string LuaCallPackFinishInstall = "OnPackFinishInstall";

    private static readonly object m_lock = new object();

    private string m_fileRecord;
    private string m_pathDownload;
    private string m_pathDownloadTemp;

    private Dictionary<string, PackState> m_packsRecord = new Dictionary<string, PackState>();

    private static ExpansionPackManager _instance;
    public static ExpansionPackManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ExpansionPackManager();
            return _instance;
        }
    }


    public void Prepare()
    {
        InitRelatedPath();
        LoadPackRecord();
    }

    private void InitRelatedPath()
    {
        string externalPath = GameSetting.ExternalPath();
        m_fileRecord = Path.Combine(externalPath, Path.Combine("expack", "record.xml"));
        m_pathDownload = Path.Combine(externalPath, Path.Combine("expack", "pack"));
        m_pathDownloadTemp = Path.Combine(externalPath, Path.Combine("expack", "temp"));
    }


    public enum PackState
    {
        /// <summary>
        /// 未找到,不存在
        /// </summary>
        Miss = -1,
        /// <summary>
        /// 未完成(下载中...)
        /// </summary>
        UnFinished = 0,
        /// <summary>
        /// 下载完毕
        /// </summary>
        Downloaded = 1,
        /// <summary>
        /// 安装完毕
        /// </summary>
        Installed =2,
    }

    private void LoadPackRecord()
    {
        lock (m_lock)
        {
            m_packsRecord.Clear();

            //从未更新过外部资源
            if (!File.Exists(m_fileRecord)) return;

            //try
            {

            }
        }
    }



}
