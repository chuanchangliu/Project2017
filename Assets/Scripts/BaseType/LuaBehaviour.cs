using UnityEngine;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using System;

namespace LuaFramework
{
    public class LuaBehaviour : MonoBehaviour
    {
        private List<LuaFunction> luaFunctions = new List<LuaFunction>();

        protected void Awake()
        {
            LuaState lua = LuaClient.GetMainState();
            LuaTable table = lua.GetTable(name);
            if (table == null)
            {
                throw new LuaException(string.Format("Lua table {0} not exists", name));
            }
            table.RawSet<string, GameObject>("gameObject", gameObject);
            table.RawSet<string, Transform>("transform", transform);
            table.RawSet<string, LuaBehaviour>("script", this);
            table.Dispose();

            Util.CallLuaMethod(name, "Awake");
        }

        protected void Start(){
            Util.CallLuaMethod(name, "Start");
        }

        public void AddEventListener(GameObject go, LuaFunction luafunc, string typ)
        {
            if (go == null || luafunc == null) return;
            if (typ.CompareTo("click") == 0)
                UIEventListener.Get(go).onClick = (o) => { luafunc.Call(o); };
            else if (typ.CompareTo("press") == 0)
                UIEventListener.Get(go).onPress = (o, isPress) => { luafunc.Call(o, isPress); };
            else if (typ.CompareTo("drag") == 0)
                UIEventListener.Get(go).onDrag = (o, delta) => { luafunc.Call(o, delta); };
            else if (typ.CompareTo("drop") == 0)
                UIEventListener.Get(go).onDrop = (o, target) => { luafunc.Call(o, target); };

            luaFunctions.Add(luafunc);
        }

        public void ClearClick()
        {
            foreach (var de in luaFunctions) { de.Dispose(); }
            luaFunctions.Clear();
        }

        //-----------------------------------------------------------------
        protected void OnDestroy()
        {
            Util.CallLuaMethod(name, "OnDestroy", gameObject);
            ClearClick();

            if (LuaClient.Instance == null) return;

            LuaTable table = LuaClient.GetMainState().GetTable(name);
            if (table == null) return;

            table.RawSet<string, object>("gameObject", null);
            table.RawSet<string, object>("transform", null);
            table.RawSet<string, object>("script", null);
            table.Dispose();
        }

        protected void OnDisable()
        {
            Util.CallLuaMethod(name, "OnDisable");
        }

        protected void OnEnable()
        {
            Util.CallLuaMethod(name, "OnEnable");
        }
    }
}