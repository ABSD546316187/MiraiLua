using KeraLua;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MiraiLua
{
    class ModuleLoader
    {
        static public List<ModuleInfo> m_hModule = new List<ModuleInfo>();
        static char g = Program.g;

        public struct ModuleInfo {
            public IntPtr p;
            public string Name;
            public string Version;
            public string Author;
        }

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr LoadLibrary(string lpFileName, int h, int flags);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lProcName);
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetLastError();

        delegate int Delegate_Init(IntPtr p,int datap,int datal);
        delegate void Delegate_InitCallback(IntPtr p,int l);
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
                        Util.Print("加载模块..." + f.Name);

                        try
                        {
                            IntPtr moduleP = LoadLibrary(f.FullName, 0, (int)LoaderOptimization.MultiDomain);
                            if (moduleP == 0)
                            {
                                Util.Print($"加载DLL失败(错误代码:{GetLastError()}).", Util.PrintType.ERROR, ConsoleColor.Red);
                                continue;
                            }
                            IntPtr initFunc = GetProcAddress(moduleP, "Init");

                            if (initFunc == 0)
                            {
                                Util.Print("加载DLL失败(可能是不存在初始化等必要函数?).", Util.PrintType.ERROR, ConsoleColor.Red);
                                continue;
                            }

                            Delegate_Init init = (Delegate_Init)Marshal.GetDelegateForFunctionPointer(initFunc, typeof(Delegate_Init));

                            Delegate_InitCallback d = new Delegate_InitCallback(delegate (IntPtr pt,int l) {
                                var b = new byte[l];
                                Marshal.Copy(pt, b, 0, l);
                                var d = new Data(b);

                                var info = new ModuleInfo()
                                {
                                    p = moduleP,
                                    Name = d.ReadString(),
                                    Version = d.ReadString(),
                                    Author = d.ReadString()
                                };

                                Util.Print($"指针：{info.p}\n模块名称：{info.Name}\n版本：{info.Version}\n作者：{info.Author}");

                                
                            });//回调函数，用于获取初始化信息

                            IntPtr CBPtr = Marshal.GetFunctionPointerForDelegate(d);

                            byte[] dataB = ModuleFunctions.GetFunsData();

                            int np = (int)Marshal.UnsafeAddrOfPinnedArrayElement(dataB, 0);

                            init(CBPtr, np, dataB.Length);
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
