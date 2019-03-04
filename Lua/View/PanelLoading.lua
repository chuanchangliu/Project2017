PanelLoading = {}
local this = PanelLoading

function this.Awake()
	print("====Lua Awake====")
end


function this.OnEnable()
	print("====Lua OnEnable====")
end


function this.Start()
	print("====Lua Start====")
end


function this.OnDisable()
	print("====Lua OnDisable====")
end

function this.OnDestroy()
	print("====Lua OnDestroy====")
end

return PanelLoading;