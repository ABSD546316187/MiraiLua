## MiraiLua
MiraiLua是基于 [Mirai.Net] / [mirai-api-http] 编写的以Lua为脚本引擎的QQ机器人框架。

#使用方法
- 安装.NET Core 3.1 SDK (暂时需要这么做)
- 配置主程序目录下的 `settings.xml`
  - `Address` 是 [mirai-api-http] 中配置的地址
  - `QQ` 是 [mirai] 中配置的QQ号
  - `Key` 是 [mirai-api-http] 中配置的 `VerifyKey` (如果存在)
  
#部分API
本框架处于开发初期，以下列出已经开发好的api
```cs
api.Reload()                                      //重载插件
api.SendGroupMsg(string GroupID, string text)     //发送群组消息
api.SendGroupMsgEX(string GroupID, ...)           //发送群组消息，后面为可变参数，可解析上传图片等高级接口返回的table
api.OnReceiveGroup()
api.HttpGet()

api.UploadImg()
api.At()
```
