using KeraLua;
using System;
using System.Text;

namespace MiraiLua
{
    public static class Util
    {
        public enum PrintType { INFO,WARNING,ERROR }
        static public void Print(object t,PrintType type = PrintType.INFO,ConsoleColor c = ConsoleColor.Gray) {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(DateTime.Now.ToString("T") + " ");
            Console.Write("[");

            if (type == PrintType.INFO) {
                Console.Write("信息");
            }
            else if (type == PrintType.WARNING) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("警告");
            }
            else if (type == PrintType.ERROR)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("错误");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");

            Console.ForegroundColor = c;
            Console.Write(t + "\n");

            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static string EncodingConvert(Encoding src, Encoding dst, string text)
        {
            var bytes = src.GetBytes(text);
            bytes = Encoding.Convert(src, dst, bytes);
            return dst.GetString(bytes);
        }

        public static void PushFunction(string table,string name,Lua lua,LuaFunction f)
        {
            lua.GetGlobal(table);
            if (lua.Type(-1) == LuaType.Nil)//不存在就创建
            {
                lua.NewTable();
                lua.SetGlobal(table);
            }
            lua.GetGlobal(table);
            lua.Remove(-2);
            lua.PushCFunction(f);
            lua.SetField(1, name);
            lua.Pop(1);
        }

        /// <summary>
        /// 取中间文本 + static string GetMiddleStr(string oldStr,string preStr,string nextStr)
        /// </summary>
        /// <param name="oldStr">原文</param>
        /// <param name="preStr">前文</param>
        /// <param name="nextStr">后文</param>
        /// <returns></returns>
        public static string GetMiddleStr(string oldStr, string preStr, string nextStr)
        {
            string tempStr = oldStr.Substring(oldStr.IndexOf(preStr) + preStr.Length);
            tempStr = tempStr.Substring(0, tempStr.IndexOf(nextStr));
            return tempStr;
        }
    }
}
