local class_set = {}


function class(className,superClass)
	if type(superClass) ~= "table" then return nil end

	if rawget(superClass,"__singleton") then
		--print(superClass.__classname.." is singleton!")
	end

	local _class = {__classname = className}
	local metatable = {__index = superClass}
	return setmetatable(_class,metatable)
end