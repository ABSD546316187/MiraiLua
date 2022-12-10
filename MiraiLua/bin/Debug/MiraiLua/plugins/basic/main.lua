--命令系统
local cmdt = {}
function AddCmd(name,admin,help,func)--参数分别为命令名，是否为管理员权限，帮助信息，回调函数
	cmdt[name] = {F = func,H = help,Admin = admin}
end

api.ReceiveQ("cmds",function (data)
	local s = data.MsgCon
	if string.sub(s, 1, 1) == "." then
		local t = string.split(string.sub(s,2,#s)," ")
		local cmd = t[1]
		if cmd ~= "" then
			table.remove(t,1)
			for k,v in pairs(cmdt) do
				if k == cmd and (v.Admin and data.SenderRank <= 1 or not v.Admin) then
					v.F(data,table.unpack(t))
				end
			end
		end
	end
end)

AddCmd("reload",true,"重载脚本",function (data,...)--命令的添加函数都是这样的格式
	local args = {...}--获取后续的参数

	--下面自定义
	api.SendGroupMsg(data.GroupID,"所有脚本已经重载.")
	api.Reload()
end)

AddCmd("lua",true,"执行一段lua",function (data,...)
	local args = {...}
	local s = ""
	for k,v in pairs(args) do
		if k ~= 1 then
			s = s .. " "
		end
		s = s .. v
	end
	--下面自定义
	if args[1] then
		local state,err = pcall(load(s))
		if not state then
			api.SendGroupMsg(data.GroupID,err)
		end
	else
		api.SendGroupMsg(data.GroupID,"命令格式:.lua <script>")
	end
end)

AddCmd("help",false,"获取帮助",function (data,...)
	local args = {...}

	local s = "养老天堂服务Bot帮助列表\n"
	for k,v in pairs(cmdt) do
		if not v.Admin then
			s = s .. "\n>." .. k .. " - " .. v.H
		end
	end
	if data.SenderRank <= 1 then
		s = s .. "\n\n以下为管理员命令:"
		for k,v in pairs(cmdt) do
			if v.Admin then
				s = s .. "\n>." .. k .. " - " .. v.H
			end
		end
	end
	
	s = s .. "\n\nPowered by ABSD"
	api.SendGroupMsg(data.GroupID,s)
end)

AddCmd("说",false,"让bot复读",function (data,...)
	local args = {...}
	local s = ""
	for k,v in pairs(args) do
		if k ~= 1 then
			s = s .. " "
		end
		s = s .. v
	end
	--下面自定义
	if args[1] then
		api.SendGroupMsg(data.GroupID,s)
	else
		api.SendGroupMsg(data.GroupID,"你说你马呢")
	end
end)

AddCmd("img",true,"test",function (data,...)
	local args = {...}
	api.SendGroupMsg(data.GroupID,"[pic,hash=A3773E37EBB956648268B296B09CEC1E,url=http://gchat.qpic.cn/gchatpic_new/2624860918/616319393-2756147542-A3773E37EBB956648268B296B09CEC1E/0?term=3&is_origin=0,wide=64,high=66,cartoon=false]")
end)