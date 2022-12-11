# MiraiLua
MiraiLua是基于 [Mirai.Net](https://github.com/SinoAHpx/Mirai.Net) / [mirai-api-http](https://github.com/project-mirai/mirai-api-http) 编写的以Lua为脚本引擎的QQ机器人框架。

## 使用方法

- 安装.NET Core 3.1 SDK (暂时需要这么做)
- 配置主程序目录下的 `settings.xml`
  - `Address` 是 [mirai-api-http](https://github.com/project-mirai/mirai-api-http) 中配置的地址
  - `QQ` 是 [mirai](https://github.com/mamoe/mirai) 中配置的QQ号
  - `Key` 是 [mirai-api-http](https://github.com/project-mirai/mirai-api-http) 中配置的 `VerifyKey` (如果存在)
- 配置 `plugins/basic/init.lua` 第2行 `enableQ` 为启用的群列表
 
## 部分API

本框架处于开发初期，以下列出已经开发好的api
```lua
void api.Reload()                                       --重载插件
void api.SendGroupMsg(string GroupID, string text)      --发送群组消息
void api.SendGroupMsgEX(string GroupID, ...)            --发送群组消息，后面为可变参数，可解析上传图片等高级接口返回的table
void api.OnReceiveGroup(table data)                     --接收到消息后由C#调用，结构见下文
string api.HttpGet(string url)                          --调用Http Api(GET)

table api.UploadImg(string path)                        --上传本地图片，返回格式化表
table api.At(string qq)                                 --艾特，返回格式化表
```
- 示例
```lua
function api.OnReceiveGroup(data)
	PrintTable(data)
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

## 注意

- 在发布版本中，我已经写好了 `basic` 和 `util` 的脚本，其中部分需要被C#调用，请不要随意更改。
- 插件的写法参考已经写好的2个插件，这里不赘述（懒）。