using System;
using System.Reactive.Linq;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Manganese.Text;

using KeraLua;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Runtime.InteropServices;
using MiraiLua.Classes;

namespace MiraiLua
{
    class Program
    {
        static public Util util = new Util();
        static public Lua lua = new Lua();
        static public MiraiBot bot;
        static public object o = new object();
        static public string curFileDir { get; set; }
        static public char g = Path.DirectorySeparatorChar;
        static void Test()
        {
            lua.GetGlobal("test");
            lua.PCall(0,0,0);
            if (lua.GetTop() >= 1)
                Util.Print(lua.ToString(-1), Util.PrintType.ERROR, ConsoleColor.Red);
            lua.Pop(lua.GetTop());
        }

        static void FileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.Name.IndexOf(".lua") == -1)
                return;
            Thread.Sleep(10);
            Util.Print("文件更新..." + e.FullPath);
            curFileDir = e.FullPath.Replace(g + e.Name,"").Replace($".{g}plugins{g}","");
            lock (o)
            {
                if (lua.DoFile(e.FullPath))
                {
                    Util.Print(lua.ToString(-1), Util.PrintType.ERROR, ConsoleColor.Red);
                    lua.Pop(1);
                }
                curFileDir = "";
            }
        }

        static public void LoadPlugins()
        {
            if (!Directory.Exists($".{g}plugins"))
                Directory.CreateDirectory($".{g}plugins");
            else
            {
                DirectoryInfo dir = new DirectoryInfo($".{g}plugins");
                DirectoryInfo[] ds = dir.GetDirectories();

                foreach (DirectoryInfo d in ds)
                {
                    FileSystemWatcher watcher = new FileSystemWatcher();
                    watcher.Changed += new FileSystemEventHandler(FileChanged);
                    watcher.Path = $".{g}plugins{g}" + d.Name;
                    watcher.EnableRaisingEvents = true;

                    curFileDir = d.Name;

                    FileInfo[] fs = d.GetFiles();
                    //lua.DoFile(f.FullName);
                    foreach (FileInfo f in fs)
                    {
                        if (f.Name == "init.lua")
                        {
                            Util.Print("加载插件..." + d.Name + g + f.Name);

                            if (lua.DoFile($".{g}plugins{g}" + d.Name + g + f.Name))
                            {
                                Util.Print(lua.ToString(-1), Util.PrintType.ERROR, ConsoleColor.Red);
                                lua.Pop(1);
                            }
                        }
                    }
                    curFileDir = "";
                }
            }
        }

        static int Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("MiraiLua v1.1 - Powered by ABSD\n");

            Util.Print("正在启动MiraiLua...");

            ////////////////LUA///////////////////
            lua.Encoding = Encoding.UTF8;

            lua.Register("print",LFunctions.Print);
            lua.Register("include", LFunctions.include);
            lua.Register("GetDir", LFunctions.GetDir);
            lua.Register("ByteArray", ByteArray.New);
            lua.Register("LoadFile", ByteArray.LoadFile);
            lua.Register("SaveFile", ByteArray.SaveFile);

            lua.NewTable();
            lua.SetGlobal("api");
            Util.PushFunction("api", "Reload", lua, LFunctions.Reload);
            Util.PushFunction("api", "SendGroupMsg", lua, LFunctions.SendGroupMsg);
            Util.PushFunction("api", "SendGroupMsgEX", lua, LFunctions.SendGroupMsgEX);
            Util.PushFunction("api", "OnReceiveGroup", lua, LFunctions.OnReceiveGroup);
            Util.PushFunction("api", "HttpGet", lua, LFunctions.HttpGetA);
            Util.PushFunction("api", "HttpPost", lua, LFunctions.HttpPostA);
            Util.PushFunction("api", "UploadImg", lua, LFunctions.UploadImg);
            Util.PushFunction("api", "At", lua, LFunctions.At);

            lua.Pop(lua.GetTop());

            //加载脚本
            LoadPlugins();
            //////////////////////////////////////
            try 
            {
                //XmlDocument读取xml文件
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("settings.xml");
                //获取xml根节点
                XmlNode xmlRoot = xmlDoc.DocumentElement;
                //根据节点顺序逐步读取
                //读取第一个name节点
                string a = xmlRoot.SelectSingleNode("Address").InnerText;
                string q = xmlRoot.SelectSingleNode("QQ").InnerText;
                string v = xmlRoot.SelectSingleNode("Key").InnerText;

                bot = new MiraiBot
                {
                    Address = a,
                    QQ = q,
                    VerifyKey = v
                };

                bot.LaunchAsync();

                Util.Print(String.Format("Bot已连接：{0:G} / {1:G}", a, q));
            }
            catch(Exception e)
            {
                Util.Print(String.Format("发生错误：{0:G}\n请检查settings.xml是否存在且合法.", e.Message));
                Console.ReadLine();
                return 0;
            }
            
            //接收消息
            bot.MessageReceived.OfType<GroupMessageReceiver>().Subscribe(x =>
            {
                if (x.Sender.Id == bot.QQ)
                    return;
                
                string msg = x.MessageChain.GetPlainMessage();
                
                lock (o) {
                    lua.GetGlobal("api");
                    lua.GetField(-1, "OnReceiveGroup");

                    lua.NewTable();

                    lua.PushString(x.Sender.Id);
                    lua.SetField(-2, "SenderID");

                    lua.PushString(x.Sender.Name);
                    lua.SetField(-2, "SenderName");

                    lua.PushNumber((int)x.Sender.Permission);
                    lua.SetField(-2, "SenderRank");

                    lua.PushString(x.GroupId);
                    lua.SetField(-2, "GroupID");

                    lua.PushString(x.GroupName);
                    lua.SetField(-2, "GroupName");

                    lua.PushString(x.Type.ToString());
                    lua.SetField(-2, "From");

                    lua.GetGlobal("util");
                    //Util.Print(lua.ToString(-1)+" "+ lua.ToString(-2) + " "+ lua.ToString(-3) + " "+ lua.ToString(-4) + " ");
                    lua.GetField(-1, "JSONToTable");

                    lua.PushString(x.MessageChain.ToJsonString());

                    lua.Call(1, 1);

                    lua.Remove(-2);

                    Util.Print(String.Format("[{0:G}][{1:G}]：{2:G}", x.GroupName, x.Sender.Name, msg));

                    lua.SetField(-2, "Data");

                    lua.PCall(1, 0, 0);
                    lua.Remove(1);

                    //Util.Print(lua.GetTop().ToString());

                    if (lua.GetTop() >= 1)
                        Util.Print(lua.ToString(-1), Util.PrintType.ERROR, ConsoleColor.Red);
                    lua.Pop(lua.GetTop());
                }
            });

            Util.Print("准备就绪");

            while (true)
            {
                string cmd = Console.ReadLine();
                string[] cargs = cmd.Split(" ");
                lock (o)
                {
                    if (cargs.GetLength(0) < 1)
                    {
                        Util.Print("无效的命令. 要获取帮助请输入help.", Util.PrintType.INFO, ConsoleColor.Red);
                        continue;
                    }

                    if (cargs[0] == "exit")
                        break;
                    if (cargs[0] == "test")
                        Test();
                    if (cargs[0] == "reload")
                        LoadPlugins();
                    else if (cargs[0] == "help")
                    {
                        Util.Print("帮助列表：", Util.PrintType.INFO);
                        Util.Print("help - 获取帮助", Util.PrintType.INFO);
                        Util.Print("reload - 重载插件", Util.PrintType.INFO);
                        Util.Print("exit - 退出MiraiLua", Util.PrintType.INFO);
                        Util.Print("lua <代码> - 执行一段lua文本", Util.PrintType.INFO);
                        Util.Print("Powered by ABSD", Util.PrintType.INFO);
                    }
                    else if (cargs[0] == "lua")
                    {
                        if (cargs.GetLength(0) >= 2)
                        {
                            string s = cargs[1];
                            for (int i = 0; i < cargs.GetLength(0); i++)
                            {
                                if (i > 1)
                                    s += " " + cargs[i];
                            }

                            if (lua.DoString(s))
                            {
                                Util.Print(lua.ToString(-1), Util.PrintType.ERROR, ConsoleColor.Red);
                                lua.Pop(1);
                            }
                        }
                        else
                            Util.Print("命令格式: lua <代码>", Util.PrintType.INFO, ConsoleColor.Red);
                    }
                    else
                        Util.Print("无效的命令. 要获取帮助请输入help.", Util.PrintType.INFO, ConsoleColor.Red);
                }
            }
            return 0;
        }
    }
}
