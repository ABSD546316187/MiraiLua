using KeraLua;
using Manganese.Array;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiraiLua.Classes
{
    public static class ByteArray//数据结构类型，其中index 1表示从第1个字节开始读取
    {
        static Lua lua = Program.lua;
        static void CreateMeta(Action f) {//该方法压入一个元表，f为添加项目
            lua.NewMetaTable("ByteArray " + Guid.NewGuid().ToString());
            lua.PushCopy(-1);
            lua.SetField(-2, "__index");

            lua.PushCFunction(delegate(IntPtr p) {
                lua.GetField(-1, "data");
                lua.PushNil();
                int i = 0;
                while (lua.Next(-2))
                {
                    i++;
                    lua.PushCopy(-2);
                    lua.Pop(2);
                }
                lua.PushString("[ByteArray][" + i.ToString() + " 字节]");

                //Util.Print(lua.Type(-1).ToString());
                return 1;
            });
            lua.SetField(-2, "__tostring");

            lua.PushCFunction(delegate (IntPtr p) {
                lua.CheckType(1,LuaType.UserData);
                lua.CheckType(2,LuaType.UserData);

                var n1 = GetDataArr(1, 0, 1);
                var n2 = GetDataArr(1, 0, 2);
                var n = n1.Concat(n2).ToArray();

                lua.NewUserData(0);

                CreateMeta(delegate {
                    lua.NewTable();
                    for (int i = 1; i <= n.Length; i++)
                    {
                        lua.PushInteger(i);
                        lua.PushInteger(n[i - 1]);
                        lua.SetTable(-3);
                    }
                    lua.SetField(-2, "data");
                });
                //元表的创建
                lua.SetMetaTable(-2);

                return 1;
            });
            lua.SetField(-2, "__add");

            lua.PushCFunction(SetData);
            lua.SetField(-2, "SetData");
            lua.PushCFunction(GetData);
            lua.SetField(-2, "GetData");

            lua.PushCFunction(WriteShort);
            lua.SetField(-2, "WriteShort");
            lua.PushCFunction(ReadShort);
            lua.SetField(-2, "ReadShort");

            lua.PushCFunction(WriteInt);
            lua.SetField(-2, "WriteInt");
            lua.PushCFunction(ReadInt);
            lua.SetField(-2, "ReadInt");

            lua.PushCFunction(WriteLong);
            lua.SetField(-2, "WriteLong");
            lua.PushCFunction(ReadLong);
            lua.SetField(-2, "ReadLong");

            lua.PushCFunction(WriteFloat);
            lua.SetField(-2, "WriteFloat");
            lua.PushCFunction(ReadFloat);
            lua.SetField(-2, "ReadFloat");

            lua.PushCFunction(WriteDouble);
            lua.SetField(-2, "WriteDouble");
            lua.PushCFunction(ReadDouble);
            lua.SetField(-2, "ReadDouble");

            lua.PushCFunction(WriteString);
            lua.SetField(-2, "WriteString");
            lua.PushCFunction(ReadString);
            lua.SetField(-2, "ReadString");

            lua.PushCFunction(WriteBool);
            lua.SetField(-2, "WriteBool");
            lua.PushCFunction(ReadBool);
            lua.SetField(-2, "ReadBool");

            lua.PushCFunction(Add);
            lua.SetField(-2, "Add");
            f();
            //元表的创建
        }
        static int WriteString(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var n = lua.CheckString(2);
            var b0 = Encoding.UTF8.GetBytes(n).ToList();
            b0.Add(0);
            var b = GetDataArr(1).Concat(b0).ToArray();

            CreateMeta(delegate {
                lua.NewTable();
                for (int i = 1; i <= b.Length; i++)
                {
                    lua.PushInteger(i);
                    lua.PushInteger(b[i - 1]);
                    lua.SetTable(-3);
                }
                lua.SetField(-2, "data");
            });

            lua.SetMetaTable(1);

            return 0;
        }
        static int ReadString(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var ind = lua.CheckInteger(2);
            var b = GetDataArr(ind);

            var b1 = new List<byte>();
            foreach (var s in b)
            {
                if (s == 0)
                    break;
                b1.Add(s);
            }

            try
            {
                lua.PushString(Encoding.UTF8.GetString(b1.ToArray()));//返回字符串
                lua.PushInteger(b1.Count);//注意，string的字节集最后有一个空字符“0”，读取下一个数据时要+1偏移，这里返回的是字符串字节数
            }
            catch (Exception e)
            {
                lua.PushString(e.Message);
                lua.Error();
            }

            return 2;
        }
        static int WriteLong(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var n = lua.CheckInteger(2);
            var b = GetDataArr(1).Concat(BitConverter.GetBytes(n)).ToArray();

            CreateMeta(delegate {
                lua.NewTable();
                for (int i = 1; i <= b.Length; i++)
                {
                    lua.PushInteger(i);
                    lua.PushInteger(b[i - 1]);
                    lua.SetTable(-3);
                }
                lua.SetField(-2, "data");
            });

            lua.SetMetaTable(1);

            return 0;
        }
        static int ReadLong(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var ind = lua.CheckInteger(2);
            var b = GetDataArr(ind, 8).ToArray();

            try
            {
                lua.PushInteger(BitConverter.ToInt64(b, 0));
                lua.PushInteger(8);
            }
            catch (Exception e)
            {
                lua.PushString(e.Message);
                lua.Error();
            }

            return 2;
        }
        static int WriteInt(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var n = (int)lua.CheckInteger(2);
            var b = GetDataArr(1).Concat(BitConverter.GetBytes(n)).ToArray();

            CreateMeta(delegate {
                lua.NewTable();
                for (int i = 1; i <= b.Length; i++)
                {
                    lua.PushInteger(i);
                    lua.PushInteger(b[i - 1]);
                    lua.SetTable(-3);
                }
                lua.SetField(-2, "data");
            });

            lua.SetMetaTable(1);

            return 0;
        }
        static int ReadInt(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var ind = lua.CheckInteger(2);
            var b = GetDataArr(ind, 4).ToArray();

            try
            {
                lua.PushInteger(BitConverter.ToInt32(b, 0));
                lua.PushInteger(4);
            }
            catch (Exception e)
            {
                lua.PushString(e.Message);
                lua.Error();
            }

            return 2;
        }
        static int WriteShort(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var n = (short)lua.CheckInteger(2);
            var b = GetDataArr(1).Concat(BitConverter.GetBytes(n)).ToArray();

            CreateMeta(delegate {
                lua.NewTable();
                for (int i = 1; i <= b.Length; i++)
                {
                    lua.PushInteger(i);
                    lua.PushInteger(b[i-1]);
                    lua.SetTable(-3);
                }
                lua.SetField(-2, "data");
            });

            lua.SetMetaTable(1);

            return 0;
        }
        static int ReadShort(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var ind = lua.CheckInteger(2);
            var b = GetDataArr(ind,2).ToArray();

            try
            {
                lua.PushInteger(BitConverter.ToInt16(b, 0));
                lua.PushInteger(2);
            }
            catch (Exception e)
            {
                lua.PushString(e.Message);
                lua.Error();
            }
            
            return 2;
        }
        static int WriteBool(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);

            bool n = false;
            if (lua.ToString(2) != "false" && lua.ToString(2) != "0" && lua.ToString(2) != "nil")
                n = true;

            var b = GetDataArr(1).Concat(BitConverter.GetBytes(n)).ToArray();

            CreateMeta(delegate {
                lua.NewTable();
                for (int i = 1; i <= b.Length; i++)
                {
                    lua.PushInteger(i);
                    lua.PushInteger(b[i - 1]);
                    lua.SetTable(-3);
                }
                lua.SetField(-2, "data");
            });

            lua.SetMetaTable(1);

            return 0;
        }
        static int ReadBool(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var ind = lua.CheckInteger(2);
            var b = GetDataArr(ind, 1).ToArray();

            try
            {
                lua.PushBoolean(BitConverter.ToBoolean(b, 0));
                lua.PushInteger(1);
            }
            catch (Exception e)
            {
                lua.PushString(e.Message);
                lua.Error();
            }

            return 2;
        }
        static int WriteDouble(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var n = lua.CheckNumber(2);
            var b = GetDataArr(1).Concat(BitConverter.GetBytes(n)).ToArray();

            CreateMeta(delegate {
                lua.NewTable();
                for (int i = 1; i <= b.Length; i++)
                {
                    lua.PushInteger(i);
                    lua.PushInteger(b[i - 1]);
                    lua.SetTable(-3);
                }
                lua.SetField(-2, "data");
            });

            lua.SetMetaTable(1);

            return 0;
        }
        static int ReadDouble(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var ind = lua.CheckInteger(2);
            var b = GetDataArr(ind, 8).ToArray();

            try
            {
                lua.PushNumber(BitConverter.ToDouble(b, 0));
                lua.PushInteger(8);
            }
            catch (Exception e)
            {
                lua.PushString(e.Message);
                lua.Error();
            }

            return 2;
        }
        static int WriteFloat(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var n = (float)lua.CheckNumber(2);
            var b = GetDataArr(1).Concat(BitConverter.GetBytes(n)).ToArray();

            CreateMeta(delegate {
                lua.NewTable();
                for (int i = 1; i <= b.Length; i++)
                {
                    lua.PushInteger(i);
                    lua.PushInteger(b[i - 1]);
                    lua.SetTable(-3);
                }
                lua.SetField(-2, "data");
            });

            lua.SetMetaTable(1);

            return 0;
        }
        static int ReadFloat(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            var ind = lua.CheckInteger(2);
            var b = GetDataArr(ind, 4).ToArray();

            try
            {
                lua.PushNumber(BitConverter.ToSingle(b, 0));
                lua.PushInteger(4);
            }
            catch (Exception e)
            {
                lua.PushString(e.Message);
                lua.Error();
            }

            return 2;
        }

        static int SetData(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            lua.CheckType(2, LuaType.Table);

            CreateMeta(delegate {
                lua.PushCopy(2);
                lua.SetField(-2, "data");
            });
            
            lua.SetMetaTable(1);

            return 0;
        }
        static int GetData(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);

            lua.GetField(1, "data");

            return 1;
        }
        static public int New(IntPtr p)
        {
            lua.NewUserData(0);

            CreateMeta(delegate {
                if (lua.Type(1) == LuaType.Table)
                {
                    lua.PushCopy(1);
                    lua.SetField(-2, "data");
                }
                else
                {
                    lua.NewTable();
                    lua.SetField(-2, "data");
                }
            });
            //元表的创建
            lua.SetMetaTable(-2);

            return 1;
        }
        static public int LoadFile(IntPtr p)
        {
            var path = lua.CheckString(1);
            try
            {
                var n = System.IO.File.ReadAllBytes(path);

                lua.NewUserData(0);

                CreateMeta(delegate {
                    lua.NewTable();
                    for (int i = 1; i <= n.Length; i++)
                    {
                        lua.PushInteger(i);
                        lua.PushInteger(n[i - 1]);
                        lua.SetTable(-3);
                    }
                    lua.SetField(-2, "data");
                });
                //元表的创建
                lua.SetMetaTable(-2);

                return 1;
            }
            catch(Exception e)
            {
                lua.NewUserData(0);

                CreateMeta(delegate {
                    lua.NewTable();
                    lua.SetField(-2, "data");
                });
                lua.SetMetaTable(-2);

                lua.PushString(e.Message);
                return 2;
            }
        }
        static public int SaveFile(IntPtr p)
        {
            lua.CheckType(2, LuaType.UserData);

            var path = lua.CheckString(1);

            var n = GetDataArr(1,0,2).ToArray();

            try
            {
                System.IO.File.WriteAllBytes(path,n);
                lua.PushBoolean(true);
                return 1;
            }
            catch(Exception e)
            {
                lua.PushBoolean(false);
                lua.PushString(e.Message);
                return 2;
            }
        }

        static public int Add(IntPtr p)
        {
            lua.CheckType(1, LuaType.UserData);
            lua.CheckType(2, LuaType.Table);

            var b = GetDataArr(1);

            lua.PushCopy(2);
            lua.PushNil();

            while (lua.Next(-2))
            {
                lua.PushCopy(-2);
                var n = lua.ToInteger(-2);
                b.Add((byte)n);
                //Util.Print(lua.ToString(-1) + " " + lua.ToString(2));
                lua.Pop(2);
            }

            CreateMeta(delegate {
                lua.NewTable();
                for (int i = 1; i <= b.Count; i++)
                {
                    lua.PushInteger(i);
                    lua.PushInteger(b[i - 1]);
                    lua.SetTable(-3);
                }
                lua.SetField(-2, "data");
            });

            lua.SetMetaTable(1);

            return 0;
        }
        static List<byte> GetDataArr(long ind,int length = 0,int udind = 1)//udind为Userdata在栈的位置
        {
            var b = new List<byte>();
            lua.GetField(udind, "data");

            if (ind > 1)
                lua.PushInteger(ind - 1);
            else
                lua.PushNil();

            int i = 0;
            while (lua.Next(-2))
            {
                i++;

                lua.PushCopy(-2);
                b.Add((byte)lua.ToInteger(-2));
                //Util.Print(lua.ToString(-1) + " " + lua.ToString(2));
                lua.Pop(2);

                if (length > 0 && i >= length)
                    break;
            }
            return b;
        }
    }
}
