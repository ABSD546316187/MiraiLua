QmsgtG = {}
QmsgtF = {}
QmsgtT = {}

local enableQ = {}

function isValidQ(n)
	for _,v in pairs(enableQ) do
		if v == n then
			return true
		end
	end
	return false
end

function api.ReceiveQ(q,func)--参数为一个独一无二的标识符,回调函数
	QmsgtG[q]=func
end

function api.ReceiveQF(q,func)--参数为一个独一无二的标识符,回调函数
	QmsgtF[q]=func
end

function api.ReceiveQT(q,func)--参数为一个独一无二的标识符,回调函数
	QmsgtT[q]=func
end

local function Data2Text(data)
	local s = ""
	for _,v in pairs(data.Data) do
		if v.type == "Plain" then
			s = s .. v.text
		elseif v.type ~= "Source" then
			s = s .. "[" .. v.type .. "]"
		end
	end
	return s
end

function api.OnReceiveGroup(data)--群号/Q号均为string
	--PrintTable(data)
	local s = Data2Text(data)
	print(string.format("[%s][%s]：%s",data.GroupName,data.SenderName,s))
	
	if data.SenderID == api.LocalBot() then return end
	if not isValidQ(data.GroupID) then return end
	for _,v in pairs(QmsgtG) do
		v(data)
	end
end

function api.OnReceiveFriend(data)
	--PrintTable(data)
	local s = Data2Text(data)
	print(string.format("[Friend][%s]：%s",data.SenderName,s))
	if data.SenderID == api.LocalBot() then return end
	for _,v in pairs(QmsgtF) do
		v(data)
	end
end

function api.OnReceiveTemp(data)
	--PrintTable(data)
	local s = Data2Text(data)
	print(string.format("[Temp %s][%s]：%s",data.GroupName,data.SenderName,s))
	if data.SenderID == api.LocalBot() then return end
	for _,v in pairs(QmsgtT) do
		v(data)
	end
end
---------------------------------
-- log输出格式化
local function logPrint(str)
    str = os.date("\nLog output date: %Y-%m-%d %H:%M:%S \n", os.time()) .. str
    print(str)
end
 
-- key值格式化
local function formatKey(key)
    local t = type(key)
    if t == "number" then
        return "["..key.."]"
    elseif t == "string" then
        local n = tonumber(key)
        if n then
            return "["..key.."]"
        end
    end
    return key
end
 
-- 栈
local function newStack()
    local stack = {
        tableList = {}
    }
    function stack:push(t)
        table.insert(self.tableList, t)
    end
    function stack:pop()
        return table.remove(self.tableList)
    end
    function stack:contains(t)
        for _, v in ipairs(self.tableList) do
            if v == t then
                return true
            end
        end
        return false
    end
    return stack
end
 
-- 输出打印table表 函数
function PrintTable(...)
    local args = {...}
    for k, v in pairs(args) do
        local root = v
        if type(root) == "table" then
            local temp = {}
            local stack = newStack()
            local function table2String(t, depth)
                stack:push(t)
                if type(depth) == "number" then
                    depth = depth + 1
                else
                    depth = 1
                end
                local indent = ""
                for i=1, depth do
                    indent = indent .. "    "
                end
                for k, v in pairs(t) do
                    local key = tostring(k)
                    local typeV = type(v)
                    if typeV == "table" then
                        if key ~= "__valuePrototype" then
                            if stack:contains(v) then
                                table.insert(temp, indent..formatKey(key).." = {检测到循环引用!},\n")
                            else
                                table.insert(temp, indent..formatKey(key).." = {\n")
                                table2String(v, depth)
                                table.insert(temp, indent.."},\n")
                            end
                        end
                    elseif typeV == "string" then
                        table.insert(temp, string.format("%s%s = \"%s\",\n", indent, formatKey(key), tostring(v)))
                    else
                        table.insert(temp, string.format("%s%s = %s,\n", indent, formatKey(key), tostring(v)))
                    end
                end
                stack:pop()
            end
            table2String(root)
            logPrint(table.concat(temp))
        else
            logPrint(tostring(root))
        end
    end
end

--split
function string.split(str,sp)
    local result = {}
    local i = 0
    local j = 0
    local num = 1
    local pos = 0
	if not str or not sp then return "" end
    while true do
        i , j = string.find(str,sp,i+1)
        if i == nil then 
            if num ~=1 then
                result[num] = string.sub(str,pos,string.len(str))
			else
				return {str}
            end
            break 
        end
        result[num] = string.sub(str,pos,i-1)
        pos = i+string.len(sp)
        num = num +1
    end
    return result
end

include("main.lua")