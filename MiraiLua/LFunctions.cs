using System;
using KeraLua;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages;
using System.Collections.Generic;
using Mirai.Net.Utils.Scaffolds;
using System.IO;
using System.Drawing;
using Manganese.Text;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Diagnostics;
using System.Net;

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

        static public int Base64ToString(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            string s = lua.CheckString(1);
            byte[] c = Convert.FromBase64String(s);
            s = Encoding.Default.GetString(c);
            lua.PushString(s);
            return 1;
        }

        static public int StringToBase64(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            string s = lua.CheckString(1);
            byte[] b = Encoding.Default.GetBytes(s);
            s = Convert.ToBase64String(b);
            lua.PushString(s);
            return 1;
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
            try
            {
                s = $"plugins{g}{Util.GetMiddleStr(s, $".{g}plugins{g}", g.ToString())}";
                lua.PushString(s);
                return 1;
            }
            catch
            {
                lua.PushNil();
                return 1;
            }
        }
        static public int Reload(IntPtr p)
        {
            Program.LoadPlugins();
            return 0;
        }
        static public int LocalBot(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            lua.PushString(Program.bot.QQ);
            return 1;
        }
        static public int Unescape(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            var s = lua.CheckString(1);
            lua.PushString(Regex.Unescape(s));
            return 1;
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
            var pa = new Dictionary<string, string>();
            var pas = "";
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

            hs.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:107.0) Gecko/20100101 Firefox/107.0");
            
            if (lua.Type(4) == LuaType.Table)
            {

                lua.PushCopy(4);
                lua.PushNil();
                while (lua.Next(-2))
                {
                    lua.PushCopy(-2);
                    if (lua.Type(-1) == LuaType.String)
                        pa.Add(lua.ToString(-1), lua.ToString(-2));
                    lua.Pop(2);
                }
                lua.Pop(1);
                pas = JsonConvert.SerializeObject(pa);
            }
            else if (lua.Type(4) == LuaType.String)
            {
                pas = lua.ToString(4);
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

            HttpRequest.PostAsync(u, k1, k2, p, pas, hs);

            return 0;
        }


        static public int HttpDownloadA(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            string u = lua.CheckString(1);

            string? onS = null;
            string? onF = null;
            if (lua.Type(2) == LuaType.Function)
            {
                onS = lua.ToString(2);
                lua.PushCopy(2);
                lua.SetGlobal(onS);
            }
            if (lua.Type(3) == LuaType.Function)
            {
                onF = lua.ToString(3);
                lua.PushCopy(3);
                lua.SetGlobal(onF);
            }

            HttpRequest.DownloadAsync(u, onS, onF, p);

            return 0;
        }

        static public int UploadVoiceBase64(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            lua.CheckType(1, LuaType.UserData);
            try
            {
                string pic = "";
                if (lua.Type(1) == LuaType.String)
                {
                    pic = lua.CheckString(1);
                }
                else if (lua.Type(1) == LuaType.UserData)
                {
                    var b = ByteArray.GetDataArr(1).ToArray();
                    pic = Convert.ToBase64String(b);
                }
                var re = new MessageChainBuilder().ImageFromBase64(pic).Build();

                var json = re.ToJsonString().Replace("[", "").Replace("]", "");

                var rspObj = JsonConvert.DeserializeAnonymousType(json, new
                {
                    base64 = "",
                });

                lua.NewTable();
                lua.PushString("Voice");
                lua.SetField(-2, "Type");
                lua.PushString(rspObj.base64);
                lua.SetField(-2, "Base64");

                return 1;
            }
            catch (Exception e)
            {
                Util.Print("上传语音失败：" + e.Message, Util.PrintType.ERROR, ConsoleColor.Red);
                return 0;
            }
        }

        static public int UploadImgBase64(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            // lua.CheckType(1, LuaType.UserData);
            try
            {
                string pic = "";
                if(lua.Type(1) == LuaType.String)
                {
                    pic = lua.CheckString(1);
                }
                else if (lua.Type(1) == LuaType.UserData)
                {
                    var b = ByteArray.GetDataArr(1).ToArray();
                    pic = Convert.ToBase64String(b);
                }
                var re = new MessageChainBuilder().ImageFromBase64(pic).Build();

                var json = re.ToJsonString().Replace("[", "").Replace("]", "");

                var rspObj = JsonConvert.DeserializeAnonymousType(json, new
                {
                    base64 = "",
                });

                lua.NewTable();
                lua.PushString("Image");
                lua.SetField(-2, "Type");
                lua.PushString(rspObj.base64);
                lua.SetField(-2, "Base64");

                return 1;
            }
            catch (Exception e)
            {
                Util.Print("上传图片失败：" + e.Message, Util.PrintType.ERROR, ConsoleColor.Red);
                return 0;
            }
        }

        /*
        static public int UploadVoiceAmr(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            //取得ffmpeg.exe的物理路径
            string sourceFile = lua.CheckString(1);
            string ffmpeg = AppDomain.CurrentDomain.BaseDirectory + "ffmpeg.exe";
            if (!File.Exists(ffmpeg))
            {
                Util.Print("上传语音失败：找不到格式转换程序", Util.PrintType.ERROR, ConsoleColor.Red);
                return 0;
            }

            var g = Program.g;
            if (!Directory.Exists($".{g}temp"))
                Directory.CreateDirectory($".{g}temp");

            string filen = Guid.NewGuid() + ".amr";
            string destFile = AppDomain.CurrentDomain.BaseDirectory + $"temp\\{filen}";

            ProcessStartInfo FilestartInfo = new ProcessStartInfo(ffmpeg);
            FilestartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            FilestartInfo.Arguments = " -i " + sourceFile + " -y -abort_on empty_output -v quiet -ab 12.2k -ar 8000 -ac 1 " + destFile;

            //转换
            var t = Process.Start(FilestartInfo);
            t.WaitForExit();

            if (t.ExitCode != 0)
            {
                Util.Print("上传语音失败。错误代码：" + t.ExitCode.ToString(), Util.PrintType.ERROR, ConsoleColor.Red);
                return 0;
            }

            try
            {
                var result = FileManager.UploadVoiceAsync(destFile).Result;
                var Id = result.Item1;

                lua.NewTable();

                lua.PushString("Voice");
                lua.SetField(-2, "Type");
                lua.PushString(Id);
                lua.SetField(-2, "VoiceId");

                File.Delete(destFile);

                return 1;
            }
            catch (Exception e)
            {
                Util.Print("上传语音失败：" + e.Message, Util.PrintType.ERROR, ConsoleColor.Red);
                return 0;
            }
        }
        */
        

        static public int UploadVoice(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            //取得ffmpeg.exe的物理路径
            string sourceFile = lua.CheckString(1);
            try
            {
                var result = FileManager.UploadVoiceAsync(sourceFile).Result;
                var Id = result.Item1;

                lua.NewTable();

                lua.PushString("Voice");
                lua.SetField(-2, "Type");
                lua.PushString(Id);
                lua.SetField(-2, "VoiceId");

                return 1;
            }
            catch (Exception e)
            {
                Util.Print("上传语音失败：" + e.Message, Util.PrintType.ERROR, ConsoleColor.Red);
                return 0;
            }
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
                        var image = new ImageMessage();
                        lua.GetField(i, "ImageId");
                        if(lua.Type(-1) == LuaType.Nil)
                        {
                            lua.Remove(-1);
                            lua.GetField(i, "Base64");
                            string base64 = lua.ToString(-1);
                            lua.Remove(-1);
                            image.Base64 = base64;
                        }
                        else
                        {
                            string imgid = lua.ToString(-1);
                            lua.Remove(-1);
                            image.ImageId = imgid;
                        }
                        mc += image;
                        s += "[" + type + "]";
                    }
                    else if (type == "Voice")
                    {
                        var image = new VoiceMessage();
                        lua.GetField(i, "VoiceId");
                        if (lua.Type(-1) == LuaType.Nil)
                        {
                            lua.Remove(-1);
                            lua.GetField(i, "Base64");
                            string base64 = lua.ToString(-1);
                            lua.Remove(-1);
                            image.Base64 = base64;
                        }
                        else
                        {
                            string imgid = lua.ToString(-1);
                            lua.Remove(-1);
                            image.VoiceId = imgid;
                        }
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
