using KeraLua;
using Manganese.Array;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MiraiLua
{
    class ModuleFunctions
    {
        static List<IntPtr> a = new List<IntPtr>();
        static Lua lua = Program.lua;

        delegate int Delegate_Lua(IntPtr p);

        delegate void Delegate_Print(string t, int type, int color);
        static void Print(string t, int type, int color)
        {
            Util.Print(t, (Util.PrintType)type, (ConsoleColor)color);
        }

        delegate void Delegate_Lua_PushFunction(IntPtr p, string table, string name);
        static void Lua_PushFunction(IntPtr p,string table, string name)
        {
            Delegate_Lua f1 = (Delegate_Lua)Marshal.GetDelegateForFunctionPointer(p,typeof(Delegate_Lua));
            LuaFunction f2 = new LuaFunction(delegate (IntPtr p)//我技术力太低了，不会写委托转换只能这样（悲）
            {
                return f1(p);
            });

            lua.GetGlobal(table);
            lua.PushCFunction(f2);
            lua.SetField(1, name);
        }

        static ModuleFunctions()
        {
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Print(Print)));//1
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_PushFunction(Lua_PushFunction)));

            CreateFuns_Push();
            CreateFuns_Check();
            CreateFuns_Get();
            CreateFuns_Set();
            CreateFuns_Other();
        }

        delegate void Delegate_Lua_PushBoolean(bool n);
        delegate void Delegate_Lua_PushCopy(int n);
        delegate void Delegate_Lua_PushInteger(long n);
        delegate void Delegate_Lua_PushNil();
        delegate void Delegate_Lua_PushNumber(double n);
        delegate void Delegate_Lua_PushString(string n);
        static void CreateFuns_Push()
        {
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_PushBoolean(lua.PushBoolean)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_PushCopy(lua.PushCopy)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_PushInteger(lua.PushInteger)));//5
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_PushNil(lua.PushNil)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_PushNumber(lua.PushNumber)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_PushString(lua.PushString)));
        }

        delegate void Delegate_Lua_CheckAny(int n);
        delegate long Delegate_Lua_CheckInteger(int n);
        delegate double Delegate_Lua_CheckNumber(int n);
        delegate bool Delegate_Lua_CheckStack(int n);
        delegate string Delegate_Lua_CheckString(int n);
        delegate void Delegate_Lua_CheckType(int n, LuaType t);
        delegate IntPtr Delegate_Lua_CheckUserData(int n, string name);
        static void CreateFuns_Check()
        {
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_CheckAny(lua.CheckAny)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_CheckInteger(lua.CheckInteger)));//10
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_CheckNumber(lua.CheckNumber)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_CheckStack(lua.CheckStack)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_CheckString(lua.CheckString)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_CheckType(lua.CheckType)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_CheckUserData(lua.CheckUserData)));//15
        }

        delegate LuaType Delegate_Lua_GetField(int n,string k);
        delegate LuaType Delegate_Lua_GetGlobal(string n);
        delegate LuaType Delegate_Lua_GetInteger(int n,long i);
        delegate string Delegate_Lua_GetLocal(nint ar,int n);
        delegate LuaType Delegate_Lua_GetMetaField(int n,string f);
        delegate bool Delegate_Lua_GetMetaTable(int n);
        delegate int Delegate_Lua_GetStack(int n,nint ar);
        delegate bool Delegate_Lua_GetSubTable(int n,string name);
        delegate LuaType Delegate_Lua_GetTable(int n);
        delegate int Delegate_Lua_GetTop();
        delegate string Delegate_Lua_GetUpValue(int funcindex,int n);
        delegate int Delegate_Lua_GetUserValue(int n);
        static void CreateFuns_Get()
        {
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetField(lua.GetField)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetGlobal(lua.GetGlobal)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetInteger(lua.GetInteger)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetLocal(lua.GetLocal)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetMetaField(lua.GetMetaField)));//20
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetMetaTable(lua.GetMetaTable)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetStack(lua.GetStack)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetSubTable(lua.GetSubTable)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetTable(lua.GetTable)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetTop(lua.GetTop)));//25
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetUpValue(lua.GetUpValue)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_GetUserValue(lua.GetUserValue)));
        }

        delegate void Delegate_Lua_SetField(int n, string k);
        delegate void Delegate_Lua_SetGlobal(string n);
        delegate void Delegate_Lua_SetInteger(int n, long i);
        delegate string Delegate_Lua_SetLocal(nint ar, int n);
        delegate void Delegate_Lua_SetMetaTable(int n);
        delegate void Delegate_Lua_SetTable(int n);
        delegate void Delegate_Lua_SetTop(int n);
        delegate string Delegate_Lua_SetUpValue(int funcindex, int n);
        delegate void Delegate_Lua_SetUserValue(int n);
        delegate void Delegate_Lua_SetIndexedUserValue(int index,int n);
        static void CreateFuns_Set()
        {
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_SetField(lua.SetField)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_SetGlobal(lua.SetGlobal)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_SetIndexedUserValue(lua.SetIndexedUserValue)));//30
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_SetInteger(lua.SetInteger)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_SetLocal(lua.SetLocal)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_SetMetaTable(lua.SetMetaTable)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_SetTable(lua.SetTable)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_SetTop(lua.SetTop)));//35
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_SetUpValue(lua.SetUpValue)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_SetUserValue(lua.SetUserValue)));
        }

        delegate void Delegate_Lua_Pop(int n);
        delegate void Delegate_Lua_Call(int args, int results);
        delegate LuaStatus Delegate_Lua_PCall(int args, int results, int errindex);
        delegate bool Delegate_Lua_Next(int index);
        delegate bool Delegate_Lua_ToBoolean(int index);
        delegate long Delegate_Lua_ToInteger(int index);
        delegate double Delegate_Lua_ToNumber(int index);
        delegate IntPtr Delegate_Lua_ToPointer(int index);
        delegate string Delegate_Lua_ToString(int index);
        delegate IntPtr Delegate_Lua_ToUserData(int index);
        static void CreateFuns_Other()
        {
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_Pop(lua.Pop)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_Call(lua.Call)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_PCall(lua.PCall)));//40
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_Next(lua.Next)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_ToBoolean(lua.ToBoolean)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_ToInteger(lua.ToInteger)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_ToNumber(lua.ToNumber)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_ToPointer(lua.ToPointer)));//45
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_ToString(lua.ToString)));
            a.Add(Marshal.GetFunctionPointerForDelegate(new Delegate_Lua_ToUserData(lua.ToUserData)));
        }

        static public byte[] GetFunsData()
        {
            Data data = new Data(new byte[0]);
            data.WriteInt(a.Count);

            foreach (var v in a)
            {
                data.WriteInt((int)v);
            }
            return data.GetData().ToArray();
        }
    }
}
