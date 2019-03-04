using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;
using UnityEngine;

public class LogOutput
{

#if UNITY_EDITOR
    string _persistentPath = Application.dataPath + "/../PersistentPath";
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    string _persistentPath = Application.dataPath + "/PersistentPath";
#else
    string _persistentPath = Application.persistentDataPath;
#endif

    private readonly object _lockObj;
    private bool _isRunning = false;
    private Thread _logThread = null;
    private StreamWriter _logWriter = null;

    private Queue<LogData> _writingQueue = null;
    private Queue<LogData> _waitingQueue = null;

    public LogOutput()
    {
        _lockObj = new object();

        GameManager.Instance.onApplicationQuit += Fini;

        _writingQueue = new Queue<LogData>();
        _waitingQueue = new Queue<LogData>();

        System.DateTime now = System.DateTime.Now;
        string logName = string.Format("Track_{0}_{1:D2}_{2:D2}_{3:D2}{4:D2}{5:D2}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        string logPath = string.Format("{0}/Log/{1}.txt", _persistentPath, logName);
        if (File.Exists(logPath)) File.Delete(logPath);

        string logFold = Path.GetDirectoryName(logPath);
        if (!Directory.Exists(logFold))
            Directory.CreateDirectory(logFold);

        _logWriter = new StreamWriter(logPath);
        _logWriter.AutoFlush = true;

        _isRunning = true;
        _logThread = new Thread(new ThreadStart(OutputLog));
        _logThread.Start();
    }

    //输出log信息到文件
    void OutputLog()
    {
        while (_isRunning)
        {
            if (_writingQueue.Count == 0)
            {
                lock (_lockObj)
                {
                    while (_waitingQueue.Count == 0)
                    {
                        Monitor.Wait(_lockObj);
                    }

                    Queue<LogData> tmpQueue = _writingQueue;
                    _writingQueue = _waitingQueue;
                    _waitingQueue = tmpQueue;
                }
            }
            else
            {
                while (this._writingQueue.Count > 0)
                {
                    LogData logData = _writingQueue.Dequeue();
                    _logWriter.WriteLine(logData.log);
                    if (logData.type == LogType.Error)
                    {
                        _logWriter.WriteLine("=============Error Track============");
                        _logWriter.WriteLine(logData.track);
                        _logWriter.WriteLine("=============Error Over!============");
                    }
                    else if(logData.type == LogType.Exception)
                    {
                        _logWriter.WriteLine("=============Exception Track============");
                        _logWriter.WriteLine(logData.track);
                        _logWriter.WriteLine("=============Exception Over!============");
                    }
                }
            }
        }
    }

    public void Log(LogData logData)
    {
        lock (_lockObj)
        {
            _waitingQueue.Enqueue(logData);
            Monitor.Pulse(_lockObj);
        }
    }

    public void Fini()
    {
        Debug.Log("====LogOutput Fini====");
        _isRunning = false;
        _logWriter.Close();
    }

}



public class TrackManager : Singleton<TrackManager>
{
    private LogOutput _logOutput = null;
    private int _mainThreadID = -1;

    private TrackManager()
    {
        _mainThreadID = Thread.CurrentThread.ManagedThreadId;
        _logOutput = new LogOutput();
    }

    public void Init()
    {
        Application.logMessageReceived += LogCallback;
        Application.logMessageReceivedThreaded += LogMultiThreadCallback;
        GameManager.Instance.onDestroy += Fini;
    }

    void Fini()
    {
        Application.logMessageReceived -= LogCallback;
        Application.logMessageReceivedThreaded -= LogMultiThreadCallback;
    }

    /// <summary>
    /// 日志调用回调，主线程和其他线程都会回调这个函数，在其中根据配置输出日志
    /// </summary>
    /// <param name="log">日志</param>
    /// <param name="track">堆栈追踪</param>
    /// <param name="type">日志类型</param>
    void LogCallback(string log, string track, LogType type)
    {
        if (_mainThreadID == Thread.CurrentThread.ManagedThreadId)
            Output(log, track, type);
    }

    void LogMultiThreadCallback(string log, string track, LogType type)
    {
        if (_mainThreadID != Thread.CurrentThread.ManagedThreadId)
            Output(log, track, type);
    }

    void Output(string log, string track, LogType type)
    {
        LogData logData = new LogData(log, track, type);
        _logOutput.Log(logData);
    }
}
