function Log(text)
    print("====functions Log====");
    EasyCall.Log(text)
end

function LogWarning(text)
    print("====functions LogWarning====")
    EasyCall.LogWarning(debug.traceback(text))
end

function  LogError(text)
    print("====functions LogError====")
    EasyCall.LogError(debug.traceback(text));
end

function LoadPrefab(filePath)
    return EasyCall.LoadPrefab(filePath)
end

function CreateWindow(name,prefab)
    return EasyCall.CreateWindow(name,prefab)
end

function ShowWindow(targetObject)
    EasyCall.ShowWindow(targetObject)
end

--多字符串的连接table.concat要比..操作快且省
function GetSpace(num)
    local strs = {};
    for i = 1,num do
        strs[i] = "  ";
    end

    return table.concat(strs);
end

function GetText(...)
    local args = {...}
    local strs = {}
    for id,value in pairs(args) do
        table.insert(strs,value)
    end

    return table.concat(strs)
end


function ParseTable(tmpTab,resultTexts,deepLevel)

    local space_left = deepLevel
    local space_deep = GetSpace(space_left)
    table.insert(resultTexts,space_deep)
    table.insert(resultTexts,"{\n");

    space_left = space_left + 1;
    local space_text = GetSpace(space_left);

    for key,value in pairs(tmpTab) do
        if type(value) == "string" then
            table.insert(resultTexts,GetText(space_text,"[",key,"]: \"",value,"\",\n"))
        
        elseif type(value) == "number" then
            table.insert(resultTexts,GetText(space_text,"[",key,"]: ",value,"\n"))
        
        elseif type(value) == "boolean" then
            table.insert(resultTexts,GetText(space_text,"[",key,"]: ",tostring(value),"\n"))
        
        elseif type(value) == "table" then
            table.insert(resultTexts,GetText(space_text,"[",key,"]: ","\n"))
            ParseTable(value,resultTexts,deepLevel+1)
        else
            table.insert(resultTexts,GetText(space_text,"[",key,"]: ",tostring(value),"\n"))
        end
    end

    table.insert(resultTexts,space_deep)
    table.insert(resultTexts,"}\n");

    return table.concat(resultTexts)
end

function PrintTable(tab)

    if type(tab) ~= "table" then
        print(debug.traceback("table need, get a "..type(tab)))
        return
    end

    local resultTexts = {};
    print(ParseTable(tab,resultTexts,0))
end


function ReadOnly (tab)
    local proxy = {}
    local mt = {       -- create metatable
        __index = tab,
        __newindex = function (tab,k,v) LogError("attempt to update a read-only table") end
    }
    
    setmetatable(proxy, mt)
    return proxy
end