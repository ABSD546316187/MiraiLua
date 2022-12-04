using System;
using System.Text;
using KeraLua;
using Mirai.Net.Sessions.Http.Managers;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Mirai.Net.Data.Messages.Concretes;
using System.Runtime.InteropServices;
using Mirai.Net.Data.Messages;

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
        static public int Reload(IntPtr p)
        {
            Program.LoadPlugins();

            return 0;
        }
        static public int SendGroupMsg(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);

            string id = lua.CheckString(1);
            string t = lua.CheckString(2);

            MessageManager.SendGroupMessageAsync(id,t);
            //s = Util.EncodingConvert(Encoding.GetEncoding("unicode"), Encoding.GetEncoding("utf-8"), s);
            Util.Print("发送消息：群 " + id + " .");

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

        static public int HttpGet(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);

            string serviceUrl = lua.CheckString(1);
            //创建Web访问对  象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            //通过Web访问对象获取响应内容
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
            string returnXml = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
            reader.Close();
            myResponse.Close();

            lua.PushString(returnXml);

            return 1;
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
        static public int SendGroupMsgEX(IntPtr p)
        {
            Lua lua = Lua.FromIntPtr(p);
            string id = lua.CheckString(1);
            string s = "";
            int argn = lua.GetTop();

            MessageChain mc = new MessageChain();

            for (int i = 2; i <= argn; i++)
            {
                if (lua.Type(i) == LuaType.String)
                {
                    //s = Util.EncodingConvert(Encoding.GetEncoding("unicode"), Encoding.GetEncoding("utf-8"), s);
                    mc += lua.ToString(i);
                    s += lua.ToString(i);
                }
                else if (lua.Type(i) == LuaType.Table)
                {
                    lua.GetField(i,"Type");
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
            MessageManager.SendGroupMessageAsync(id, mc);
            Util.Print("发送消息： " + id + " :" + s);
            return 0;
        }
    }
}
