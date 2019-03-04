EventManager = {};
local this = EventManager;

local events = {}

function this.ListenEvent( msg , fun , obj, isSafe )
    local Event = events[msg]
    if Event == nil then
        events[msg] = event(msg,isSafe == nil and true or isSafe)
        Event = events[msg]
    end

    local listener = Event:CreateListener(fun,obj)
    Event:AddListener(listener)

    return listener
end

function this.SpreadEvent( msg , ... )
  local Event = events[msg] 
  if Event == nil then
    LogError("named " .. msg .. "msg has no event.")
    return
  end

  Event(...)
end

function this.RemoveEvent( msg , handle )
  local Event = events[msg]
  if Event == nil then
    log("named " .. msg .. "msg has no event.")
    return 
  end
  Event:RemoveListener(handle)
end

return EventManager;