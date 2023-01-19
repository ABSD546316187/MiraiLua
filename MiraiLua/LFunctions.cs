using System;
using KeraLua;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages;
using System.Collections.Generic;

namespace MiraiLua
{
    class LFunctions
    {
        static public int Print(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);

            string s = "";

            for (int i = 1; i <= lua.GetTop(); i++)
            {
                s += lua.ToString(i) + "    ";
            }
            
            //s = Util.EncodingConvert(Encoding.GetEncoding("unicode"), Encoding.GetEncoding("utf-8"), s);
            Util.Print(s);

            return 0;
        }
        static public int include(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            char g = Program.g;
            if (Program.curFileDir != "")
            {
                string s = lua.CheckString(1);

                s = s.Replace(".lua", "");

                Util.Print("加载插件..." + Program.curFileDir + g + s + ".lua");
                if (lua.DoFile($".{g}plugins{g}" + Program.curFileDir + g + s + ".lua"))
                {
                    Util.Print(lua.ToString(-1), Util.PrintType.ERROR, ConsoleColor.Red);
                    lua.Pop(1);
                }
            }
            else
            {
                lua.Traceback(lua,1);
                lua.PushString("请勿在加载完毕后调用include\n" + lua.ToString(-1));
                lua.Error();
            }
            return 0;
        }

        static public int GetDir(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            char g = Program.g;

            lua.GetGlobal("debug");
            lua.GetField(-1, "getinfo");
            lua.PushNumber(2);
            lua.Call(1, 1);
            lua.GetField(-1, "short_src");
            string s = lua.CheckString(-1);
            s = $"plugins{g}{Util.GetMiddleStr(s, $".{g}plugins{g}", g.ToString())}{g}";
            lua.PushString(s);
            return 1;
        }
        static public int Reload(IntPtr p)
        {
            Program.LoadPlugins();
            return 0;
        }
        
        static public int OnReceiveGroup(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);

            lua.Pop(lua.GetTop());

            //s = Util.EncodingConvert(Encoding.GetEncoding("unicode"), Encoding.GetEncoding("utf-8"), s);
            Util.Print("OnReceiveGroup函数未被定义，请检查是否丢失basic插件.", Util.PrintType.WARNING);

            return 0;
        }
        static public int OnReceiveFriend(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);

            lua.Pop(lua.GetTop());

            //s = Util.EncodingConvert(Encoding.GetEncoding("unicode"), Encoding.GetEncoding("utf-8"), s);
            Util.Print("OnReceiveFriend函数未被定义，请检查是否丢失basic插件.", Util.PrintType.WARNING);

            return 0;
        }
        static public int OnReceiveTemp(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);

            lua.Pop(lua.GetTop());

            //s = Util.EncodingConvert(Encoding.GetEncoding("unicode"), Encoding.GetEncoding("utf-8"), s);
            Util.Print("OnReceiveTemp函数未被定义，请检查是否丢失basic插件.", Util.PrintType.WARNING);

            return 0;
        }

        static public int HttpGetA(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            var hs = new Dictionary<string, string>();
            string u = lua.CheckString(1);

            lua.CheckType(2, LuaType.Function);
            string k1 = lua.ToString(2);

            lua.CheckType(3, LuaType.Function);
            string k2 = lua.ToString(3);

            lua.PushCopy(2);
            lua.SetGlobal(k1);
            lua.PushCopy(3);
            lua.SetGlobal(k2);

            hs.Add("User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:107.0) Gecko/20100101 Firefox/107.0" );

            if (lua.Type(4) == LuaType.Table)
            {
                lua.PushCopy(4);
                lua.PushNil();
                while (lua.Next(-2))
                {
                    lua.PushCopy(-2);
                    if (lua.Type(-1) == LuaType.String && lua.Type(-2) == LuaType.String)
                    {
                        if (hs.TryGetValue(lua.ToString(-1),out _))
                            hs[lua.ToString(-1)] = lua.ToString(-2);
                        else
                            hs.Add(lua.ToString(-1), lua.ToString(-2));
                    }
                    
                    lua.Pop(2);
                }
                lua.Pop(1);
            }
            
            HttpRequest.GetAsync(u, k1, k2, p, hs);

            return 0;
        }
        static public int HttpPostA(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            var hs = new Dictionary<string, string>();
            var pa = new Dictionary<string, string>();
            string u = lua.CheckString(1);

            lua.CheckType(2, LuaType.Function);
            string k1 = lua.ToString(2);

            lua.CheckType(3, LuaType.Function);
            string k2 = lua.ToString(3);

            lua.PushCopy(2);
            lua.SetGlobal(k1);
            lua.PushCopy(3);
            lua.SetGlobal(k2);

            hs.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:107.0) Gecko/20100101 Firefox/107.0");

            if (lua.Type(4) == LuaType.Table)
            {
                lua.PushCopy(4);
                lua.PushNil();
                while (lua.Next(-2))
                {
                    lua.PushCopy(-2);
                    if (lua.Type(-1) == LuaType.String && lua.Type(-2) == LuaType.String)
                        pa.Add(lua.ToString(-1), lua.ToString(-2));
                    lua.Pop(2);
                }
                lua.Pop(1);
            }

            if (lua.Type(5) == LuaType.Table)
            {
                lua.PushCopy(5);
                lua.PushNil();
                while (lua.Next(-2))
                {
                    lua.PushCopy(-2);
                    if (lua.Type(-1) == LuaType.String && lua.Type(-2) == LuaType.String)
                        if (hs.TryGetValue(lua.ToString(-1), out _))
                            hs[lua.ToString(-1)] = lua.ToString(-2);
                        else
                            hs.Add(lua.ToString(-1), lua.ToString(-2));

                    lua.Pop(2);
                }
                lua.Pop(1);
            }

            HttpRequest.PostAsync(u, k1, k2, p, pa,hs);

            return 0;
        }

        static public int UploadImg(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);

            try
            {
                var result = FileManager.UploadImageAsync(lua.CheckString(1)).Result;
                var imageId = result.Item1;


                //MessageManager.SendGroupMessageAsync("616319393", image);
                //Util.Print(image.ToString());
                lua.NewTable();

                lua.PushString("Image");
                lua.SetField(-2, "Type");
                lua.PushString(imageId);
                lua.SetField(-2, "ImageId");

                return 1;
            }
            catch(Exception e)
            {
                Util.Print("上传图片失败：" + e.Message,Util.PrintType.ERROR,ConsoleColor.Red);
                return 0;
            }
        }
        static public int At(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            string tar = lua.CheckString(1);

            lua.NewTable();

            lua.PushString("At");
            lua.SetField(-2, "Type");
            lua.PushString(tar);
            lua.SetField(-2, "Target");

            return 1;
        }
        static MessageChain PackMsg(IntPtr p,int paramIndex)
        {
            Lua lua = Lua.FromIntPtr(p);
            string s = "";
            int argn = lua.GetTop();

            MessageChain mc = new MessageChain();

            for (int i = paramIndex; i <= argn; i++)
            {
                if (lua.Type(i) == LuaType.String)
                {
                    //s = Util.EncodingConvert(Encoding.GetEncoding("unicode"), Encoding.GetEncoding("utf-8"), s);
                    mc += lua.ToString(i);
                    s += lua.ToString(i);
                }
                else if (lua.Type(i) == LuaType.Table)
                {
                    lua.GetField(i, "Type");
                    string type = lua.ToString(-1);
                    lua.Remove(-1);

                    if (type == "Image")
                    {
                        lua.GetField(i, "ImageId");
                        string imgid = lua.ToString(-1);
                        lua.Remove(-1);

                        var image = new ImageMessage
                        {
                            ImageId = imgid
                        };

                        mc += image;
                        s += "[" + type + "]";
                    }
                    else if (type == "At")
                    {
                        lua.GetField(i, "Target");
                        string tar = lua.ToString(-1);
                        lua.Remove(-1);


                        var atm = new AtMessage(tar);

                        mc += atm;
                        s += "@" + tar;
                    }
                    //MessageManager.SendGroupMessageAsync(id, mc);
                    //s = Util.EncodingConvert(Encoding.GetEncoding("unicode"), Encoding.GetEncoding("utf-8"), s);
                }
                //Util.Print(lua.TypeName(i));
            }
            return mc;
        }
        static public int SendGroupMsgEX(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            string id = lua.CheckString(1);
            MessageChain mc = PackMsg(p,2);
            
            MessageManager.SendGroupMessageAsync(id, mc);
            //Util.Print("发送消息： " + id + " :" + s);
            return 0;
        }

        static public int SendTempMsgEX(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            string qid = lua.CheckString(1);
            string gid = lua.CheckString(2);
            MessageChain mc = PackMsg(p, 3);
            
            MessageManager.SendTempMessageAsync(qid, gid, mc);

            //Util.Print("发送消息： " + id + " :" + s);
            return 0;
        }
        static public int SendFriendMsgEX(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            string id = lua.CheckString(1);
            MessageChain mc = PackMsg(p, 2);

            MessageManager.SendFriendMessageAsync(id, mc);
            //Util.Print("发送消息： " + id + " :" + s);
            return 0;
        }

        static public int SendGroupMsg(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);

            string id = lua.CheckString(1);
            string t = lua.CheckString(2);

            MessageManager.SendGroupMessageAsync(id, t);
            //s = Util.EncodingConvert(Encoding.GetEncoding("unicode"), Encoding.GetEncoding("utf-8"), s);
            //Util.Print("发送消息：群 " + id + " .");

            return 0;
        }

        static public int SendFriendMsg(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);

            string id = lua.CheckString(1);
            string t = lua.CheckString(2);

            MessageManager.SendFriendMessageAsync(id, t);
            //s = Util.EncodingConvert(Encoding.GetEncoding("unicode"), Encoding.GetEncoding("utf-8"), s);
            //Util.Print("发送消息：群 " + id + " .");

            return 0;
        }

        static public int SendTempMsg(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);

            string qid = lua.CheckString(1);
            string gid = lua.CheckString(2);
            string t = lua.CheckString(3);

            MessageManager.SendTempMessageAsync(qid, gid, t);
            //s = Util.EncodingConvert(Encoding.GetEncoding("unicode"), Encoding.GetEncoding("utf-8"), s);
            //Util.Print("发送消息：群 " + id + " .");

            return 0;
        }
    }
}
