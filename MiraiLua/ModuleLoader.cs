using Mirai.Net.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MiraiLua
{
    class ModuleLoader
    {
        static public List<ModuleInfo> m_hModule = new List<ModuleInfo>();
        static char g = Program.g;

        public struct ModuleInfo {
            public string Name;
            public string Version;
            public string Author;
        }

        static public void LoadModule()
        {
            if (!Directory.Exists($".{g}modules"))
                Directory.CreateDirectory($".{g}modules");
            else
            {
                DirectoryInfo dir = new DirectoryInfo($".{g}modules");
                var fs = dir.GetFiles();

                foreach (FileInfo f in fs)
                {
                    if (f.Extension == ".dll")
                    {
                        try
                        {
                            Util.Print("加载模块..." + f.Name);
                            var pluginAsm = Assembly.LoadFile(f.FullName);
                            Type[] types = pluginAsm.GetTypes();

                            foreach (Type type in types)
                            {
                                if (!type.Name.Equals("MiraiLuaModule"))
                                    continue;

                                object _obj = Activator.CreateInstance(type) ?? throw new Exception("无法创建类型实例");
                                MethodInfo mf = type.GetMethod("Init") ?? throw new Exception("无法获取插件加载方法");

                                // 入口：Load
                                Data d = (Data)mf.Invoke(null, new object[] { });

                                var info = new ModuleInfo()
                                {
                                    Name = d.ReadString(),
                                    Version = d.ReadString(),
                                    Author = d.ReadString()
                                };

                                Util.Print($"\n模块名称：{info.Name}\n版本：{info.Version}\n作者：{info.Author}");
                            }
                        }
                        catch (Exception e)
                        {
                            Util.Print(e.Message + "\n" + e.StackTrace, Util.PrintType.ERROR, ConsoleColor.Red);
                            continue;
                        }
                    }
                }
            }
        }
    }
}
