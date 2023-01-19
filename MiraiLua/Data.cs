using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiraiLua
{
    class Data
    {
        int ind = 0;
        List<byte> b = new List<byte>();
        public Data(byte[] data)
        {
            b = data.ToList();
        }
        public byte[] GetData()
        {
            return b.ToArray();
        }
        public void WriteInt(int i)
        {
            b = b.Concat(BitConverter.GetBytes(i)).ToList();
        }
        public int ReadInt()
        {
            var i = BitConverter.ToInt32(b.ToArray(), ind);
            ind += 4;
            return i;
        }
        public void WriteFloat(float i)
        {
            b = b.Concat(BitConverter.GetBytes(i)).ToList();
        }
        public float ReadFloat()
        {
            var i = BitConverter.ToSingle(b.ToArray(), ind);
            ind += 4;
            return i;
        }

        public void WriteBool(bool i)
        {
            b = b.Concat(BitConverter.GetBytes(i)).ToList();
        }
        public bool ReadBool()
        {
            var i = BitConverter.ToBoolean(b.ToArray(), ind);
            ind += 1;
            return i;
        }

        public void WriteString(string i)
        {
            b = b.Concat(Encoding.UTF8.GetBytes(i)).ToList();
            b.Add(0);
        }

        public string ReadString()
        {
            var b1 = new List<byte>();
            for (int i = ind; i < b.Count; i++)
            {
                if (b[i] == 0)
                    break;
                b1.Add(b[i]);
            }

            var s = Encoding.UTF8.GetString(b1.ToArray());
            ind += b1.Count + 1;
            return s;
        }
    }
}
