using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct LogData
{
    public readonly string log;
    public readonly string track;
    public readonly LogType type;

    public LogData(string log, string track, LogType type)
    {
        this.log = log;
        this.track = track;
        this.type = type;
    }
}

public class Common
{
    public static string extend = ".unity3d";
    public static void CreateDirectoryForFile(string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
    }
}
