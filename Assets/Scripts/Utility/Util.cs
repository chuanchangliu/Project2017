using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

namespace LuaFramework
{
    public class Util
    {
        /// <summary>
        /// 执行Lua方法
        /// </summary>
        public static object[] CallLuaMethod(string module, string func, params object[] args)
        {
            if (LuaClient.Instance == null) return null;

            LuaState lua = LuaClient.GetMainState();
            LuaFunction luaFunc = lua.GetFunction(module + "." + func);
            if (luaFunc != null)
                return luaFunc.LazyCall(args);

            return null;
        }
    }

}