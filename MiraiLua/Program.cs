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
using System.Threading.Tasks;
using Flurl.Http;

namespace MiraiLua
{
    class Program
    {
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

        static async Task<int> Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("MiraiLua v1.2 - Powered by ABSD\n");

            Util.Print("正在启动MiraiLua...");

            FlurlHttp.ConfigureClient("https://api.openai.com/v1/moderations", cli =>
            cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());

            ////////////////LUA///////////////////
            lua.Encoding = Encoding.UTF8;

            lua.Register("print",LFunctions.Print);
            lua.Register("include", LFunctions.include);
            lua.Register("Unescape", LFunctions.Unescape);
            lua.Register("GetDir", LFunctions.GetDir);
            lua.Register("ByteArray", ByteArray.New);
            lua.Register("LoadFile", ByteArray.LoadFile);
            lua.Register("SaveFile", ByteArray.SaveFile);

            Util.PushFunction("api", "Reload", lua, LFunctions.Reload);
            Util.PushFunction("api", "LocalBot", lua, LFunctions.LocalBot);
            Util.PushFunction("api", "SendGroupMsg", lua, LFunctions.SendGroupMsg);
            Util.PushFunction("api", "SendFriendMsg", lua, LFunctions.SendFriendMsg);
            Util.PushFunction("api", "SendTempMsg", lua, LFunctions.SendTempMsg);
            Util.PushFunction("api", "SendGroupMsgEX", lua, LFunctions.SendGroupMsgEX);
            Util.PushFunction("api", "SendFriendMsgEX", lua, LFunctions.SendFriendMsgEX);
            Util.PushFunction("api", "SendTempMsgEX", lua, LFunctions.SendTempMsgEX);
            Util.PushFunction("api", "OnReceiveGroup", lua, LFunctions.OnReceiveGroup);
            Util.PushFunction("api", "OnReceiveFriend", lua, LFunctions.OnReceiveFriend);
            Util.PushFunction("api", "OnReceiveTemp", lua, LFunctions.OnReceiveTemp);
            Util.PushFunction("api", "HttpGet", lua, LFunctions.HttpGetA);
            Util.PushFunction("api", "HttpPost", lua, LFunctions.HttpPostA);
            Util.PushFunction("api", "UploadImg", lua, LFunctions.UploadImg);
            Util.PushFunction("api", "UploadImgBase64", lua, LFunctions.UploadImgBase64);
            Util.PushFunction("api", "At", lua, LFunctions.At);

            Util.PushFunction("util", "Base64ToString", lua, LFunctions.Base64ToString);
            Util.PushFunction("util", "StringToBase64", lua, LFunctions.StringToBase64);
            //加载模块
            ModuleLoader.LoadModule();

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

                Util.Print(String.Format($"正在连接至：{a} / {q} ... 可能需要几秒钟时间."));

                bot = new MiraiBot
                {
                    Address = a,
                    QQ = q,
                    VerifyKey = v
                };

                await bot.LaunchAsync();
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

                    //Util.Print(String.Format("[{0:G}][{1:G}]：{2:G}", x.GroupName, x.Sender.Name, msg));

                    lua.SetField(-2, "Data");

                    lua.PCall(1, 0, 0);
                    lua.Remove(1);

                    //Util.Print(lua.GetTop().ToString());

                    if (lua.GetTop() >= 1)
                        Util.Print(lua.ToString(-1), Util.PrintType.ERROR, ConsoleColor.Red);
                    lua.Pop(lua.GetTop());
                }
            });

            bot.MessageReceived.OfType<FriendMessageReceiver>().Subscribe(x =>
            {
                string msg = x.MessageChain.GetPlainMessage();

                lock (o)
                {
                    lua.GetGlobal("api");
                    lua.GetField(-1, "OnReceiveFriend");

                    lua.NewTable();

                    lua.PushString(x.FriendRemark);
                    lua.SetField(-2, "Remark");

                    lua.PushString(x.FriendName);
                    lua.SetField(-2, "SenderName");

                    lua.PushString(x.FriendId);
                    lua.SetField(-2, "SenderID");

                    lua.PushString(x.Type.ToString());
                    lua.SetField(-2, "From");

                    lua.GetGlobal("util");
                    //Util.Print(lua.ToString(-1)+" "+ lua.ToString(-2) + " "+ lua.ToString(-3) + " "+ lua.ToString(-4) + " ");
                    lua.GetField(-1, "JSONToTable");

                    lua.PushString(x.MessageChain.ToJsonString());

                    lua.Call(1, 1);

                    lua.Remove(-2);

                    //Util.Print($"[Friend][{x.FriendName}]：{msg}");

                    lua.SetField(-2, "Data");

                    lua.PCall(1, 0, 0);
                    lua.Remove(1);

                    //Util.Print(lua.GetTop().ToString());

                    if (lua.GetTop() >= 1)
                        Util.Print(lua.ToString(-1), Util.PrintType.ERROR, ConsoleColor.Red);
                    lua.Pop(lua.GetTop());
                }
            });

            bot.MessageReceived.OfType<TempMessageReceiver>().Subscribe(x =>
            {
                string msg = x.MessageChain.GetPlainMessage();

                lock (o)
                {
                    lua.GetGlobal("api");
                    lua.GetField(-1, "OnReceiveTemp");

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

                    lua.PushInteger((int)x.BotPermission);
                    lua.SetField(-2, "BotPermission");

                    lua.PushString(x.Type.ToString());
                    lua.SetField(-2, "From");

                    lua.GetGlobal("util");
                    //Util.Print(lua.ToString(-1)+" "+ lua.ToString(-2) + " "+ lua.ToString(-3) + " "+ lua.ToString(-4) + " ");
                    lua.GetField(-1, "JSONToTable");

                    lua.PushString(x.MessageChain.ToJsonString());

                    lua.Call(1, 1);

                    lua.Remove(-2);

                    //Util.Print($"[Temp {x.GroupName}][{x.Sender.Name}]：{msg}");

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
                    else if (cargs[0] == "test")
                    {
                        GC.Collect();
                    }
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
