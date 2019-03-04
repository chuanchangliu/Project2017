Easyloader = {}
local this = Easyloader;

function this.LoadBasicModules()
	local BasicModules = {
	"Common.predefine",
	"Common.functions",
	"Common.classwrap",
	"Common.uiusewrap",

	"Data.GlobalData",
	"Data.GameEvent",

	"Framework.StageCenter",

	"Manager.EventManager",
	"Manager.StageManager",
	"Manager.PanelManager",

	"View.PanelLoading"
	}

	for _,moduleName in ipairs(BasicModules) do
		require(moduleName)
	end
end


function this.LoadLogicModules()
end
