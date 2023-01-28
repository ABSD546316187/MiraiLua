using KeraLua;
using Manganese.Array;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MiraiLua
{
    class ModuleFunctions
    {
        static List<IntPtr> a = new List<IntPtr>();
        static Lua lua = Program.lua;

        delegate int Delegate_Lua(IntPtr p);
        delegate void Delegate_Print(string t, int type, int color);


        static void AddFunc<T>(T d)
        {
            GCHandle.Alloc(d);//不回收api函数，以便随时调用
            a.Add(Marshal.GetFunctionPointerForDelegate(d));//1
        }

        static void Print(string t, int type, int color)
        {
            Util.Print(t, (Util.PrintType)type, (ConsoleColor)color);
        }

        delegate void Delegate_Lua_PushFunction(IntPtr p, string table, string name);
        static void Lua_PushFunction(IntPtr p,string table, string name)
        {
            
            Delegate_Lua f1 = Marshal.GetDelegateForFunctionPointer<Delegate_Lua>(p);
            LuaFunction f2 = new LuaFunction(f1);

            GCHandle.Alloc(f2);//不回收api函数，以便随时调用

            Util.PushFunction(table, name, lua, f2);
        }

        static ModuleFunctions()
        {
            GC.KeepAlive(a);

            AddFunc<Delegate_Print>(Print);//1
            AddFunc<Delegate_Lua_PushFunction>(Lua_PushFunction);

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
            AddFunc<Delegate_Lua_PushBoolean>(lua.PushBoolean);
            AddFunc<Delegate_Lua_PushCopy>(lua.PushCopy);
            AddFunc<Delegate_Lua_PushInteger>(lua.PushInteger);//5
            AddFunc<Delegate_Lua_PushNil>(lua.PushNil);
            AddFunc<Delegate_Lua_PushNumber>(lua.PushNumber);
            AddFunc<Delegate_Lua_PushString>(lua.PushString);
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
            AddFunc<Delegate_Lua_CheckAny>(lua.CheckAny);
            AddFunc<Delegate_Lua_CheckInteger>(lua.CheckInteger);//10
            AddFunc<Delegate_Lua_CheckNumber>(lua.CheckNumber);
            AddFunc<Delegate_Lua_CheckStack>(lua.CheckStack);
            AddFunc<Delegate_Lua_CheckString>(lua.CheckString);
            AddFunc<Delegate_Lua_CheckType>(lua.CheckType);
            AddFunc<Delegate_Lua_CheckUserData>(lua.CheckUserData);//15
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
            AddFunc<Delegate_Lua_GetField>(lua.GetField);
            AddFunc<Delegate_Lua_GetGlobal>(lua.GetGlobal);
            AddFunc<Delegate_Lua_GetInteger>(lua.GetInteger);
            AddFunc<Delegate_Lua_GetLocal>(lua.GetLocal);
            AddFunc<Delegate_Lua_GetMetaField>(lua.GetMetaField);//20
            AddFunc<Delegate_Lua_GetMetaTable>(lua.GetMetaTable);
            AddFunc<Delegate_Lua_GetStack>(lua.GetStack);
            AddFunc<Delegate_Lua_GetSubTable>(lua.GetSubTable);
            AddFunc<Delegate_Lua_GetTable>(lua.GetTable);
            AddFunc<Delegate_Lua_GetTop>(lua.GetTop);//25
            AddFunc<Delegate_Lua_GetUpValue>(lua.GetUpValue);
            AddFunc<Delegate_Lua_GetUserValue>(lua.GetUserValue);
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
            AddFunc<Delegate_Lua_SetField>(lua.SetField);
            AddFunc<Delegate_Lua_SetGlobal>(lua.SetGlobal);
            AddFunc<Delegate_Lua_SetIndexedUserValue>(lua.SetIndexedUserValue);//30
            AddFunc<Delegate_Lua_SetInteger>(lua.SetInteger);
            AddFunc<Delegate_Lua_SetLocal>(lua.SetLocal);
            AddFunc<Delegate_Lua_SetMetaTable>(lua.SetMetaTable);
            AddFunc<Delegate_Lua_SetTable>(lua.SetTable);
            AddFunc<Delegate_Lua_SetTop>(lua.SetTop);//35
            AddFunc<Delegate_Lua_SetUpValue>(lua.SetUpValue);
            AddFunc<Delegate_Lua_SetUserValue>(lua.SetUserValue);
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
        delegate void Delegate_Lua_NewTable();
        delegate LuaType Delegate_Lua_Type(int index);
        delegate void Delegate_Lua_PushByteArray(int p,int l);
        static void CreateFuns_Other()
        {
            AddFunc<Delegate_Lua_Pop>(lua.Pop);
            AddFunc<Delegate_Lua_Call>(lua.Call);
            AddFunc<Delegate_Lua_PCall>(lua.PCall);//40
            AddFunc<Delegate_Lua_Next>(lua.Next);
            AddFunc<Delegate_Lua_ToBoolean>(lua.ToBoolean);
            AddFunc<Delegate_Lua_ToInteger>(lua.ToInteger);
            AddFunc<Delegate_Lua_ToNumber>(lua.ToNumber);
            AddFunc<Delegate_Lua_ToPointer>(lua.ToPointer);//45
            AddFunc<Delegate_Lua_ToString>(lua.ToString);
            AddFunc<Delegate_Lua_ToUserData>(lua.ToUserData);
            AddFunc<Delegate_Lua_NewTable>(lua.NewTable);
            AddFunc<Delegate_Lua_Type>(lua.Type);
            AddFunc<Delegate_Lua_PushByteArray>(delegate(int p,int l)
            {
                var b = new byte[l];
                Marshal.Copy(p, b, 0, l);
                ByteArray.PushByteArray(b);
            });//50
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
