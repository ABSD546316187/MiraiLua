# MiraiLua
MiraiLua是基于 [Mirai.Net](https://github.com/SinoAHpx/Mirai.Net) / [mirai-api-http](https://github.com/project-mirai/mirai-api-http) 编写的以Lua为脚本引擎的QQ机器人框架。

## 使用方法

- 安装.NET7 (暂时需要这么做)
- 最新版本的源码支持dll模块，请使用.Net 7.0的框架进行开发。简易教程见下。
- 配置主程序目录下的 `settings.xml`
  - `Address` 是 [mirai-api-http](https://github.com/project-mirai/mirai-api-http) 中配置的地址
  - `QQ` 是 [mirai](https://github.com/mamoe/mirai) 中配置的QQ号
  - `Key` 是 [mirai-api-http](https://github.com/project-mirai/mirai-api-http) 中配置的 `VerifyKey` (如果存在)
- 配置 `plugins/basic/init.lua` 第2行 `enableQ` 为启用的群列表
 
## 部分API

本框架处于开发初期，以下列出已经开发好的api
### API
```lua
void api.Reload()                                       --重载插件
string api.LocalBot()                                   --获取当前bot的QQ号
void api.SendGroupMsg(string GroupID, string text)      --发送群组消息
void api.SendTempMsg(string QQID, string GroupID, string text)
                                                        --发送临时消息
void api.SendFriendMsg(string QQID, string text)        --发送好友消息
void api.SendGroupMsgEX(string GroupID, ...)            --发送群组消息，后面为可变参数，可解析上传图片等高级接口返回的table
void api.SendTempMsgEX(string QQID, string GroupID, ...)--发送临时消息，后面为可变参数，可解析上传图片等高级接口返回的table
void api.SendFriendMsgEX(string QQID, ...)              --发送好友消息，后面为可变参数，可解析上传图片等高级接口返回的table
void api.OnReceiveGroup(table data)                     --接收到消息后由C#调用，结构见下文
void api.HttpGet(                                       --调用Http Api(GET)
	string url,
	function onSuccess,
	function onFailure,
	table headers = {["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:107.0) Gecko/20100101 Firefox/107.0"}
)

void api.HttpPost(                                      --调用Http Api(POST)
	string url,
	function onSuccess,
	function onFailure,
	table params = {},
	table headers = {["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:107.0) Gecko/20100101 Firefox/107.0"}
)

table api.UploadImg(string path)                        --上传本地图片，返回格式化表
table api.UploadImgBase64(ByteArray data)               --将ByteArrary内的数据作为图片上传，返回格式化表
table api.At(string qq)                                 --艾特，返回格式化表
```
### Global
```lua
void include(string path)                               --加载文件，该函数只能在加载插件的时候调用
string GetDir()                                         --获取当前脚本的运行目录。注意：不自带最后的"\"
ByteArray ByteArray(table data = {})                    --创建一个字节数组
ByteArray LoadFile(string path)                         --读入文件，返回字节数组。失败返回长度为0的ByteArray并附加一个错误信息
boolean SaveFile(string path,ByteArray data)            --保存文件。成功返回true，失败返回false并附加一个错误信息
```
### ByteArray
注意，以下"ByteArray"均为ByteArray的对象，而不是其元表本身
```lua
void ByteArray:WriteBool(any n)                         --写入bool(1字节) 除了0、nil、false均视为true(包括no value)
void ByteArray:WriteShort(number n)                     --写入short(2字节)
void ByteArray:WriteInt(number n)                       --写入int(4字节)
void ByteArray:WriteLong(number n)                      --写入long(8字节)
void ByteArray:WriteFloat(number n)                     --写入float(4字节)
void ByteArray:WriteDouble(number n)                    --写入double(8字节)
void ByteArray:WriteString(string s)                    --写入string

boolean ByteArray:ReadBool()                            --读取bool(1字节)并返回长度
number ByteArray:ReadShort()                            --读取short(2字节)并返回长度
number ByteArray:ReadInt()                              --读取int(4字节)并返回长度
number ByteArray:ReadLong()                             --读取long(8字节)并返回长度
number ByteArray:ReadFloat()                            --读取float(4字节)并返回长度
number ByteArray:ReadDouble()                           --读取double(8字节)并返回长度
string,number ByteArray:ReadString()                    --读取string并返回长度（不计字符串最后的"0"字节）

void ByteArray:Add(ByteArray ba)                        --与另一个ByteArray相加，ba将会加在其末尾，也可以使用"+"运算符
void ByteArray:SetData(table data)                      --通过一个table设置该字节数组的数据，也可以使用ByteArray.data = xxx
table ByteArray:GetData()                               --将该字节数组的数据转换成table，也可以使用ByteArray.data
```
- 示例
```lua
function api.OnReceiveGroup(data)
	PrintTable(data)--接收到消息后，将会打印出data的结构。
end
```
当有人说话，你会得到类似的输出：
```
Log output date: 2022-12-04 18:03:53
    SenderID = "xxx",
    Data = {
        [1] = {
            MessageId = "16174",
            Type = "Source",
            Time = "1670148233",
        },
        [2] = {
            Type = "Plain",
            Text = "123",
        },
        [3] = {
            ImageId = "{Axxxxx66-3ExC-8xx3-6xxx4xxxxx9}.jpg",
            Width = "88",
            Height = "62",
            Path = "",
            Base64 = "",
            Url = "http://gchat.qpic.cn/gchatpic_new/xxx/xxx/0?term=2&is_origin=0",
            Type = "Image",
        },
    },
    From = "Group",
    SenderRank = 0.0,
    GroupName = "xxx",
    SenderName = "xxx",
    GroupID = "xxx",
```
- GET示例
```lua
api.HttpGet("https://api.bilibili.com/x/space/acc/info?mid=114514",
	function(data)
		print(data)
	end,
	function(msg)
		print(msg)
	end
)
```
该函数访问b站的api，如果成功则输出"data"的内容，如果失败则输出错误信息"msg"。你也可以不写怎么处理，但必须给2个function类型的变量。
- POST示例
```lua
api.HttpPost("https://api.ownthink.com/bot",
	function(data)
		print(data)
	end,
	function(msg)
		print(msg)
	end,
	{
		spoken = "hello"
	}
)
```
你会得到类似的输出：
```JSON
{
    "message": "success",
    "data": {
        "type": 5000,
        "info": {
            "text": "是一个问候的单词"
        }
    }
}
```
- ByteArray示例
```lua
local ba = ByteArray()

local ba1 = ByteArray()
local ba2 = ByteArray()

ba:WriteInt(114514)
ba:WriteLong(1145141919810)
ba1:WriteFloat(19.19)
ba1:WriteDouble(114.5141919)
ba2:WriteString("测试")
ba2:WriteBool(1234)
ba2:WriteInt(1234)

--ba:WriteString("你说的")
--PrintTable(ba.data)
--PrintTable(ba1.data)
local ba3 = ba + ba1 + ba2
print(ba3:ReadInt(1))
print(ba3:ReadLong(5))
print(ba3:ReadFloat(13))
print(ba3:ReadDouble(17))
local t,l = ba3:ReadString(25)	--t:读取的文本 l:文本长度
print(t,l)
print(ba3:ReadBool(26+l))	--这里为25+1+l是因为ReadString返回的长度不计最后的"0"
print(ba3:ReadInt(26+l+1))
print(ba,ba1,ba2,ba3)

local ba,e=LoadFile(GetDir() .. "t.txt")	--t.txt的内容为"你说得对，但是《原神》..."
if #ba.data == 0 then	--失败输出错误信息
	print(e)
else
	print(ba:ReadString(1))
	print(SaveFile(GetDir() .. "t2.txt",ba))
end

print(ba)
```
你会得到类似的输出：
```
11:45:14 [信息] 114514    4
11:45:14 [信息] 1145141919810    8
11:45:14 [信息] 19.190000534058    4
11:45:14 [信息] 114.5141919    8
11:45:14 [信息] 测试    6
11:45:14 [信息] true    1
11:45:14 [信息] 1234    4
11:45:14 [信息] [ByteArray][12 字节]    [ByteArray][12 字节]    [ByteArray][12 字节]    [ByteArray][36 字节]
11:45:14 [信息] 你说的对，但是《原神》是由米哈游自主研发的一款全新开放世界冒险游戏。游戏发生在一个被称作「提瓦特」的幻想世界，在这里，被神选中的人将被授予「神之眼」，导引元素之力。你将扮演一位名为「旅行者」的神秘角色，在自由的旅行中邂逅性格各异、能力独特的同伴们，和他们一起击败强敌，找回失散的亲人——同时，逐步发掘「原神」的真相。    474
11:45:14 [信息] true
11:45:14 [信息] [ByteArray][474 字节]
```
其中，目录中会生成t2.txt，其内容与t.txt一致

- 模块基本框架
```csharp
using KeraLua;
using MiraiLua;
namespace Example_Module//这里名称随意，但最好不要与其他模块重复
{
    public class MiraiLuaModule//类名必须这么写
    {
        static public Data Init()//程序入口
        {
            var data = new Data();
            data.WriteString("模块名称");
            data.WriteString("版本");
            data.WriteString("作者");//注意顺序
            
            //加入你自己的代码

            return data;
        }
    }
}
```

- 示例
```csharp
using KeraLua;
using MiraiLua;
namespace Example_Module
{
    public class MiraiLuaModule
    {
        static public Data Init()
        {
            var data = new Data();
            data.WriteString("C# .net7示例模块");
            data.WriteString("1.0");
            data.WriteString("ABSD");

            var lua = ModuleFunctions.GetLua();
            Util.PushFunction("ExampleMod", "Test", lua, delegate (IntPtr p) {
                Util.Print("Hello, MiraiLua!", Util.PrintType.WARNING, ConsoleColor.Red);
                Util.Print("Hello, MiraiLua!", Util.PrintType.ERROR, ConsoleColor.Yellow);
                Util.Print("Hello, MiraiLua!", Util.PrintType.INFO, ConsoleColor.Green);
                Util.Print("Hello, MiraiLua!", Util.PrintType.WARNING, ConsoleColor.Blue);
                Util.Print("Hello, MiraiLua!", Util.PrintType.ERROR, ConsoleColor.Magenta);
                return 0;
            });

            Util.Print("Hello, MiraiLua!",Util.PrintType.INFO,ConsoleColor.Gray);
            
            return data;
        }
    }
}
```
在以上的示例中，`Util.PushFunction()`用于压入自定义Lua函数，参数分别为函数所在的表(全局表为_G)、函数名称、Lua对象、C#方法。其中Lua对象需要使用`ModuleFunctions.GetLua()`获取，C#方法这里使用了匿名方法，当然你也可以在下面定义一个方法来代替。

压入Lua的方法必须有一个IntPtr的参数(代表Lua对象的指针，使用`Lua.FromIntPtr(p)`来获取Lua对象)和一个int的返回值(代表对应Lua函数的返回值个数)。

注意：
- 模块的语言使用C#，并且需要在项目中引用主程序目录下的`MiraiLua.dll`与`KeraLua.dll`。
- 命名空间随意，但必须有`MiraiLuaModule`的公开类与`Init()`的正确定义。
- 最好不要使用NuGet包，因为必须要将编译后的相应dll放入主程序目录，显得杂乱。除非你有好的方法合并模块dll与外部dll，否则请考虑从源码方面并入。
- Lua的灵活运用需要了解栈的概念以及学习如何进行Lua栈操作。
- MiraiLua仅用于加载、调用模块，不对模块的具体内容负责。请自行甄别模块是否安全。

## 注意

- 在发布版本中，我已经写好了 `basic` 和 `util` 的脚本，其中部分需要被C#调用，请不要随意更改。
- 插件的写法参考已经写好的2个插件，这里不赘述（懒）。