--[[
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
local t,l = ba3:ReadString(25) --t:读取的文本 l:
print(t,l)
print(ba3:ReadBool(26+l))
print(ba3:ReadInt(26+l+1))
print(ba,ba1,ba2,ba3)

local ba,e=LoadFile(GetDir() .. "t.txt")
if #ba.data == 0 then
	print(e)
else
	print(ba:ReadString(1))
	print(SaveFile(GetDir() .. "t2.txt",ba))
end

print(ba)
]]

