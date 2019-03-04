require "Common.easyloader"

--主入口函数。从这里开始lua逻辑
function Main()					
	print("logic start")

	Easyloader.LoadBasicModules()

	StageManager.Init()
	PanelManager.Init()

	EventManager.SpreadEvent(GameEvent.CHANGE_STAGE,GameStage.Login)
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
	print("lua OnApplicationQuit")
end