StageInterface = {}
StageInterface.__classname = "StageInterface"
StageInterface.__singleton = true;
function StageInterface:Loading() LogError(self.__classname .. ' no implement function Loading()!') end
function StageInterface:Display() LogError(self.__classname .. ' no implement function Display()!') end
function StageInterface:Cleanup() LogError(self.__classname .. ' no implement function Cleanup()!') end

PatchStage = class("PatchStage",StageInterface)


LoginStage = class("LoginStage",StageInterface)
function LoginStage:Loading()
	PanelManager.OpenPanel("PanelLoading")
end

BornStage = class("BornStage",StageInterface)


CityStage = class("CityStage",StageInterface)


PlayStage = class("PlayStage",StageInterface)