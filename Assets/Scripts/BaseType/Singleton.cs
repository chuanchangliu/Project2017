using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

public abstract class Singleton<T> where T : Singleton<T>
{

    public static T _instance;
    protected Singleton() {}

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                ConstructorInfo[] constructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                ConstructorInfo constructor = Array.Find(constructors, c => c.GetParameters().Length == 0);

                if (constructor == null)
                    throw new Exception("Non-public ctor() not found!");

                // 调用构造方法
                _instance = constructor.Invoke(null) as T;
            }

            return _instance;
        }
    }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}
