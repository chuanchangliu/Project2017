using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class PackageManager : Singleton<PackageManager>
{
    private PackageManager() {}

    public void PackageDirectory(string absPath)
    {
        //如果是文件
        if (File.Exists(absPath))
        {
            Debug.LogError(absPath + " is not Directory but file, check it please!");
            return;
        }

        if(!Directory.Exists(absPath))
        {
            Debug.LogError("there is not a Directory that named " + absPath);
            return;
        }
    }

}
