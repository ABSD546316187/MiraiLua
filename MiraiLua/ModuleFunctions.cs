using KeraLua;
using System;

namespace MiraiLua
{
    public class ModuleFunctions
    {
        static public Lua GetLua()
        {
            return Program.lua;
        }

        static public object GetLock()
        {
            return Program.o;
        }
    }
}
