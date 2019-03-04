PanelManager = {}
local this = PanelManager;

local States = {
	Unload = 0,
	Show = 1,
	Hide = 2,
	Avoid = 3
}

local PanelSet = {}
local StateSet = {}

function this.Reset()

end

function this.Init()
	this.RegisterPanels()
end

--只有注册过的界面才会被正确的显示
function this.RegisterPanels()
	local function AddPanel(panel) PanelSet[string.lower(panel.name)] = panel end
	AddPanel({name="PanelLoading",prefab="UI/Panel_Loading",fullscreen = false, visible = false, eternal = false})
end


function this.GetPanel(panelName)
	local panelConfig = PanelSet[string.lower(panelName)]
	if panelConfig == nil then
		LogError(panelName.." is not valid, unregistered or misspelled ? check it please!")
	end
	return panelConfig
end


function this.OpenPanel(panelName,funcOnLoad)
	local panel = this.GetPanel(panelName)
	if panel == nil then return end

	if panel.gameObject == nil then
		panel.gameObject = CreateWindow(panelName,panel.prefab)
	end

	ShowWindow(panel.gameObject)
end


function this.ClosePanel(panelName)
	-- body
end


return PanelManager;