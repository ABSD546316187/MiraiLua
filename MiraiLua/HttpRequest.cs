using Flurl.Http;
using System;
using System.Collections.Generic;

namespace MiraiLua
{
    /// <summary>
    /// 配套Lua的Http请求类。
    /// </summary>
    class HttpRequest
    {
        /// <summary>
        /// 键值结构体
        /// </summary>
        public struct KV {public string k;public string v; }

        /// <summary>
        /// HTTP GET
        /// </summary>
        /// <param name="u">url</param>
        /// <param name="k1">onSuccessFunctionID</param>
        /// <param name="k2">onFaildFunctionID</param>
        /// <param name="l">Lua Ptr</param>
        /// <param name="head">Head List</param>
        public static async void GetAsync(string u, string k1, string k2, IntPtr l, List<KV> head)
        {
            try
            {
                foreach (KV s in head)
                    u.WithHeader(s.k, s.v);

                var re = await u
                .GetStringAsync();
                lock (Program.o)
                {
                    KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(l);
                    lua.GetGlobal(k1);
                    lua.PushString(re);
                    lua.PCall(1, 0, 0);
                    if (lua.GetTop() >= 1)
                        Util.Print(lua.ToString(-1), Util.PrintType.ERROR, ConsoleColor.Red);   
                    lua.Pop(lua.GetTop());
                    //Util.Print(k);
                    lua.PushNil();
                    lua.SetGlobal(k1);
                }
            }
            catch (Exception e)
            {
                lock (Program.o)
                {
                    KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(l);
                    lua.GetGlobal(k2);
                    lua.PushString(e.Message);
                    lua.PCall(1, 0, 0);
                    lua.Pop(lua.GetTop());
                    //Util.Print(k);
                    lua.PushNil();
                    lua.SetGlobal(k2);
                }
                //Util.Print("HttpGet失败：" + e.Message, Util.PrintType.ERROR, ConsoleColor.Red);
            }
        }
        /// <summary>
        /// HTTP POST
        /// </summary>
        /// <param name="u">url</param>
        /// <param name="k1">onSuccessFunctionID</param>
        /// <param name="k2">onFaildFunctionID</param>
        /// <param name="l">Lua Ptr</param>
        /// <param name="param">Params</param>
        /// <param name="head">Head List</param>
        public static async void PostAsync(string u, string k1, string k2, IntPtr l, List<KV> param, List<KV> head)
        {
            try
            {
                foreach (KV s in head)
                    u.WithHeader(s.k, s.v);

                var ps = new Dictionary<string, string>();

                foreach (KV s in param)
                    ps.Add(s.k,s.v);

                var re = await u.PostJsonAsync(ps).ReceiveString();

                lock (Program.o)
                {
                    KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(l);
                    lua.GetGlobal(k1);
                    lua.PushString(re);
                    lua.PCall(1, 0, 0);
                    if (lua.GetTop() >= 1)
                        Util.Print(lua.ToString(-1), Util.PrintType.ERROR, ConsoleColor.Red);
                    lua.Pop(lua.GetTop());
                    lua.PushNil();
                    lua.SetGlobal(k1);
                }
            }
            catch (Exception e)
            {
                lock (Program.o)
                {
                    KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(l);
                    lua.GetGlobal(k2);
                    lua.PushString(e.Message);
                    lua.PCall(1, 0, 0);
                    lua.Pop(lua.GetTop());
                    //Util.Print(k);
                    lua.PushNil();
                    lua.SetGlobal(k2);
                }
                //Util.Print("HttpGet失败：" + e.Message, Util.PrintType.ERROR, ConsoleColor.Red);
            }
        }
    }
}
