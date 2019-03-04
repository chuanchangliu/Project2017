using System;
using UnityEngine;

public enum DataSource
{
    RawResource,
    AssetBundle,
    PackageFile
}

[System.Serializable]
public class GameOption
{
    public bool showFPSMeter;
    public bool fullFrameRate;
    public bool skipProgramUpdate;
    public bool skipResouceUpdate;
    public string externalPath;
    public DataSource dataSource;
}

public class GameSetting
{
    private static GameObject s_gameObject;
    private static GameOption s_gameOption;

    public static DataSource dataSource
    {
        get { return s_gameOption.dataSource; }
    }

    public static void RecordSetting(GameOption gameOption,GameObject gameObject)
    {
        s_gameObject = gameObject;
        s_gameOption = gameOption;
        ParseCommandLine();
        ApplyDebugConfig();
    }

    static void ParseCommandLine()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        string[] aArgs = Environment.GetCommandLineArgs();
        if (Array.IndexOf(aArgs, "-skipprogupdate") >= 0)
            s_gameOption.skipProgramUpdate = true;
#endif
    }

    public static string ExternalPath()
    {
        return s_gameOption.externalPath;
    }

    public static void UpdateFrameRate()
    {
        Application.targetFrameRate = s_gameOption.fullFrameRate ? -1 : 30;
    }

    public static void UpdateFpsStatus()
    {
        if(s_gameOption.showFPSMeter)
            s_gameObject.AddComponent<FPSMeter>();
    }

    static void ApplyDebugConfig()
    {
        UpdateFpsStatus();
        UpdateFrameRate();
    }
}
