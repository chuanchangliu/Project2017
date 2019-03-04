StageManager = {}
local this = StageManager

local eventMap = {}
local curStageEnum,curStageInst;

function this.Init()
    print("====StageManager.Init====")
    this.Reset()
    this.AllotEvent()

end


function this.Reset()
    print("====reset state====")
    curStage = GameStage.Patch;
end


function this.AllotEvent()
    local eventFuncMap = {
        [GameEvent.CHANGE_STAGE] = this.OnChangeStage,
    }

    eventMap = {}
    for event,func in pairs(eventFuncMap) do
        eventMap[event] = EventManager.ListenEvent(event,func);
    end
end


function this.GetStageInst(targetStage)

    if targetStage == GameStage.Patch then
        return PatchStage
    end

    if targetStage == GameStage.Login then
        print("====LoginStage====")
        return LoginStage
    end

    if targetStage == GameStage.Born then
        return BornStage
    end

    if targetStage == GameStage.City then
        return CityStage
    end

    if targetStage == GameStage.Play then
        return PlayStage
    end
    
    LogError("GetStageInst Error: no inst match gamestage: "..tostring(targetStage))
    return nil;
end

function this.OnChangeStage(targetStage)

    if curStageEnum == targetStage then
        LogError(targetStage + " is same stage !");
        return;
    end

    curStageEnum = targetStage;
    
    --open loading panel
    
    if curStageInst then
        curStageInst:Cleanup()
    end

    curStageInst = this.GetStageInst(curStageEnum)
    if curStageInst then
        curStageInst:Loading()
    end

end




return StageManager